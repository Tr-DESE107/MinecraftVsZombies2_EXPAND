using System.Collections.Generic;
using System.Linq;
using MVZ2.Managers;
using PVZEngine;
using UnityEngine;
using UnityEngine.Audio;

namespace MVZ2.Audios
{
    public class SoundManager : MonoBehaviour
    {
        public void SetGlobalVolume(float volume)
        {
            mixer.SetFloat("SoundVolume", AudioHelper.PercentageToDbA(volume));
        }
        public bool IsPlaying(NamespaceID id)
        {
            return soundSources.Any(s => s.SoundID == id);
        }
        public AudioSource Play2D(NamespaceID id, float pitch = 1)
        {
            return Play(id, Vector3.zero, pitch, 0);
        }
        public AudioSource Play(NamespaceID id, Vector3 pos, float pitch = 1, float spatialBlend = 1)
        {
            if (id == null)
                return null;
            var soundsMeta = main.ResourceManager.GetSoundMetaList(id.spacename);
            var meta = main.ResourceManager.GetSoundMeta(id);
            if (soundsMeta == null || meta == null)
                return null;
            var sample = meta.GetRandomSample();
            if (sample == null)
                return null;
            var maxCount = meta.maxCount;
            var clip = main.ResourceManager.GetSoundClip(sample.path);
            if (maxCount > 0 && soundSources.Count(s => s.SoundID == id) >= maxCount)
                return null;
            if (!clip)
                return null;
            var source = Instantiate(soundTemplate, pos, Quaternion.identity, soundSourceRoot);
            source.SoundID = id;

            var audioSource = source.AudioSource;
            audioSource.clip = clip;
            audioSource.pitch = pitch;
            audioSource.spatialBlend = spatialBlend;
            audioSource.priority = meta.priority;
            soundSources.Add(source);
            audioSource.gameObject.SetActive(true);
            return audioSource;
        }
        private void Update()
        {
            foreach (var source in soundSources.ToArray())
            {
                if (!source.AudioSource.isPlaying)
                {
                    soundSources.Remove(source);
                    Destroy(source.gameObject);
                }
            }
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private AudioMixer mixer;
        [SerializeField]
        private Transform soundSourceRoot;
        [SerializeField]
        private SoundSource soundTemplate;

        [Header("AudioClips")]
        private List<SoundSource> soundSources = new List<SoundSource>();
    }
}
