using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.IO;
using MVZ2.Metas;
using MVZ2.Modding;
using MVZ2.TalkData;
using MVZ2Logic.Entities;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour, IGameMetas
    {
        #region 公有方法

        #region Mod资源
        public async Task LoadAllModResources()
        {
            ClearResources();
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

        #endregion

        #region 私有方法
        private void ClearResources()
        {
            modResources.Clear();
            spriteReferenceCacheDict.Clear();
            entitiesCacheDict.Clear();
            talksCacheDict.Clear();
            achievementCacheDict.Clear();
            mainmenuViewCacheDict.Clear();
            artifactsCacheDict.Clear();
            ClearResources_Store();
            difficultyCache.Clear();
            noteCache.Clear();
        }
        private void OnApplicationQuit()
        {
            if (generatedSpriteManifest && Application.isEditor)
            {
                generatedSpriteManifest.Reset();
            }
        }
        private async Task<ModResource> LoadModResources(ModInfo mod)
        {
            var modNamespace = mod.Namespace;
            var modResource = new ModResource(modNamespace);
            modResources.Add(modResource);

            await LoadMetaLists(modNamespace);
            await LoadModMusicClips(modNamespace);
            await LoadModSoundClips(modNamespace);
            await LoadModModels(modNamespace);
            await LoadModMapModels(modNamespace);
            await LoadModAreaModels(modNamespace);
            await LoadSpriteManifests(modNamespace);
            LoadCharacterVariantSprites(modNamespace);

            ShotModelIcons(modNamespace, modNamespace, modResource.ModelMetaList);

            foreach (var meta in modResource.EntityMetaList.metas)
            {
                entitiesCacheDict.Add(new NamespaceID(modNamespace, meta.ID), meta);
            }
            foreach (var meta in modResource.AchievementMetaList.metas)
            {
                achievementCacheDict.Add(new NamespaceID(modNamespace, meta.ID), meta);
            }
            foreach (var meta in modResource.MainmenuViewMetaList.Metas) 
            {
                mainmenuViewCacheDict.Add(new NamespaceID(modNamespace, meta.ID), meta);
            }
            foreach (var meta in modResource.ArtifactMetaList.metas)
            {
                artifactsCacheDict.Add(new NamespaceID(modNamespace, meta.ID), meta);
            }
            PostLoadMod_Store(modNamespace, modResource);
            foreach (var meta in modResource.DifficultyMetaList.metas)
            {
                difficultyCache.Add(new NamespaceID(modNamespace, meta.id));
            }
            foreach (var meta in modResource.NoteMetaList.metas)
            {
                noteCache.Add(new NamespaceID(modNamespace, meta.id));
            }
            foreach (var pair in modResource.TalkMetas)
            {
                foreach (var group in pair.Value.groups)
                {
                    talksCacheDict.Add(new NamespaceID(modNamespace, group.id), group);
                }
            }

            return modResource;
        }
        private void LoadSingleMetaList(ModResource modResource, NamespaceID resID, TextAsset resource)
        {
            const string talksDirectory = "talks/";

            using var memoryStream = new MemoryStream(resource.bytes);
            var document = memoryStream.ReadXmlDocument();
            var metaPath = resID.path.Replace("\\", "/");
            var defaultNsp = main.BuiltinNamespace;
            if (metaPath.StartsWith(talksDirectory))
            {
                var talkRelativePath = metaPath.Substring(talksDirectory.Length);
                var meta = TalkMeta.FromXmlDocument(document, defaultNsp);
                modResource.TalkMetas.Add(talkRelativePath, meta);
            }
            else
            {
                modResource.LoadMetaList(metaPath, document, defaultNsp);
            }
        }
        private async Task LoadMetaLists(string modNamespace)
        {
            var modResource = GetModResource(modNamespace);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<TextAsset>(modNamespace, "Meta");
            foreach (var (resID, resource) in resources)
            {
                LoadSingleMetaList(modResource, resID, resource);
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
                    var resID = NamespaceID.Parse(loc.PrimaryKey, Main.BuiltinNamespace);
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
        private T FindInMods<T>(NamespaceID id, Func<ModResource, Dictionary<string, T>> dictionaryGetter)
        {
            if (id == null)
                return default;
            foreach (var mod in modResources)
            {
                if (mod.Namespace != id.spacename)
                    continue;
                var dict = dictionaryGetter(mod);
                if (dict.TryGetValue(id.path, out var resource))
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
        #region 接口实现
        IStageMeta IGameMetas.GetStageMeta(NamespaceID id) => GetStageMeta(id);

        IStageMeta[] IGameMetas.GetModStageMetas(string spaceName) => GetModStageMetas(spaceName);
        IAreaMeta IGameMetas.GetAreaMeta(NamespaceID id) => GetAreaMeta(id);

        IAreaMeta[] IGameMetas.GetModAreaMetas(string spaceName) => GetModAreaMetas(spaceName);

        IEntityMeta IGameMetas.GetEntityMeta(NamespaceID id) => GetEntityMeta(id);

        IEntityMeta[] IGameMetas.GetModEntityMetas(string spaceName) => GetModEntityMetas(spaceName);

        IArtifactMeta[] IGameMetas.GetModArtifactMetas(string spaceName) => GetModArtifactMetas(spaceName);

        IModelMeta IGameMetas.GetModelMeta(NamespaceID id) => GetModelMeta(id);

        IModelMeta[] IGameMetas.GetModModelMetas(string spaceName) => GetModModelMetas(spaceName);
        ISeedOptionMeta IGameMetas.GetSeedOptionMeta(NamespaceID id) => GetBlueprintOptionMeta(id);

        ISeedOptionMeta[] IGameMetas.GetModSeedOptionMetas(string spaceName) => GetModBlueprintOptionMetas(spaceName);
        #endregion

        #endregion
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private List<ModResource> modResources = new List<ModResource>();
    }
}
