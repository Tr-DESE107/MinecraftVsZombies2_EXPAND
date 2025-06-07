﻿using PVZEngine;

namespace MVZ2.Vanilla.Properties
{
    public class VanillaLevelPropertyMeta<T> : PropertyMeta<T>
    {
        public VanillaLevelPropertyMeta(string name, T defaultValue = default, params string[] obsoleteNames) : base(name, defaultValue, obsoleteNames)
        {
        }
    }
}
