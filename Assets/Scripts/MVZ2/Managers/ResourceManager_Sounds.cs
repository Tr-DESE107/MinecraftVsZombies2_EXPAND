﻿using System.Linq;
using System.Threading.Tasks;
using MVZ2.Metas;
using PVZEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVZ2.Managers
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
            var soundMeta = GetSoundMetaList(id.SpaceName);
            if (soundMeta == null)
                return null;
            return soundMeta.metas.FirstOrDefault(m => m.name == id.Path);
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
        private async Task LoadInitModSoundClips(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<AudioClip>(nsp, Addressables.MergeMode.Intersection, "Init", "Sound");
            foreach (var (id, res) in resources)
            {
                modResource.Sounds.Add(id.Path, res);
            }
        }
        private async Task LoadMainModSoundClips(string nsp, TaskProgress progress)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var resources = await LoadLabeledResources<AudioClip>(nsp, Addressables.MergeMode.Intersection, progress, "Main", "Sound");
            for (int i = 0; i < resources.Length; i++)
            {
                var (id, res) = resources[i];
                modResource.Sounds.Add(id.Path, res);
                progress?.SetCurrentTaskName($"Added {id.Path}");
            }
        }
        #endregion
    }
}
