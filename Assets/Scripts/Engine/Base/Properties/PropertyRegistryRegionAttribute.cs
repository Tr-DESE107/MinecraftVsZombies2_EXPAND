﻿using System;

namespace PVZEngine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PropertyRegistryRegionAttribute : Attribute
    {
        public PropertyRegistryRegionAttribute(string regionName = null)
        {
            RegionName = regionName;
        }
        public string RegionName { get; private set; }
    }
}
