using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        public ModResource GetModResource(string spaceName)
        {
            return modResources.FirstOrDefault(m => m.Namespace == spaceName);
        }
        public async Task LoadAllModResources()
        {
            foreach (var mod in main.ModManager.GetAllModInfos())
            {
                modResources.Add(await LoadModResources(mod));
            }
        }
        private async Task<ModResource> LoadModResources(ModInfo mod)
        {
            var nsp = mod.Namespace;
            var modResource = new ModResource(nsp);
            var soundResources = await LoadSoundMeta(nsp, mod.ResourceLocator);
            modResource.SoundMeta = soundResources;
            modResource.AudioClips = await LoadAudioClips(nsp, mod.ResourceLocator, soundResources);
            return modResource;
        }
        public string GetNamespaceDirectory(string nsp)
        {
            if (nsp == main.BuiltinNamespace)
            {
                return Path.Combine(Application.dataPath, "GameContent");
            }
            return Path.Combine(Application.streamingAssetsPath, "Mods", nsp);
        }
        public string GetContentDirectory(string nsp)
        {
            return Path.Combine(GetNamespaceDirectory(nsp), "Content");
        }
        public string GetResourcesDirectory(string nsp)
        {
            return Path.Combine(GetNamespaceDirectory(nsp), "Res");
        }
        private async Task<T> LoadAddressableResource<T>(IResourceLocator locator, string key)
        {
            if (!locator.Locate(key, typeof(T), out var locs))
                return default;
            var loc = locs.FirstOrDefault();
            if (loc == null)
                return default;
            return await Addressables.LoadAssetAsync<T>(loc).Task;
        }
        private static XmlDocument LoadXmlDocument(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            using var xmlReader = XmlReader.Create(stream, settings);
            var document = new XmlDocument();
            document.Load(xmlReader);
            return document;
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private List<ModResource> modResources = new List<ModResource>();
    }
}
