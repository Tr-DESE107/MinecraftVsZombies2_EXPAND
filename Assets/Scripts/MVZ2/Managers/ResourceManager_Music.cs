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

        #region 音频片段
        public AudioClip GetMusicClip(string nsp, string path)
        {
            return GetMusicClip(new NamespaceID(nsp, path));
        }
        public AudioClip GetMusicClip(NamespaceID id)
        {
            return FindInMods(id, mod => mod.Musics);
        }
        #endregion

        #region 私有方法
        private async Task LoadModMusicClips(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<AudioClip>(nsp, "Music");
            foreach (var (path, res) in resources)
            {
                modResource.Musics.Add(path, res);
            }
        }
        #endregion
    }
}
