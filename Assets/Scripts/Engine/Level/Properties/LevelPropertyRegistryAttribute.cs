﻿using System;

namespace PVZEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LevelPropertyRegistryAttribute : PropertyRegistryAttribute
    {
        public LevelPropertyRegistryAttribute(string regionName = null) : base(regionName, PropertyRegions.level)
        {
        }
    }
}
