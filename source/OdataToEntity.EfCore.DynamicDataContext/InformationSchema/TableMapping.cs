﻿using System;
using System.Collections.Generic;

namespace OdataToEntity.EfCore.DynamicDataContext.InformationSchema
{
    public sealed class InformationSchemaMapping
    {
        public IReadOnlyList<OperationMapping>? Operations { get; set; }
        public IReadOnlyList<TableMapping>? Tables { get; set; }
    }

    public sealed class TableMapping
    {
        private readonly String? _dbName;

        public TableMapping()
        {
        }
        public TableMapping(String dbName) : this(dbName, null)
        {
        }
        public TableMapping(String dbName, String? edmName)
        {
            _dbName = dbName;
            EdmName = edmName;
        }

        public String DbName => _dbName ?? throw new InvalidOperationException(nameof(DbName) + " must not null");
        public String? EdmName { get; set; }
        public bool Exclude { get; set; }
        public IReadOnlyList<NavigationMapping>? Navigations { get; set; }
    }

    public sealed class NavigationMapping
    {
        public NavigationMapping()
        {
        }
        public NavigationMapping(String targetTableName, String navigationName)
        {
            TargetTableName = targetTableName;
            NavigationName = navigationName;
        }

        public String? ConstraintName { get; set; }
        public String? NavigationName { get; set; }
        public String? ManyToManyTarget { get; set; }
        public String? TargetTableName { get; set; }
    }

    public sealed class OperationMapping
    {
        private readonly String? _dbName;

        public OperationMapping()
        {
        }
        public OperationMapping(String dbName, String resultTableDbName)
        {
            _dbName = dbName;
            ResultTableDbName = resultTableDbName;
        }

        public String DbName => _dbName ?? throw new InvalidOperationException(nameof(DbName) + " must not null");
        public bool Exclude { get; set; }
        public String? ResultTableDbName { get; set; }
    }
}
