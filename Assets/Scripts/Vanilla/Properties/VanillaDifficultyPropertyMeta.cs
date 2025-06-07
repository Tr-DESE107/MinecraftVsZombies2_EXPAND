using PVZEngine;

namespace MVZ2.Vanilla.Properties
{
    public class VanillaDifficultyPropertyMeta<T> : PropertyMeta<T>
    {
        public VanillaDifficultyPropertyMeta(string name, T defaultValue = default, params string[] obsoleteNames) : base(name, defaultValue, obsoleteNames)
        {
        }
    }
}
