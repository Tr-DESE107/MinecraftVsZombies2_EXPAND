﻿using System;

namespace PVZEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PropertyRegistryAttribute : Attribute
    {
        public PropertyRegistryAttribute(string regionName = null, string typeName = null)
        {
            TypeName = typeName;
            RegionName = regionName;
        }
        public string TypeName { get; private set; }
        public string RegionName { get; private set; }
    }
}
