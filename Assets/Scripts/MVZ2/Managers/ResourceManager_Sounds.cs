using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.GameContent;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public SoundMetaList GetSoundMetaList(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.SoundMetaList;
        }
        #endregion

        #region 元数据
        public SoundMeta GetSoundMeta(NamespaceID id)
        {
            var soundMeta = GetSoundMetaList(id.spacename);
            if (soundMeta == null)
                return null;
            return soundMeta.metas.FirstOrDefault(m => m.name == id.path);
        }
        #endregion

        #region 音频片段
        public AudioClip GetSoundClip(string nsp, string path)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            if (modResource.Sounds.TryGetValue(path, out var res))
                return res;
            return null;
        }
        public AudioClip GetSoundClip(NamespaceID id)
        {
            if (id == null)
                return null;
            return GetSoundClip(id.spacename, id.path);
        }
        #endregion

        #region 私有方法
        private async Task<SoundMetaList> LoadSoundMetaList(string nsp)
        {
            var textAsset = await LoadModResource<TextAsset>(nsp, "sounds", ResourceType.Meta);
            using var memoryStream = new MemoryStream(textAsset.bytes);
            var document = LoadXmlDocument(memoryStream);
            return SoundMetaList.FromXmlNode(document["sounds"]);
        }
        private async Task LoadModSoundClips(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<AudioClip>(nsp, "Sound");
            foreach (var (path, res) in resources)
            {
                modResource.Sounds.Add(path, res);
            }
        }
        #endregion
    }
}
