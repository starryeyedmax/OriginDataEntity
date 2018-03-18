﻿using Microsoft.OData;
using Microsoft.OData.Edm;
using OdataToEntity.Parsers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OdataToEntity.Ef6
{
    public class OeEf6DataAdapter<T> : Db.OeDataAdapter where T : DbContext
    {
        private sealed class DbSetAdapterImpl<TEntity> : Db.OeEntitySetMetaAdapter where TEntity : class
        {
            private readonly Func<T, IDbSet<TEntity>> _getEntitySet;
            private bool _isCascade;
            private bool _isInitialized;
            private bool _isSelfReference;

            public DbSetAdapterImpl(Func<T, IDbSet<TEntity>> getEntitySet, String entitySetName)
            {
                _getEntitySet = getEntitySet;
                EntitySetName = entitySetName;
            }

            public override void AddEntity(Object dataContext, ODataResourceBase entry)
            {
                IDbSet<TEntity> dbSet = _getEntitySet((T)dataContext);
                var entity = (TEntity)OeEdmClrHelper.CreateEntity(EntityType, entry);
                dbSet.Add(entity);
            }
            public override void AttachEntity(Object dataContext, ODataResourceBase entry)
            {
                var entity = (TEntity)OeEdmClrHelper.CreateEntity(EntityType, entry);
                ObjectContext objectContext = ((IObjectContextAdapter)dataContext).ObjectContext;
                EntityKey entityKey = objectContext.CreateEntityKey(EntitySetName, entity);

                if (objectContext.ObjectStateManager.TryGetObjectStateEntry(entityKey, out ObjectStateEntry objectStateEntry))
                {
                    foreach (ODataProperty odataProperty in entry.Properties)
                        if (Array.Find(objectStateEntry.EntityKey.EntityKeyValues, k => k.Key == odataProperty.Name) == null)
                        {
                            int i = objectStateEntry.CurrentValues.GetOrdinal(odataProperty.Name);
                            objectStateEntry.CurrentValues.SetValue(i, odataProperty.Value);
                        }
                }
                else
                {
                    var context = (T)dataContext;
                    _getEntitySet(context).Attach(entity);
                    objectContext.ObjectStateManager.TryGetObjectStateEntry(entityKey, out objectStateEntry);

                    foreach (ODataProperty odataProperty in entry.Properties)
                        if (Array.Find(objectStateEntry.EntityKey.EntityKeyValues, k => k.Key == odataProperty.Name) == null)
                            objectStateEntry.SetModifiedProperty(odataProperty.Name);
                }
            }
            public override IQueryable GetEntitySet(Object dataContext)
            {
                return _getEntitySet((T)dataContext);
            }
            private void InitKey(T context)
            {
                if (_isInitialized)
                    return;

                ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var itemCollection = (ObjectItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.OSpace);
                EntityType entityType = itemCollection.OfType<EntityType>().Single(e => itemCollection.GetClrType(e) == typeof(TEntity));

                bool isCascade = true;
                foreach (AssociationType associationType in objectContext.MetadataWorkspace.GetItems<AssociationType>(DataSpace.CSpace))
                    if (associationType.Constraint.ToRole.GetEntityType().Name == typeof(TEntity).Name &&
                        associationType.Constraint.ToRole.DeleteBehavior == OperationAction.None)
                    {
                        isCascade = false;
                        break;
                    }

                bool isSelfReference = false;
                foreach (NavigationProperty navigationProperty in entityType.NavigationProperties)
                    if (navigationProperty.FromEndMember.GetEntityType() == navigationProperty.ToEndMember.GetEntityType())
                    {
                        isSelfReference = true;
                        break;
                    }

                Volatile.Write(ref _isCascade, isCascade);
                Volatile.Write(ref _isSelfReference, isSelfReference);
                Volatile.Write(ref _isInitialized, true);
            }
            public override void RemoveEntity(Object dataContext, ODataResourceBase entry)
            {
                var context = (T)dataContext;
                InitKey(context);

                ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var entity = (TEntity)OeEdmClrHelper.CreateEntity(EntityType, entry);
                EntityKey entityKey = objectContext.CreateEntityKey(EntitySetName, entity);

                if (objectContext.ObjectStateManager.TryGetObjectStateEntry(entityKey, out ObjectStateEntry objectStateEntry))
                    objectStateEntry.ChangeState(EntityState.Deleted);
                else
                {
                    if (_isCascade && !_isSelfReference)
                        context.Entry(entity).State = EntityState.Deleted;
                    else
                    {
                        var keyValues = new Object[entityKey.EntityKeyValues.Length];
                        for (int i = 0; i < keyValues.Length; i++)
                            keyValues[i] = entityKey.EntityKeyValues[i].Value;

                        IDbSet<TEntity> dbSet = _getEntitySet(context);
                        context.Entry(dbSet.Find(keyValues)).State = EntityState.Deleted;
                    }
                }
            }

            public override Type EntityType => typeof(TEntity);
            public override String EntitySetName { get; }
        }

        private readonly static Db.OeEntitySetMetaAdapterCollection _entitySetMetaAdapters = CreateEntitySetMetaAdapters();

        public OeEf6DataAdapter() : this(null, new OeEf6OperationAdapter(typeof(T)))
        {
        }
        public OeEf6DataAdapter(Db.OeQueryCache queryCache, OeEf6OperationAdapter operationAdapter)
            : base(queryCache, operationAdapter)
        {
        }

        public override void CloseDataContext(Object dataContext)
        {
            var dbContext = (T)dataContext;
            dbContext.Dispose();
        }
        public override Object CreateDataContext()
        {
            T dbContext = Db.FastActivator.CreateInstance<T>();
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.AutoDetectChangesEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;
            return dbContext;
        }
        private static Db.OeEntitySetMetaAdapterCollection CreateEntitySetMetaAdapters()
        {
            var entitySetMetaAdapters = new List<Db.OeEntitySetMetaAdapter>();
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                Type dbSetType = property.PropertyType.GetInterface(typeof(IDbSet<>).FullName);
                if (dbSetType != null)
                    entitySetMetaAdapters.Add(CreateDbSetInvoker(property, dbSetType));
            }
            return new Db.OeEntitySetMetaAdapterCollection(entitySetMetaAdapters.ToArray(), new ModelBuilder.OeEdmModelMetadataProvider());
        }
        private static Db.OeEntitySetMetaAdapter CreateDbSetInvoker(PropertyInfo property, Type dbSetType)
        {
            MethodInfo mi = ((Func<PropertyInfo, Db.OeEntitySetMetaAdapter>)CreateDbSetInvoker<Object>).Method.GetGenericMethodDefinition();
            Type entityType = dbSetType.GetGenericArguments()[0];
            MethodInfo func = mi.GetGenericMethodDefinition().MakeGenericMethod(entityType);
            return (Db.OeEntitySetMetaAdapter)func.Invoke(null, new Object[] { property });
        }
        private static Db.OeEntitySetMetaAdapter CreateDbSetInvoker<TEntity>(PropertyInfo property) where TEntity : class
        {
            var getDbSet = (Func<T, IDbSet<TEntity>>)Delegate.CreateDelegate(typeof(Func<T, IDbSet<TEntity>>), property.GetGetMethod());
            return new DbSetAdapterImpl<TEntity>(getDbSet, property.Name);
        }
        public override Db.OeAsyncEnumerator ExecuteEnumerator(Object dataContext, OeQueryContext queryContext, CancellationToken cancellationToken)
        {
            IQueryable query = queryContext.EntitySetAdapter.GetEntitySet(dataContext);
            Expression expression = queryContext.CreateExpression(query, new OeConstantToVariableVisitor(queryContext.SkipTokenParser != null));

            expression = new EnumerableToQuerableVisitor().Visit(expression);
            var queryAsync = (IDbAsyncEnumerable)query.Provider.CreateQuery(expression);
            Db.OeAsyncEnumerator asyncEnumerator = new OeEf6AsyncEnumerator(queryAsync.GetAsyncEnumerator(), cancellationToken);
            if (queryContext.CountExpression != null)
                asyncEnumerator.Count = query.Provider.Execute<int>(queryContext.CountExpression);

            return asyncEnumerator;
        }
        public override TResult ExecuteScalar<TResult>(Object dataContext, OeQueryContext queryContext)
        {
            IQueryable query = queryContext.EntitySetAdapter.GetEntitySet(dataContext);
            Expression expression = queryContext.CreateExpression(query, new OeConstantToVariableVisitor(false));
            return query.Provider.Execute<TResult>(expression);
        }
        public override Db.OeEntitySetAdapter GetEntitySetAdapter(String entitySetName)
        {
            return new Db.OeEntitySetAdapter(_entitySetMetaAdapters.FindByEntitySetName(entitySetName), this);
        }
        public override Task<int> SaveChangesAsync(IEdmModel edmModel, Object dataContext, CancellationToken cancellationToken)
        {
            var dbContext = (T)dataContext;
            return dbContext.SaveChangesAsync(cancellationToken);
        }

        public override Db.OeEntitySetMetaAdapterCollection EntitySetMetaAdapters => _entitySetMetaAdapters;
    }
}
