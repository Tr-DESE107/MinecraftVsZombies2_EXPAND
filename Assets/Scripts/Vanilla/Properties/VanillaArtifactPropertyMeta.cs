using PVZEngine;

namespace MVZ2.Vanilla.Properties
{
    public class VanillaArtifactPropertyMeta<T> : PropertyMeta<T>
    {
        public VanillaArtifactPropertyMeta(string name, T defaultValue = default, params string[] obsoleteNames) : base(name, defaultValue, obsoleteNames)
        {
        }
    }
}
