using PVZEngine;

namespace MVZ2.Vanilla.Properties
{
    public class VanillaArmorPropertyMeta<T> : PropertyMeta<T>
    {
        public VanillaArmorPropertyMeta(string name, T defaultValue = default, params string[] obsoleteNames) : base(name, defaultValue, obsoleteNames)
        {
        }
    }
}
