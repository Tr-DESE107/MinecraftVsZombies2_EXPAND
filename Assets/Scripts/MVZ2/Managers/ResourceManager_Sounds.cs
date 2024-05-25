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
            return soundMeta.resources.FirstOrDefault(m => m.name == id.name);
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
        private Task<Dictionary<string, AudioClip>> LoadAudioClips(string nsp, IResourceLocator locator, SoundsMeta meta)
        {
            var paths = meta.resources.SelectMany(r => r.samples).Select(s => s.path).Distinct();
            return LoadResourceGroup<AudioClip>(nsp, locator, meta.root, paths.ToArray());
        }
        private async Task<SoundsMeta> LoadSoundMeta(IResourceLocator locator)
        {
            var textAsset = await LoadAddressableResource<TextAsset>(locator, "sounds");
            using var memoryStream = new MemoryStream(textAsset.bytes);
            var document = LoadXmlDocument(memoryStream);
            return SoundsMeta.FromXmlNode(document["sounds"]);
        }
    }
}
