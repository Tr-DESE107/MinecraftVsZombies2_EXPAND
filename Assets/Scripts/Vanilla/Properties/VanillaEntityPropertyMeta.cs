using PVZEngine;

namespace MVZ2.Vanilla.Properties
{
    public class VanillaEntityPropertyMeta<T> : PropertyMeta<T>
    {
        public VanillaEntityPropertyMeta(string name, T defaultValue = default, params string[] obsoleteNames) : base(name, defaultValue, obsoleteNames)
        {
        }
    }
}
