﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace OdataToEntity.AspNetCore
{
    public class OeMiddleware
    {
        private readonly PathString _apiPath;
        private readonly RequestDelegate _next;

        public OeMiddleware(RequestDelegate next, PathString apiPath, IEdmModel edmModel)
        {
            _next = next;
            _apiPath = apiPath;

            EdmModel = edmModel;
        }

        private static bool GetCsdlSchema(IEdmModel edmModel, Stream stream)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(stream))
                if (CsdlWriter.TryWriteCsdl(edmModel, xmlWriter, CsdlTarget.OData, out IEnumerable<EdmError> errors))
                    return true;

            return false;
        }
        protected virtual Query.OeModelBoundProvider GetModelBoundProvider(HttpContext httpContext)
        {
            return null;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/$metadata")
                InvokeMetadata(httpContext);
            else if (httpContext.Request.Path == "/$batch")
                await InvokeBatch(httpContext);
            else if (httpContext.Request.PathBase == _apiPath)
                await InvokeApi(httpContext);
            else
                await _next(httpContext);
        }
        private async Task InvokeApi(HttpContext httpContext)
        {
            var requestHeaders = (HttpRequestHeaders)httpContext.Request.Headers;
            ((IDictionary<String, StringValues>)requestHeaders).TryGetValue("Prefer", out StringValues preferHeader);
            OeRequestHeaders headers = OeRequestHeaders.Parse(requestHeaders.HeaderAccept, preferHeader);

            var parser = new OeParser(UriHelper.GetBaseUri(httpContext.Request), EdmModel, GetModelBoundProvider(httpContext), null);
            await parser.ExecuteGetAsync(UriHelper.GetUri(httpContext.Request), new OeHttpRequestHeaders(headers, httpContext.Response), httpContext.Response.Body, CancellationToken.None);
        }
        private async Task InvokeBatch(HttpContext httpContext)
        {
            httpContext.Response.ContentType = httpContext.Request.ContentType;
            var parser = new OeParser(UriHelper.GetBaseUri(httpContext.Request), EdmModel);
            await parser.ExecuteBatchAsync(httpContext.Request.Body, httpContext.Response.Body,
                httpContext.Request.ContentType, CancellationToken.None);
        }
        private void InvokeMetadata(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/xml";
            GetCsdlSchema(EdmModel, httpContext.Response.Body);
        }

        protected IEdmModel EdmModel { get; }
    }
}
