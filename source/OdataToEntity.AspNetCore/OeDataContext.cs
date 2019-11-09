﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OData;
using Microsoft.OData.Edm;
using OdataToEntity.Db;
using OdataToEntity.Parsers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace OdataToEntity.AspNetCore
{
    internal sealed class OeDataContextBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelState is OeBatchFilterAttributeAttribute.BatchModelStateDictionary batchModelState)
            {
                bindingContext.Result = ModelBindingResult.Success(batchModelState.DataContext);
                return Task.CompletedTask;
            }

            throw new InvalidOperationException(nameof(OeDataContext) + " must be create in " + nameof(OeBatchController));
        }
    }

    [ModelBinder(BinderType = typeof(OeDataContextBinder))]
    public sealed class OeDataContext
    {
        private readonly OeEntitySetAdapter _entitySetAdapter;
        private readonly OeOperationMessage _operation;

        public OeDataContext(OeEntitySetAdapter entitySetAdapter, IEdmModel edmModel, Object dataContext, in OeOperationMessage operation)
        {
            _entitySetAdapter = entitySetAdapter;
            DbContext = dataContext;
            EdmModel = edmModel;
            _operation = operation;
        }

        private ODataResource CreateEntry(Object entity)
        {
            IEdmEntitySet entitySet = OeEdmClrHelper.GetEntitySet(EdmModel, _entitySetAdapter.EntitySetName);
            var structuralProperties = new List<PropertyInfo>();
            foreach (IEdmStructuralProperty structuralProperty in entitySet.EntityType().StructuralProperties())
            {
                PropertyInfo? propertyInfo = _entitySetAdapter.EntityType.GetProperty(structuralProperty.Name);
                if (propertyInfo == null)
                    throw new InvalidOperationException("Not found property " + structuralProperty.Name + " in type " + _entitySetAdapter.EntityType.FullName);
                structuralProperties.Add(propertyInfo);
            }
            return CreateEntry(entity, structuralProperties);
        }
        private ODataResource CreateEntry(IDictionary<String, Object> entityProperties)
        {
            var odataProperties = new ODataProperty[entityProperties.Count];
            int i = 0;
            foreach (KeyValuePair<String, Object> entityProperty in entityProperties)
            {
                ODataValue odataValue = OeEdmClrHelper.CreateODataValue(entityProperty.Value);
                odataProperties[i++] = new ODataProperty() { Name = entityProperty.Key, Value = odataValue };
            }

            return new ODataResource
            {
                TypeName = _entitySetAdapter.EntityType.FullName,
                Properties = odataProperties
            };
        }
        private static ODataResource CreateEntry(Object entity, List<PropertyInfo> structuralProperties)
        {
            Type clrEntityType = entity.GetType();
            var odataProperties = new ODataProperty[structuralProperties.Count];
            for (int i = 0; i < odataProperties.Length; i++)
            {
                ODataValue odataValue = OeEdmClrHelper.CreateODataValue(structuralProperties[i].GetValue(entity));
                odataProperties[i] = new ODataProperty() { Name = structuralProperties[i].Name, Value = odataValue };
            }

            return new ODataResource
            {
                TypeName = clrEntityType.FullName,
                Properties = odataProperties
            };
        }
        public void Update(Object entity)
        {
            ODataResource entry;
            switch (_operation.Method)
            {
                case ODataConstants.MethodDelete:
                    entry = CreateEntry(entity);
                    _entitySetAdapter.RemoveEntity(DbContext, entry);
                    break;
                case ODataConstants.MethodPatch:
                    entry = CreateEntry((IDictionary<String, Object>)entity);
                    _entitySetAdapter.AttachEntity(DbContext, entry);
                    break;
                case ODataConstants.MethodPost:
                    entry = CreateEntry(entity);
                    _entitySetAdapter.AddEntity(DbContext, entry);
                    break;
                default:
                    throw new NotImplementedException(_operation.Method);
            }
        }

        public Object DbContext { get; }
        public IEdmModel EdmModel { get; }
        public String HttpMethod => _operation.Method;
    }
}
