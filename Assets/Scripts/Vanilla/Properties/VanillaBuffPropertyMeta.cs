using PVZEngine;

namespace MVZ2.Vanilla.Properties
{
    public class VanillaBuffPropertyMeta<T> : PropertyMeta<T>
    {
        public VanillaBuffPropertyMeta(string name, T defaultValue = default) : base(name, defaultValue)
        {
        }
    }
}
