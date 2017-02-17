﻿using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OdataToEntity.Parsers
{
    public struct OeParseNavigationSegment
    {
        private readonly FilterClause _filter;
        private readonly NavigationPropertySegment _navigationSegment;

        public OeParseNavigationSegment(NavigationPropertySegment navigationSegment, FilterClause filter)
        {
            _navigationSegment = navigationSegment;
            _filter = filter;
        }

        public FilterClause Filter => _filter;
        public NavigationPropertySegment NavigationSegment => _navigationSegment;
    }

    public sealed class OeParseUriContext
    {
        private sealed class SourceVisitor : ExpressionVisitor
        {
            private readonly IQueryable _query;

            private SourceVisitor(IQueryable query)
            {
                _query = query;
            }

            public static Expression Translate(IQueryable query, Expression expression)
            {
                var visitor = new SourceVisitor(query);
                return visitor.Visit(expression);
            }
            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Type.GetTypeInfo().IsGenericType)
                {
                    Type[] args = node.Type.GetTypeInfo().GetGenericArguments();
                    if (args.Length == 1 && args[0] == _query.ElementType)
                        return _query.Expression;
                }
                return base.VisitConstant(node);
            }
        }

        private readonly IEdmModel _edmModel;
        private readonly IEdmEntitySet _entitySet;
        private readonly ODataUri _odataUri;
        private readonly IReadOnlyList<OeParseNavigationSegment> _parseNavigationSegments;

        public OeParseUriContext(IEdmModel edmModel, ODataUri odataUri, IEdmEntitySet entitySet, IReadOnlyList<OeParseNavigationSegment> parseNavigationSegments)
        {
            _edmModel = edmModel;
            _odataUri = odataUri;
            _entitySet = entitySet;
            _parseNavigationSegments = parseNavigationSegments;
        }

        private OeEntryFactory CreateEntryFactory(OeExpressionBuilder expressionBuilder)
        {
            IEdmEntitySet entitySet = EntitySet;
            if (expressionBuilder.EntityType != EntitySetAdapter.EntityType)
            {
                String typeName = expressionBuilder.EntityType.FullName;
                Db.OeDataAdapter dataAdapter = EntitySetAdapter.DataAdapter;
                Db.OeEntitySetMetaAdapter entitySetMetaAdapter = dataAdapter.EntitySetMetaAdapters.FindByTypeName(typeName);
                if (entitySetMetaAdapter != null)
                    entitySet = EdmModel.FindDeclaredEntitySet(entitySetMetaAdapter.EntitySetName);
            }
            return expressionBuilder.CreateEntryFactory(entitySet);
        }
        public Expression CreateExpression(IQueryable query, OeConstantToVariableVisitor constantToParameterVisitor)
        {
            Expression expression;
            var expressionBuilder = new OeExpressionBuilder(EdmModel, EntitySetAdapter.EntityType);

            expression = Expression.Constant(null, typeof(IEnumerable<>).MakeGenericType(EntitySetAdapter.EntityType));
            expression = expressionBuilder.ApplyNavigation(expression, ParseNavigationSegments);
            expression = expressionBuilder.ApplyFilter(expression, ODataUri.Filter);
            expression = expressionBuilder.ApplyAggregation(expression, ODataUri.Apply);
            expression = expressionBuilder.ApplySelect(expression, ODataUri.SelectAndExpand, Headers.MetadataLevel);
            expression = expressionBuilder.ApplyOrderBy(expression, ODataUri.OrderBy);
            expression = expressionBuilder.ApplySkip(expression, ODataUri.Skip);
            expression = expressionBuilder.ApplyTake(expression, ODataUri.Top);
            expression = expressionBuilder.ApplyCount(expression, ODataUri.QueryCount);

            if (!ODataUri.QueryCount.GetValueOrDefault())
                EntryFactory = CreateEntryFactory(expressionBuilder);

            //expression = constantToParameterVisitor.Translate(expression, expressionBuilder.Constants);
            return SourceVisitor.Translate(query, expression);
        }

        public IEdmModel EdmModel => _edmModel;
        public IEdmEntitySet EntitySet => _entitySet;
        public Db.OeEntitySetAdapter EntitySetAdapter { get; set; }
        public OeEntryFactory EntryFactory { get; set; }
        public OeRequestHeaders Headers { get; set; }
        public IReadOnlyList<OeParseNavigationSegment> ParseNavigationSegments => _parseNavigationSegments;
        public ODataUri ODataUri => _odataUri;
    }
}
