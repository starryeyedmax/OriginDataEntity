﻿using System;
using System.Collections.Generic;

namespace OdataToEntity.Query
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class OrderByAttribute : Attribute
    {
        private bool _disable;

        private OrderByAttribute(bool disabled)
        {
            _disable = disabled;
        }
        public OrderByAttribute(params String[] properties)
        {
            OrderByConfigurations = new Dictionary<String, bool>();
            foreach (String key in properties)
                OrderByConfigurations[key] = true;
        }

        public bool Disabled
        {
            get
            {
                return _disable;
            }
            set
            {
                _disable = value;
                if (OrderByConfigurations != null)
                    foreach (String item in OrderByConfigurations.Keys)
                        OrderByConfigurations[item] = !_disable;
            }
        }

        public Dictionary<String, bool> OrderByConfigurations { get; }
    }
}
