using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PVZEngine;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        public SoundsMeta GetSoundsMeta(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.SoundMeta;
        }
        public SoundResource GetSoundResource(NamespaceID id)
        {
            var soundMeta = GetSoundsMeta(id.spacename);
            if (soundMeta == null)
                return null;
            return soundMeta.resources.FirstOrDefault(m => m.id == id);
        }
        public AudioClip GetAudioClip(string nsp, string path)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            if (modResource.AudioClips.TryGetValue(path, out var clip))
                return clip;
            return null;
        }
        private async Task<Dictionary<string, AudioClip>> LoadAudioClips(string nsp, IResourceLocator locator, SoundsMeta meta)
        {
            var directory = GetResourcesDirectory(nsp);
            var paths = meta.resources.SelectMany(r => r.samples).Select(s => s.path).Distinct();
            var clipDict = new Dictionary<string, AudioClip>();

            var tasks = new Dictionary<string, Task<AudioClip>>();
            foreach (var relativePath in paths)
            {
                var key = Path.Combine(meta.root, relativePath).Replace("\\", "/");
                tasks.Add(key, LoadAddressableResource<AudioClip>(locator, key));
            }
            foreach (var pair in tasks)
            {
                var relativePath = pair.Key;
                try
                {
                    var clip = await pair.Value;
                    clip.name = $"{nsp}:{relativePath}";
                    clipDict.Add(relativePath, clip);
                }
                catch (Exception e)
                {
                    Debug.LogError($"音频{nsp}:{relativePath}加载失败：{e}");
                }
            }
            return clipDict;
        }
        private async Task<SoundsMeta> LoadSoundMeta(string nsp, IResourceLocator locator)
        {
            var textAsset = await LoadAddressableResource<TextAsset>(locator, "sounds");
            using var memoryStream = new MemoryStream(textAsset.bytes);
            var document = LoadXmlDocument(memoryStream);
            return SoundsMeta.FromXmlNode(nsp, document["sounds"]);
        }
    }
}
