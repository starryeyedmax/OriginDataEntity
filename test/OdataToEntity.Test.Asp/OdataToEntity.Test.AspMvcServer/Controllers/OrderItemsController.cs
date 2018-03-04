﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using OdataToEntity.AspNetCore;

namespace OdataToEntity.Test.AspMvcServer.Controllers
{
    [Route("api/[controller]")]
    public sealed class OrderItemsController : OeControllerBase
    {
        public OrderItemsController(Db.OeDataAdapter dataAdapter, IEdmModel edmModel)
            : base(dataAdapter, edmModel)
        {
        }

        [HttpDelete]
        public void Delete(OeDataContext dataContext, Model.OrderItem orderItem)
        {
            dataContext.Update(orderItem);
        }
        [HttpGet]
        public IActionResult Get()
        {
            Db.OeAsyncEnumerator asyncEnumerator = base.GetAsyncEnumerator(base.HttpContext, base.HttpContext.Response.Body);
            return base.OData(asyncEnumerator);
        }
        [HttpPatch]
        public void Patch(OeDataContext dataContext, Model.OrderItem orderItem)
        {
            dataContext.Update(orderItem);
        }
        [HttpPost]
        public void Post(OeDataContext dataContext, Model.OrderItem orderItem)
        {
            dataContext.Update(orderItem);
        }
    }
}