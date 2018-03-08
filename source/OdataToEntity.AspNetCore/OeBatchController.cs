﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.OData.Edm;
using OdataToEntity.Db;
using OdataToEntity.Parsers;
using OdataToEntity.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdataToEntity.AspNetCore
{
    public class OeBatchController : ControllerBase
    {
        private OeDataAdapter _dataAdapter;
        private IEdmModel _edmModel;

        [HttpPost]
        public Task Batch() => BatchCore();
        [HttpPost]
        protected virtual async Task BatchCore()
        {
            var actionDescriptors = GetService<IActionDescriptorCollectionProvider>().ActionDescriptors;
            var actionInvokerFactory = GetService<IActionInvokerFactory>();

            String basePath = "";
            if (base.HttpContext.Request.PathBase.HasValue)
                basePath = base.HttpContext.Request.PathBase;
            else
            {
                int i = base.HttpContext.Request.Path.Value.IndexOf('/', 1);
                if (i > 0)
                    basePath = base.HttpContext.Request.Path.Value.Substring(0, i);
            }
            Uri baseUri = UriHelper.GetBaseUri(base.Request);

            var messageContext = new OeMessageContext(baseUri, EdmModel, DataAdapter.EntitySetMetaAdapters);
            OeBatchMessage batchMessage = OeBatchMessage.CreateBatchMessage(messageContext, base.HttpContext.Request.Body, base.HttpContext.Request.ContentType);

            Object dataContext = null;
            try
            {
                dataContext = DataAdapter.CreateDataContext();

                foreach (OeOperationMessage operation in batchMessage.Changeset)
                {
                    OeEntitySetAdapter entitySetAdapter = DataAdapter.GetEntitySetAdapter(operation.EntityItem.EntitySet.Name);
                    String path = basePath + "/" + entitySetAdapter.EntitySetMetaAdapter.EntitySetName;

                    List<ActionDescriptor> candidates = OeRouter.SelectCandidates(actionDescriptors.Items, base.RouteData.Values, path, operation.Method);
                    if (candidates.Count > 1)
                        throw new AmbiguousActionException(String.Join(Environment.NewLine, candidates.Select(c => c.DisplayName)));
                    if (candidates.Count == 0)
                        throw new InvalidOperationException("Action " + operation.Method + " for controller " + basePath + " not found");

                    var modelState = new OeFilterAttribute.BatchModelStateDictionary()
                    {
                        Entity = operation.EntityItem.Entity,
                        DataContext = new OeDataContext(ref entitySetAdapter, dataContext, operation)
                    };
                    OnBeforeInvokeController(modelState.DataContext, operation.EntityItem);

                    var actionContext = new ActionContext(base.HttpContext, base.HttpContext.GetRouteData(), candidates[0], modelState);
                    IActionInvoker actionInvoker = actionInvokerFactory.CreateInvoker(actionContext);
                    await actionInvoker.InvokeAsync();
                }

                await SaveChangesAsync(dataContext).ConfigureAwait(false);
            }
            finally
            {
                if (dataContext != null)
                    DataAdapter.CloseDataContext(dataContext);
            }

            base.HttpContext.Response.ContentType = base.HttpContext.Request.ContentType;
            var batchWriter = new OeBatchWriter(baseUri, EdmModel);
            batchWriter.Write(base.HttpContext.Response.Body, batchMessage);
        }
        protected virtual void OnBeforeInvokeController(OeDataContext dataContext, OeEntityItem entityItem)
        {
        }
        private T GetService<T>() => (T)base.HttpContext.RequestServices.GetService(typeof(T));
        protected virtual async Task<int> SaveChangesAsync(Object dataContext)
        {
            return await DataAdapter.SaveChangesAsync(EdmModel, dataContext, CancellationToken.None).ConfigureAwait(false);
        }
        protected async Task SaveWithoutController()
        {
            base.HttpContext.Response.ContentType = base.HttpContext.Request.ContentType;

            var parser = new OeBatchParser(UriHelper.GetBaseUri(base.Request), DataAdapter, EdmModel);
            await parser.ExecuteAsync(base.HttpContext.Request.Body, base.HttpContext.Response.Body,
                base.HttpContext.Request.ContentType, CancellationToken.None).ConfigureAwait(false);
        }

        protected OeDataAdapter DataAdapter
        {
            get
            {
                OeDataAdapter dataAdapter = Volatile.Read(ref _dataAdapter);
                if (dataAdapter == null)
                {
                    dataAdapter = GetService<OeDataAdapter>();
                    Interlocked.CompareExchange(ref _dataAdapter, dataAdapter, null);
                }
                return dataAdapter;
            }
        }
        protected IEdmModel EdmModel
        {
            get
            {
                IEdmModel edmModel = Volatile.Read(ref _edmModel);
                if (edmModel == null)
                {
                    edmModel = GetService<IEdmModel>();
                    Interlocked.CompareExchange(ref _edmModel, edmModel, null);
                }
                return edmModel;
            }
        }
    }
}
