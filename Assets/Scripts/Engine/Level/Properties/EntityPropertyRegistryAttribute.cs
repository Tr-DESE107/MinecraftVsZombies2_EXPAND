﻿using System;

namespace PVZEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EntityPropertyRegistryAttribute : PropertyRegistryAttribute
    {
        public EntityPropertyRegistryAttribute(string regionName = null) : base(regionName, PropertyRegions.entity)
        {
        }
    }
}
