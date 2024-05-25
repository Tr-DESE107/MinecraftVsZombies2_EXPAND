using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

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

            var soundMeta = await LoadSoundMeta(mod.ResourceLocator);
            modResource.SoundMeta = soundMeta;
            modResource.AudioClips = await LoadAudioClips(nsp, mod.ResourceLocator, soundMeta);

            var modelMeta = await LoadModelMeta(mod.ResourceLocator);
            modResource.ModelMeta = modelMeta;
            modResource.Models = await LoadModels(nsp, mod.ResourceLocator, modelMeta);

            modResource.ModelIcons = ShotModelIcons(nsp, modelMeta, modResource.Models);

            modResource.SpriteSheets = await LoadSpriteSheets(nsp, mod.ResourceLocator);
            modResource.Sprites = await LoadSprites(nsp, mod.ResourceLocator);

            modResource.FragmentsMeta = await LoadFragmentsMeta(mod.ResourceLocator);

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
        public static string CombinePath(params string[] paths)
        {
            return Path.Combine(paths).Replace("\\", "/");
        }
        private async Task<Dictionary<string, T>> LoadResourceGroup<T>(string nsp, IResourceLocator locator, string root, string[] paths)
        {
            var dict = new Dictionary<string, T>();

            var tasks = new Dictionary<string, Task<T>>();
            foreach (var relativePath in paths)
            {
                var key = CombinePath(root, relativePath);
                tasks.Add(key, LoadAddressableResource<T>(locator, key));
            }
            foreach (var pair in tasks)
            {
                var relativePath = pair.Key;
                try
                {
                    var res = await pair.Value;
                    if (res is UnityEngine.Object obj)
                    {
                        obj.name = $"{nsp}:{relativePath}";
                    }
                    dict.Add(relativePath, res);
                }
                catch (Exception e)
                {
                    Debug.LogError($"资源（{typeof(T).Name}）{nsp}:{relativePath}加载失败：{e}");
                }
            }
            return dict;
        }
        private async Task<Dictionary<string, T>> LoadLabeledResources<T>(string nsp, IResourceLocator locator, string label)
        {
            var dict = new Dictionary<string, T>();
            if (!locator.Locate(label, typeof(T), out var locs))
                return default;

            var tasks = new Dictionary<IResourceLocation, Task<T>>();
            foreach (var loc in locs)
            {
                tasks.Add(loc, Addressables.LoadAssetAsync<T>(loc).Task);
            }
            foreach (var pair in tasks)
            {
                var relativePath = pair.Key.PrimaryKey;
                try
                {
                    var res = await pair.Value;
                    dict.Add(relativePath, res);
                }
                catch (Exception e)
                {
                    Debug.LogError($"资源（{typeof(T[]).Name}）{nsp}:{relativePath}加载失败：{e}");
                }
            }
            return dict;
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
