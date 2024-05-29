using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
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
            var nsp = mod.Namespace;
            var modResource = new ModResource(nsp);
            modResources.Add(modResource);

            modResource.SoundMetaList = await LoadSoundMetaList(nsp);
            modResource.ModelMetaList = await LoadModelMetaList(nsp);
            modResource.FragmentsMetaList = await LoadFragmentMetaList(nsp);

            await LoadModSoundClips(nsp);
            await LoadModModels(nsp);
            ShotModelIcons(nsp, modResource.ModelMetaList);
            await LoadSpriteSheets(nsp);
            await LoadSprites(nsp);

            return modResource;
        }
        private IList<IResourceLocation> GetLabeledResourceLocations<T>(string nsp, string label)
        {
            var locator = Main.ModManager.GetModInfo(nsp).ResourceLocator;
            if (!locator.Locate(label, typeof(T), out var locs))
                return null;
            return locs;
        }
        private async Task<(string key, T resource)[]> LoadResourcesByLocations<T>(string nsp, IList<IResourceLocation> locations)
        {
            List<(string key, T resource)> loaded = new List<(string key, T resource)>();
            List<Task> tasks = new List<Task>();
            foreach (var loc in locations)
            {
                var op = Addressables.LoadAssetAsync<T>(loc);
                op.Completed += (handle) =>
                {
                    var relativePath = loc.PrimaryKey;
                    try
                    {
                        var res = handle.Result;
                        loaded.Add((relativePath, res));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"资源（{typeof(T[]).Name}）{nsp}:{relativePath}加载失败：{e}");
                    }
                };
                tasks.Add(op.Task);
            }
            await Task.WhenAll(tasks);
            return loaded.ToArray();
        }
        private async Task<(string key, T resource)[]> LoadLabeledResources<T>(string nsp, string label)
        {
            var locs = GetLabeledResourceLocations<T>(nsp, label);
            if (locs == null)
                return Array.Empty<(string key, T resource)>();
            return await LoadResourcesByLocations<T>(nsp, locs);
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
