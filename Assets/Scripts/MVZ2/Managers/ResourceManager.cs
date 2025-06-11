﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
using MVZ2Logic.Spawns;
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
        public void ClearResources()
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
        public async Task Init()
        {
            Init_Sprites();
            ClearResources();
            foreach (var mod in main.ModManager.GetAllModInfos())
            {
                var nsp = mod.Namespace;
                var modResource = new ModResource(nsp);
                modResources.Add(modResource);
                await LoadModResourcesInit(nsp, modResource);
            }
        }
        public async Task LoadAllModResourcesMain(TaskProgress progress)
        {
            var infos = main.ModManager.GetAllModInfos();

            var childProgresses = progress.AddChildren(infos.Length);
            for (int i = 0; i < infos.Length; i++)
            {
                var mod = infos[i];
                var nsp = mod.Namespace;
                var modResource = GetModResource(nsp);
                if (modResource == null)
                {
                    Debug.LogWarning($"Cannot find the mod resource with namespace {nsp}.");
                    continue;
                }
                progress.SetCurrentTaskName($"Mod {nsp}");
                await LoadModResourcesMain(nsp, modResource, childProgresses[i]);
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
        private void OnApplicationQuit()
        {
            if (generatedSpriteManifest && Application.isEditor)
            {
                generatedSpriteManifest.Reset();
            }
        }
        private async Task LoadModResourcesInit(string modNamespace, ModResource modResource)
        {
            await LoadMetaLists(modNamespace);
            await LoadInitModMusicClips(modNamespace);
            await LoadInitModSoundClips(modNamespace);
            await LoadInitSpriteManifests(modNamespace);

            foreach (var meta in modResource.ArmorMetaList.slots)
            {
                armorSlotsCacheDict.Add(new NamespaceID(modNamespace, meta.Name), meta);
            }
            foreach (var meta in modResource.ArmorMetaList.metas)
            {
                armorsCacheDict.Add(new NamespaceID(modNamespace, meta.ID), meta);
            }
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
                difficultyCache.Add(new NamespaceID(modNamespace, meta.ID));
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
            foreach (var meta in modResource.ArcadeMetaList.metas)
            {
                arcadeCache.Add(new NamespaceID(modNamespace, meta.ID));
            }
        }
        private async Task LoadModResourcesMain(string modNamespace, ModResource modResource, TaskProgress progress)
        {
            List<PipelineTask> tasks = new List<PipelineTask>();

            tasks.Add(new PipelineTask("Music Clips", LoadMusicClips, progress.AddChild()));
            tasks.Add(new PipelineTask("Sound Clips", LoadSoundClips, progress.AddChild()));
            tasks.Add(new PipelineTask("Sprites", LoadSprites, progress.AddChild()));
            tasks.Add(new PipelineTask("Models", LoadModels, progress.AddChild()));
            tasks.Add(new PipelineTask("Map Models", LoadMapModels, progress.AddChild()));
            tasks.Add(new PipelineTask("Area Models", LoadAreaModels, progress.AddChild()));

            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                var startTime = Time.time;
                var name = task.GetName();
                progress.SetCurrentTaskName(name);
                await task.Run();
                Debug.Log($"加载{name}花费的时间：{Time.time - startTime}");
            }

            Task LoadMusicClips(TaskProgress progress)
            {
                progress.SetCurrentTaskName("Waiting");
                return LoadMainModMusicClips(modNamespace, progress);
            }
            Task LoadSoundClips(TaskProgress progress)
            {
                progress.SetCurrentTaskName("Waiting");
                return LoadMainModSoundClips(modNamespace, progress);
            }
            async Task LoadSprites(TaskProgress progress)
            {
                var loadProgress = progress.AddChild();
                var variantProgress = progress.AddChild();
                loadProgress.SetCurrentTaskName("Waiting");
                variantProgress.SetCurrentTaskName("Waiting");

                await LoadMainSpriteManifests(modNamespace, loadProgress);
                loadProgress.SetProgress(1, "Finished");

                var enumerator = LoadCharacterVariantSprites(modNamespace, variantProgress, scale: characterImageScale);
                await Main.CoroutineManager.ToTask(enumerator);
                variantProgress.SetProgress(1, "Finished");
            }
            async Task LoadModels(TaskProgress progress)
            {
                var loadProgress = progress.AddChild();
                var shotProgress = progress.AddChild();
                loadProgress.SetCurrentTaskName("Waiting");
                shotProgress.SetCurrentTaskName("Waiting");

                await LoadModModels(modNamespace, loadProgress);
                loadProgress.SetProgress(1, "Finished");

                var enumerator = ShotModelIcons(modNamespace, shotProgress, 8);
                await Main.CoroutineManager.ToTask(enumerator);
                shotProgress.SetProgress(1, "Finished");
            }
            Task LoadMapModels(TaskProgress progress)
            {
                progress.SetCurrentTaskName("Waiting");
                return LoadModMapModels(modNamespace, progress);
            }
            Task LoadAreaModels(TaskProgress progress)
            {
                progress.SetCurrentTaskName("Waiting");
                return LoadModAreaModels(modNamespace, progress);
            }
        }
        private void LoadSingleMetaList(ModResource modResource, NamespaceID resID, TextAsset resource)
        {
            const string talksDirectory = "talks/";

            using var memoryStream = new MemoryStream(resource.bytes);
            var document = memoryStream.ReadXmlDocument();
            var metaPath = resID.Path.Replace("\\", "/");
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
        private IList<IResourceLocation> GetLabeledResourceLocations<T>(string modNamespace, Addressables.MergeMode mergeMode, params string[] labels)
        {
            var locator = Main.ModManager.GetModInfo(modNamespace).ResourceLocator;
            var t = typeof(T);
            if (t.IsArray)
                t = t.GetElementType();
            else if (t.IsGenericType && typeof(IList<>) == t.GetGenericTypeDefinition())
                t = t.GetGenericArguments()[0];

            var locations = new HashSet<IResourceLocation>(new ResourceLocationEqualityComparer());
            if (mergeMode == Addressables.MergeMode.None)
                return null;
            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (!locator.Locate(label, t, out var locs))
                    continue;
                switch (mergeMode)
                {
                    case Addressables.MergeMode.UseFirst:
                        continue;
                    case Addressables.MergeMode.Union:
                        locations.UnionWith(locs);
                        break;
                    case Addressables.MergeMode.Intersection:
                        if (i > 0)
                        {
                            locations.IntersectWith(locs);
                        }
                        else
                        {
                            locations.UnionWith(locs);
                        }
                        break;
                }
            }
            return locations.ToArray();
        }
        private Task<(NamespaceID key, T resource)[]> LoadResourcesByLocations<T>(IList<IResourceLocation> locations)
        {
            return LoadResourcesByLocations<T>(locations, null);
        }
        private async Task<(NamespaceID key, T resource)[]> LoadResourcesByLocations<T>(IList<IResourceLocation> locations, TaskProgress progress)
        {
            if (locations == null)
                return Array.Empty<(NamespaceID key, T resource)>();
            List<(NamespaceID key, T resource)> loaded = new List<(NamespaceID key, T resource)>();
            int loadedCount = 0;
            int maxConcurrency = Mathf.Max(3, locations.Count / 10);
            int yieldCounter = 0;
            int maxYieldCount = maxConcurrency;

            var sem = new SemaphoreSlim(maxConcurrency);
            List<Task> tasks = new List<Task>();
            foreach (var loc in locations)
            {
                await sem.WaitAsync();  // 等可用 slot
                var handle = Addressables.LoadAssetAsync<T>(loc);
                progress?.SetCurrentTaskName($"Loading {loc.PrimaryKey}");
                handle.Completed += (handle) =>
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
                    loadedCount++;
                    progress?.SetProgress(loadedCount / (float)locations.Count);
                };

                // 当单个 handle 完成后，释放信号量
                tasks.Add(handle.Task.ContinueWith(t =>
                {
                    sem.Release();
                    if (t.IsFaulted)
                        Debug.LogError($"加载失败：{loc}: {t.Exception}");
                }, TaskScheduler.FromCurrentSynchronizationContext()));
                yieldCounter++;
                if (yieldCounter > maxYieldCount)
                {
                    yieldCounter = 0;
                    await Task.Yield();
                }
            }
            await Task.WhenAll(tasks);
            progress?.SetProgress(1, "Finished");
            return loaded.ToArray();
        }
        private Task<(NamespaceID key, T resource)[]> LoadLabeledResources<T>(string modNamespace, string label, TaskProgress progress)
        {
            var locs = GetLabeledResourceLocations<T>(modNamespace, label);
            return LoadResourcesByLocations<T>(locs, progress);
        }
        private Task<(NamespaceID key, T resource)[]> LoadLabeledResources<T>(string modNamespace, Addressables.MergeMode mergeMode, TaskProgress progress, params string[] labels)
        {
            var locs = GetLabeledResourceLocations<T>(modNamespace, mergeMode, labels);
            return LoadResourcesByLocations<T>(locs, progress);
        }
        private Task<(NamespaceID key, T resource)[]> LoadLabeledResources<T>(string modNamespace, string label)
        {
            return LoadLabeledResources<T>(modNamespace, label, null);
        }
        private Task<(NamespaceID key, T resource)[]> LoadLabeledResources<T>(string modNamespace, Addressables.MergeMode mergeMode, params string[] labels)
        {
            return LoadLabeledResources<T>(modNamespace, mergeMode, null, labels);
        }
        private async Task<T> LoadModResource<T>(NamespaceID id, ResourceType resourceType)
        {
            if (id == null)
                return default;
            return await LoadModResource<T>(id.SpaceName, id.Path, resourceType);
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
                if (mod.Namespace != id.SpaceName)
                    continue;
                var dict = dictionaryGetter(mod);
                if (dict.TryGetValue(id.Path, out var resource))
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
        IDifficultyMeta IGameMetas.GetDifficultyMeta(NamespaceID id) => GetDifficultyMeta(id);
        IAreaMeta IGameMetas.GetAreaMeta(NamespaceID id) => GetAreaMeta(id);

        IAreaMeta[] IGameMetas.GetModAreaMetas(string spaceName) => GetModAreaMetas(spaceName);

        IEntityMeta IGameMetas.GetEntityMeta(NamespaceID id) => GetEntityMeta(id);
        IEntityCounterMeta IGameMetas.GetEntityCounterMeta(NamespaceID id) => GetEntityCounterMeta(id);

        IEntityMeta[] IGameMetas.GetModEntityMetas(string spaceName) => GetModEntityMetas(spaceName);
        IArmorSlotMeta IGameMetas.GetArmorSlotMeta(NamespaceID id) => GetArmorSlotMeta(id);
        IArmorMeta IGameMetas.GetArmorMeta(NamespaceID id) => GetArmorMeta(id);

        IArmorMeta[] IGameMetas.GetModArmorMetas(string spaceName) => GetModArmorMetas(spaceName);

        IArtifactMeta[] IGameMetas.GetModArtifactMetas(string spaceName) => GetModArtifactMetas(spaceName);

        IModelMeta IGameMetas.GetModelMeta(NamespaceID id) => GetModelMeta(id);

        IModelMeta[] IGameMetas.GetModModelMetas(string spaceName) => GetModModelMetas(spaceName);
        ISeedOptionMeta IGameMetas.GetSeedOptionMeta(NamespaceID id) => GetBlueprintOptionMeta(id);

        ISeedOptionMeta[] IGameMetas.GetModSeedOptionMetas(string spaceName) => GetModBlueprintOptionMetas(spaceName);
        ISpawnMeta IGameMetas.GetSpawnMeta(NamespaceID id) => GetSpawnMeta(id);

        ISpawnMeta[] IGameMetas.GetModSpawnMetas(string spaceName) => GetModSpawnMetas(spaceName);


        IGridLayerMeta IGameMetas.GetGridLayerMeta(NamespaceID id) => GetGridLayerMeta(id);
        IGridErrorMeta IGameMetas.GetGridErrorMeta(NamespaceID id) => GetGridErrorMeta(id);
        #endregion

        #endregion
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private List<ModResource> modResources = new List<ModResource>();
    }
    public class ResourceLocationEqualityComparer : IEqualityComparer<IResourceLocation>
    {

        //needed for IEqualityComparer<IResourceLocation> interface
        public bool Equals(IResourceLocation x, IResourceLocation y)
        {
            return x.PrimaryKey.Equals(y.PrimaryKey) && x.ResourceType.Equals(y.ResourceType) && x.InternalId.Equals(y.InternalId);
        }

        //needed for IEqualityComparer<IResourceLocation> interface
        public int GetHashCode(IResourceLocation loc)
        {
            return loc.PrimaryKey.GetHashCode() * 31 + loc.ResourceType.GetHashCode();
        }
    }
}
