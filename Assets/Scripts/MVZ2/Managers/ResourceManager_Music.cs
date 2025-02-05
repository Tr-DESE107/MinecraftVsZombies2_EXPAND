using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Metas;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public MusicMetaList GetMusicMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.MusicMetaList;
        }
        #endregion

        #region 元数据
        public MusicMeta GetMusicMeta(NamespaceID music)
        {
            var modResource = GetModResource(music.SpaceName);
            if (modResource == null)
                return null;
            return modResource.MusicMetaList.metas.FirstOrDefault(m => m.ID == music.Path);
        }
        public NamespaceID[] GetAllMusicID()
        {
            List<NamespaceID> list = new List<NamespaceID>();
            foreach (var modResource in modResources)
            {
                if (modResource == null)
                    continue;
                list.AddRange(modResource.MusicMetaList.metas.Select(m => new NamespaceID(modResource.Namespace, m.ID)));
            }
            return list.ToArray();
        }
        #endregion

        #region 音频片段
        public AudioClip GetMusicClip(string nsp, string path)
        {
            return GetMusicClip(new NamespaceID(nsp, path));
        }
        public AudioClip GetMusicClip(NamespaceID path)
        {
            return FindInMods(path, mod => mod.Musics);
        }
        #endregion
        public string GetMusicName(NamespaceID musicID)
        {
            if (NamespaceID.IsValid(musicID))
            {
                var meta = GetMusicMeta(musicID);
                if (meta != null)
                {
                    return main.LanguageManager._p(VanillaStrings.CONTEXT_MUSIC_NAME, meta.Name);
                }
            }
            return main.LanguageManager._p(VanillaStrings.CONTEXT_MUSIC_NAME, VanillaStrings.MUSIC_NAME_NONE);
        }

        #region 私有方法
        private async Task LoadModMusicClips(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<AudioClip>(nsp, "Music");
            foreach (var (id, res) in resources)
            {
                modResource.Musics.Add(id.Path, res);
            }
        }
        #endregion
    }
}
