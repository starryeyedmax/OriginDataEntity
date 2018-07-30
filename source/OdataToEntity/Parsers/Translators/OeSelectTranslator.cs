﻿using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OdataToEntity.Parsers.Translators
{
    public sealed class OeSelectTranslator
    {
        private sealed class ComputeProperty : IEdmProperty
        {
            public ComputeProperty(String alias, IEdmTypeReference edmTypeReference, Expression expression)
            {
                Name = alias;
                Type = edmTypeReference;
                Expression = expression;
            }

            public IEdmStructuredType DeclaringType => ModelBuilder.PrimitiveTypeHelper.TupleEdmType;
            public Expression Expression { get; }
            public String Name { get; }
            public EdmPropertyKind PropertyKind => throw new NotSupportedException();
            public IEdmTypeReference Type { get; }
        }

        private sealed class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly Expression _source;

            public ReplaceParameterVisitor(Expression source)
            {
                _source = source;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node.Type == _source.Type ? _source : base.VisitParameter(node);
            }
        }

        private readonly OeJoinBuilder _joinBuilder;
        private readonly OeSelectItem _navigationItem;
        private readonly OeQueryNodeVisitor _visitor;
        private OeEntryFactory _entryFactory;

        public OeSelectTranslator(OeJoinBuilder joinBuilder, ODataPath path) :
            this(joinBuilder, new OeSelectItem(null, GetEntitySet(path), path, null, null, null, false))
        {
        }
        internal OeSelectTranslator(OeJoinBuilder joinBuilder, OeSelectItem navigationItem)
        {
            _joinBuilder = joinBuilder;
            _navigationItem = navigationItem;
            _visitor = joinBuilder.Visitor;
        }

        private void AddKey(IEdmEntityType edmEntityType)
        {
            foreach (IEdmStructuralProperty keyProperty in edmEntityType.DeclaredKey)
                _navigationItem.AddSelectItem(new OeSelectItem(keyProperty, false));
        }
        private static bool HasSelectItems(SelectExpandClause selectExpandClause)
        {
            if (selectExpandClause != null)
                foreach (SelectItem odataSelectItem in selectExpandClause.SelectedItems)
                    if (odataSelectItem is PathSelectItem pathSelectItem && pathSelectItem.SelectedPath.LastSegment is PropertySegment)
                        return true;

            return false;
        }
        public Expression Build(Expression source, OeQueryContext queryContext)
        {
            bool isBuild = false;
            if (queryContext.ODataUri.OrderBy != null && queryContext.PageSize > 0)
            {
                isBuild = true;
                source = BuildSource(source, queryContext, _navigationItem);
            }

            if (queryContext.ODataUri.SelectAndExpand != null)
            {
                isBuild = true;
                source = BuildSelect(queryContext.ODataUri.SelectAndExpand, source, queryContext.NavigationNextLink, false);

                if (_navigationItem.HasSelectItems && queryContext.MetadataLevel == OeMetadataLevel.Full)
                    AddKey(queryContext.EntitySet.EntityType());
            }

            if (queryContext.ODataUri.Compute != null)
            {
                isBuild = true;
                BuildCompute(queryContext.ODataUri.Compute);
            }

            if (isBuild)
            {
                source = SelectStructuralProperties(source, _navigationItem);
                source = CreateSelectExpression(source, _joinBuilder);
                _entryFactory = CreateEntryFactory(_navigationItem, source);
            }

            return source;
        }
        private void BuildCompute(ComputeClause computeClause)
        {
            foreach (ComputeExpression computeExpression in computeClause.ComputedItems)
            {
                Expression expression = _visitor.TranslateNode(computeExpression.Expression);
                IEdmTypeReference edmTypeReference = OeEdmClrHelper.GetEdmTypeReference(_visitor.EdmModel, expression.Type);
                _navigationItem.AddSelectItem(new OeSelectItem(new ComputeProperty(computeExpression.Alias, edmTypeReference, expression), false));
            }
        }
        private static Expression BuildOrderBySkipToken(OeSelectItem navigationItem, OrderByClause orderByClause, Expression source, OeJoinBuilder joinBuilder, bool hasSelectItems)
        {
            while (orderByClause != null)
            {
                var propertyNode = (SingleValuePropertyAccessNode)orderByClause.Expression;
                if (propertyNode.Source is SingleNavigationNode navigationNode)
                {
                    OeSelectItem match = null;
                    ExpandedNavigationSelectItem navigationSelectItem = null;
                    do
                    {
                        if ((match = navigationItem.FindHierarchyNavigationItem(navigationNode.NavigationProperty)) != null)
                        {
                            match.AddSelectItem(new OeSelectItem(propertyNode.Property, true));
                            break;
                        }

                        SelectExpandClause selectExpandClause;
                        if (navigationSelectItem == null)
                        {
                            var pathSelectItem = new PathSelectItem(new ODataSelectPath(new PropertySegment((IEdmStructuralProperty)propertyNode.Property)));
                            selectExpandClause = new SelectExpandClause(new[] { pathSelectItem }, false);
                        }
                        else
                            selectExpandClause = new SelectExpandClause(new[] { navigationSelectItem }, false);

                        var segment = new NavigationPropertySegment(navigationNode.NavigationProperty, navigationNode.NavigationSource);
                        navigationSelectItem = new ExpandedNavigationSelectItem(new ODataExpandPath(segment), navigationNode.NavigationSource, selectExpandClause);
                    }
                    while ((navigationNode = navigationNode.Source as SingleNavigationNode) != null);

                    if (navigationSelectItem != null)
                    {
                        if (match == null)
                            match = navigationItem;

                        var selectItemTranslator = new OeSelectItemTranslator(match, false, source, joinBuilder, true);
                        navigationSelectItem.TranslateWith(selectItemTranslator);
                        source = selectItemTranslator.Source;
                    }
                }
                else
                {
                    if (hasSelectItems)
                        navigationItem.AddSelectItem(new OeSelectItem(propertyNode.Property, true));
                }

                orderByClause = orderByClause.ThenBy;
            }

            return source;
        }
        public Expression BuildSelect(SelectExpandClause selectClause, Expression source, bool navigationNextLink, bool skipToken)
        {
            foreach (SelectItem selectItemClause in selectClause.SelectedItems)
            {
                var selectItemTranslator = new OeSelectItemTranslator(_navigationItem, navigationNextLink, source, _joinBuilder, skipToken);
                OeSelectItem selectItem = selectItemClause.TranslateWith(selectItemTranslator);
                if (selectItem != null)
                {
                    if (selectItem.EdmProperty is IEdmNavigationProperty)
                        source = selectItemTranslator.Source;
                    else
                        _navigationItem.AddSelectItem(selectItem);
                }
            }

            return source;
        }
        private static Expression BuildSource(Expression source, OeQueryContext queryContext, OeSelectItem navigationItem)
        {
            bool hasSelectItems = HasSelectItems(queryContext.ODataUri.SelectAndExpand);
            source = BuildOrderBySkipToken(navigationItem, queryContext.ODataUri.OrderBy, source, queryContext.JoinBuilder, hasSelectItems);
            queryContext.JoinBuilder.Visitor.ChangeParameterType(source);

            var expressionBuilder = new OeExpressionBuilder(queryContext.JoinBuilder);
            source = expressionBuilder.ApplySkipToken(source, queryContext.SkipTokenNameValues, queryContext.ODataUri.OrderBy, queryContext.IsDatabaseNullHighestValue);
            source = expressionBuilder.ApplyOrderBy(source, queryContext.ODataUri.OrderBy);
            source = expressionBuilder.ApplySkip(source, queryContext.ODataUri.Skip, queryContext.ODataUri.Path);
            return expressionBuilder.ApplyTake(source, queryContext.ODataUri.Top, queryContext.ODataUri.Path);
        }
        private static OeEntryFactory CreateEntryFactory(OeSelectItem root, Expression source)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(Object));
            UnaryExpression typedParameter = Expression.Convert(parameter, OeExpressionHelper.GetCollectionItemType(source.Type));

            if (root.HasNavigationItems)
            {
                List<OeSelectItem> navigationItems = FlattenNavigationItems(root, false);
                IReadOnlyList<MemberExpression> navigationProperties = OeExpressionHelper.GetPropertyExpressions(typedParameter);
                for (int i = navigationItems.Count - 1; i >= 0; i--)
                {
                    OeSelectItem navigationItem = navigationItems[i];
                    OeEntryFactory[] nestedNavigationLinks = GetNestedNavigationLinks(navigationItem);

                    OeEntryFactory entryFactory;
                    OePropertyAccessor[] accessors = GetAccessors(navigationProperties[i].Type, navigationItem.EntitySet, navigationItem.SelectItems, source);
                    if (i == 0)
                        entryFactory = OeEntryFactory.CreateEntryFactoryParent(navigationItem.EntitySet, accessors, nestedNavigationLinks);
                    else
                        entryFactory = OeEntryFactory.CreateEntryFactoryNested(navigationItem.EntitySet, accessors, navigationItem.Resource, nestedNavigationLinks);

                    entryFactory.LinkAccessor = (Func<Object, Object>)Expression.Lambda(navigationProperties[i], parameter).Compile();
                    entryFactory.CountOption = navigationItem.CountOption;
                    navigationItem.EntryFactory = entryFactory;
                }
            }
            else
            {
                IReadOnlyList<MemberExpression> propertyExpressions = OeExpressionHelper.GetPropertyExpressions(typedParameter);
                OePropertyAccessor[] accessors;
                if (root.SelectItems.Count == 0)
                    accessors = OePropertyAccessor.CreateFromType(typedParameter.Type, root.EntitySet);
                else
                {
                    var accessorList = new List<OePropertyAccessor>(root.SelectItems.Count);
                    for (int i = 0; i < root.SelectItems.Count; i++)
                        if (!root.SelectItems[i].SkipToken)
                            accessorList.Add(OePropertyAccessor.CreatePropertyAccessor(root.SelectItems[i].EdmProperty, propertyExpressions[i], parameter));
                    accessors = accessorList.ToArray();
                }
                root.EntryFactory = OeEntryFactory.CreateEntryFactory(root.EntitySet, accessors);
            }

            return root.EntryFactory;
        }
        private MethodCallExpression CreateSelectExpression(Expression source, OeJoinBuilder joinBuilder)
        {
            if (_navigationItem.HasNavigationItems)
                return (MethodCallExpression)source;

            if (!_navigationItem.HasSelectItems)
                return (MethodCallExpression)source;

            var expressions = new List<Expression>(_navigationItem.SelectItems.Count);
            for (int i = 0; i < _navigationItem.SelectItems.Count; i++)
            {
                IEdmProperty edmProperty = _navigationItem.SelectItems[i].EdmProperty;
                PropertyInfo clrProperty = OeEdmClrHelper.GetPropertyIgnoreCase(_visitor.Parameter.Type, edmProperty);
                expressions.Add(Expression.MakeMemberAccess(_visitor.Parameter, clrProperty));
            }
            NewExpression newTupleExpression = OeExpressionHelper.CreateTupleExpression(expressions);

            LambdaExpression lambda = Expression.Lambda(newTupleExpression, _visitor.Parameter);
            MethodInfo selectMethodInfo = OeMethodInfoHelper.GetSelectMethodInfo(_visitor.Parameter.Type, newTupleExpression.Type);
            return Expression.Call(selectMethodInfo, source, lambda);
        }
        private static List<OeSelectItem> FlattenNavigationItems(OeSelectItem root, bool skipToken)
        {
            var navigationItems = new List<OeSelectItem>();
            var stack = new Stack<ValueTuple<OeSelectItem, int>>();
            stack.Push(new ValueTuple<OeSelectItem, int>(root, 0));
            do
            {
                ValueTuple<OeSelectItem, int> stackItem = stack.Pop();
                if (stackItem.Item2 == 0 && (!stackItem.Item1.SkipToken || skipToken))
                    navigationItems.Add(stackItem.Item1);

                if (stackItem.Item2 < stackItem.Item1.NavigationItems.Count)
                {
                    stack.Push(new ValueTuple<OeSelectItem, int>(stackItem.Item1, stackItem.Item2 + 1));
                    OeSelectItem selectItem = stackItem.Item1.NavigationItems[stackItem.Item2];
                    stack.Push(new ValueTuple<OeSelectItem, int>(selectItem, 0));
                }
            }
            while (stack.Count > 0);
            return navigationItems;
        }
        private static OePropertyAccessor[] GetAccessors(Type clrEntityType, IEdmEntitySet entitySet, IReadOnlyList<OeSelectItem> selectItems, Expression source)
        {
            if (selectItems.Count == 0)
                return OePropertyAccessor.CreateFromType(clrEntityType, entitySet);

            var accessorList = new List<OePropertyAccessor>(selectItems.Count);

            ParameterExpression accessorParameter = Expression.Parameter(typeof(Object));
            UnaryExpression typedAccessorParameter = Expression.Convert(accessorParameter, clrEntityType);
            for (int i = 0; i < selectItems.Count; i++)
                if (!selectItems[i].SkipToken)
                {
                    Expression accessorExpression;
                    if (selectItems[i].EdmProperty is ComputeProperty computeProperty)
                        accessorExpression = OeExpressionHelper.GetPropertyExpressions(typedAccessorParameter)[i];
                    else
                    {
                        PropertyInfo clrProperty = clrEntityType.GetPropertyIgnoreCase(selectItems[i].EdmProperty);
                        if (clrProperty == null)
                        {
                            accessorExpression = new OePropertyTranslator(source).Build(typedAccessorParameter, selectItems[i].EdmProperty);
                            if (accessorExpression == null)
                                throw new InvalidOperationException("Property " + selectItems[i].EdmProperty.Name + " not found");
                        }
                        else
                            accessorExpression = Expression.Property(typedAccessorParameter, clrProperty);
                    }
                    accessorList.Add(OePropertyAccessor.CreatePropertyAccessor(selectItems[i].EdmProperty, accessorExpression, accessorParameter));
                }

            return accessorList.ToArray();
        }
        private static IEdmEntitySet GetEntitySet(ODataPath path)
        {
            if (path.LastSegment is EntitySetSegment entitySetSegment)
                return entitySetSegment.EntitySet;

            if (path.LastSegment is NavigationPropertySegment navigationPropertySegment)
                return (IEdmEntitySet)navigationPropertySegment.NavigationSource;

            if (path.LastSegment is KeySegment keySegment)
                return (IEdmEntitySet)keySegment.NavigationSource;

            throw new InvalidOperationException("unknown segment type " + path.LastSegment.ToString());
        }
        private static OeEntryFactory[] GetNestedNavigationLinks(OeSelectItem navigationItem)
        {
            var nestedEntryFactories = new List<OeEntryFactory>(navigationItem.NavigationItems.Count);
            for (int i = 0; i < navigationItem.NavigationItems.Count; i++)
                if (!navigationItem.NavigationItems[i].SkipToken)
                    nestedEntryFactories.Add(navigationItem.NavigationItems[i].EntryFactory);
            return nestedEntryFactories.ToArray();
        }
        private static Expression SelectStructuralProperties(Expression source, OeSelectItem root)
        {
            if (!root.HasNavigationItems)
                return source;

            ParameterExpression parameter = Expression.Parameter(OeExpressionHelper.GetCollectionItemType(source.Type));
            IReadOnlyList<MemberExpression> joins = OeExpressionHelper.GetPropertyExpressions(parameter);
            var newJoins = new Expression[joins.Count];

            List<OeSelectItem> navigationItems = FlattenNavigationItems(root, true);
            for (int i = 0; i < navigationItems.Count; i++)
            {
                newJoins[i] = joins[i];
                if (navigationItems[i].HasSelectItems)
                {
                    var properties = new Expression[navigationItems[i].SelectItems.Count];
                    for (int j = 0; j < navigationItems[i].SelectItems.Count; j++)
                        if (navigationItems[i].SelectItems[j].EdmProperty is ComputeProperty computeProperty)
                            properties[j] = new ReplaceParameterVisitor(joins[i]).Visit(computeProperty.Expression);
                        else
                        {
                            PropertyInfo property = OeEdmClrHelper.GetPropertyIgnoreCase(joins[i].Type, navigationItems[i].SelectItems[j].EdmProperty);
                            properties[j] = Expression.Property(joins[i], property);
                        }
                    Expression newTupleExpression = OeExpressionHelper.CreateTupleExpression(properties);

                    if (i > 0 && navigationItems[i].EdmProperty.Type.IsNullable)
                    {
                        UnaryExpression nullConstant = Expression.Convert(OeConstantToVariableVisitor.NullConstantExpression, newTupleExpression.Type);
                        newTupleExpression = Expression.Condition(Expression.Equal(joins[i], OeConstantToVariableVisitor.NullConstantExpression), nullConstant, newTupleExpression);
                    }
                    newJoins[i] = newTupleExpression;
                }
            }

            NewExpression newSelectorBody = OeExpressionHelper.CreateTupleExpression(newJoins);
            MethodInfo selectMethodInfo = OeMethodInfoHelper.GetSelectMethodInfo(parameter.Type, newSelectorBody.Type);
            LambdaExpression newSelector = Expression.Lambda(newSelectorBody, parameter);

            //Quirk EF Core 2.1.1 bug Take/Skip must be last in expression tree
            var skipTakeExpressions = new List<MethodCallExpression>();
            while (source is MethodCallExpression callExpression && (callExpression.Method.Name == nameof(Enumerable.Skip) || callExpression.Method.Name == nameof(Enumerable.Take)))
            {
                skipTakeExpressions.Add(callExpression);
                source = callExpression.Arguments[0];
            }

            source = Expression.Call(selectMethodInfo, source, newSelector);

            for (int i = skipTakeExpressions.Count - 1; i >= 0; i--)
            {
                MethodInfo skipTakeMethodInfo = skipTakeExpressions[i].Method.GetGenericMethodDefinition().MakeGenericMethod(newSelector.ReturnType);
                source = Expression.Call(skipTakeMethodInfo, source, skipTakeExpressions[i].Arguments[1]);
            }

            return source;
        }

        public OeEntryFactory EntryFactory => _entryFactory;
    }
}
