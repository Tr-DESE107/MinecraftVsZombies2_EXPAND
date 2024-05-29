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
            return GetSoundClip(new NamespaceID(nsp, path));
        }
        public AudioClip GetSoundClip(NamespaceID id)
        {
            return FindInMods(id, mod => mod.Sounds);
        }
        #endregion

        #region 私有方法
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
