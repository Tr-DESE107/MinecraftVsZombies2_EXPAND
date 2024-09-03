using UnityEngine.AddressableAssets.ResourceLocators;

namespace MVZ2
{
    public class ModInfo
    {
        public string Namespace { get; set; }
        public string DisplayName { get; set; }
        public bool IsBuiltin { get; set; }
        public IResourceLocator ResourceLocator { get; set; }
        public string CatalogPath { get; set; }
    }
}
