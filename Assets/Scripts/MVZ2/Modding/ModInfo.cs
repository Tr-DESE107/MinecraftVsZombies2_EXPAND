using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
