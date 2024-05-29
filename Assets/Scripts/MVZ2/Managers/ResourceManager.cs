using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 公有方法

        #region Mod资源
        public async Task LoadAllModResources()
        {
            foreach (var mod in main.ModManager.GetAllModInfos())
            {
                await LoadModResources(mod);
            }
        }
        public ModResource GetModResource(string spaceName)
        {
            return modResources.FirstOrDefault(m => m.Namespace == spaceName);
        }
        #endregion

        #region 路径
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
        public static string CombinePath(params string[] paths)
        {
            return Path.Combine(paths).Replace("\\", "/");
        }
        #endregion

        public static NamespaceID ParseNamespaceID(string str)
        {
            return NamespaceID.Parse(str, VanillaMod.spaceName);
        }

        #endregion

        #region 私有方法
        private async Task<ModResource> LoadModResources(ModInfo mod)
        {
            var modNamespace = mod.Namespace;
            var modResource = new ModResource(modNamespace);
            modResources.Add(modResource);

            await LoadMetaLists(modNamespace);
            await LoadModSoundClips(modNamespace);
            await LoadModModels(modNamespace);
            await LoadSpriteSheets(modNamespace);
            await LoadSprites(modNamespace);

            ShotModelIcons(modNamespace, modNamespace, modResource.ModelMetaList);

            return modResource;
        }
        private async Task LoadMetaLists(string modNamespace)
        {
            var modResource = GetModResource(modNamespace);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<TextAsset>(modNamespace, "Meta");
            foreach (var (resID, resource) in resources)
            {
                using var memoryStream = new MemoryStream(resource.bytes);
                var document = memoryStream.ReadXmlDocument();
                switch (resID.path)
                {
                    case "sounds":
                        modResource.SoundMetaList = SoundMetaList.FromXmlNode(document["sounds"]);
                        break;
                    case "models":
                        modResource.ModelMetaList = ModelMetaList.FromXmlNode(document["models"]);
                        break;
                    case "fragments":
                        modResource.FragmentMetaList = FragmentMetaList.FromXmlNode(document["fragments"]);
                        break;
                }
            }
        }
        private IList<IResourceLocation> GetLabeledResourceLocations<T>(string modNamespace, string label)
        {
            var locator = Main.ModManager.GetModInfo(modNamespace).ResourceLocator;
            var t = typeof(T);
            if (t.IsArray)
                t = t.GetElementType();
            else if (t.IsGenericType && typeof(IList<>) == t.GetGenericTypeDefinition())
                t = t.GetGenericArguments()[0];
            if (!locator.Locate(label, t, out var locs))
                return null;
            return locs;
        }
        private async Task<(NamespaceID key, T resource)[]> LoadResourcesByLocations<T>(IList<IResourceLocation> locations)
        {
            List<(NamespaceID key, T resource)> loaded = new List<(NamespaceID key, T resource)>();
            List<Task> tasks = new List<Task>();
            foreach (var loc in locations)
            {
                var op = Addressables.LoadAssetAsync<T>(loc);
                op.Completed += (handle) =>
                {
                    var resID = ParseNamespaceID(loc.PrimaryKey);
                    try
                    {
                        var res = handle.Result;
                        loaded.Add((resID, res));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"资源（{typeof(T[]).Name}）{resID}加载失败：{e}");
                    }
                };
                tasks.Add(op.Task);
            }
            await Task.WhenAll(tasks);
            return loaded.ToArray();
        }
        private async Task<(NamespaceID key, T resource)[]> LoadLabeledResources<T>(string modNamespace, string label)
        {
            var locs = GetLabeledResourceLocations<T>(modNamespace, label);
            if (locs == null)
                return Array.Empty<(NamespaceID key, T resource)>();
            return await LoadResourcesByLocations<T>(locs);
        }
        private async Task<T> LoadModResource<T>(NamespaceID id, ResourceType resourceType)
        {
            if (id == null)
                return default;
            return await LoadModResource<T>(id.spacename, id.path, resourceType);
        }
        private async Task<T> LoadModResource<T>(string nsp, string path, ResourceType resourceType)
        {
            var modResource = GetModResource(nsp);
            var locator = Main.ModManager.GetModInfo(nsp).ResourceLocator;
            return await LoadAddressableResource<T>(locator, path);
        }
        private T FindInMods<T>(NamespaceID id, Func<ModResource, Dictionary<NamespaceID, T>> dictionaryGetter)
        {
            if (id == null)
                return default;
            foreach (var mod in modResources)
            {
                var dict = dictionaryGetter(mod);
                if (dict.TryGetValue(id, out var resource))
                {
                    return resource;
                }
            }
            return default;
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
        #endregion
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private List<ModResource> modResources = new List<ModResource>();
    }
}
