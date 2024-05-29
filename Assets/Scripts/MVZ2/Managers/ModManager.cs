using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVZ2
{
    public class ModManager : MonoBehaviour
    {
        public async Task LoadMods()
        {
            var locator = await Addressables.InitializeAsync().Task;
            modInfos.Add(new ModInfo()
            {
                Namespace = main.BuiltinNamespace,
                DisplayName = "Vanilla",
                CatalogPath = null,
                IsBuiltin = true,
                ResourceLocator = locator
            });
        }
        public ModInfo GetModInfo(string nsp)
        {
            return modInfos.FirstOrDefault(m => m.Namespace == nsp);
        }
        public ModInfo[] GetAllModInfos()
        {
            return modInfos.ToArray();
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private List<ModInfo> modInfos = new List<ModInfo>();
    }
}
