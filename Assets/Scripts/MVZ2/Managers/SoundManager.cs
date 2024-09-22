using System.Collections.Generic;
using PVZEngine;
using UnityEngine;
using UnityEngine.Audio;

namespace MVZ2
{
    public class SoundManager : MonoBehaviour
    {
        public void SetGlobalVolume(float volume)
        {
            mixer.SetFloat("SoundVolume", AudioHelper.PercentageToDbA(volume));
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
            var res = main.ResourceManager.GetSoundMeta(id);
            if (soundsMeta == null || res == null)
                return null;
            var sample = res.GetRandomSample();
            if (sample == null)
                return null;
            var clip = main.ResourceManager.GetSoundClip(sample.path);
            return Play(clip, pos, res.priority, pitch, spatialBlend);
        }
        public AudioSource Play(AudioClip clip, Vector3 pos, int priority, float pitch = 1, float spatialBlend = 1)
        {
            if (!clip)
                return null;
            var source = Instantiate(soundTemplate, pos, Quaternion.identity, soundSourceRoot);
            source.clip = clip;
            source.pitch = pitch;
            source.spatialBlend = spatialBlend;
            source.priority = priority;
            soundSources.Add(source);
            source.gameObject.SetActive(true);
            return source;
        }
        private void Update()
        {
            foreach (var source in soundSources.ToArray())
            {
                if (!source.isPlaying)
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
        private AudioSource soundTemplate;

        [Header("AudioClips")]
        private List<AudioSource> soundSources = new List<AudioSource>();
    }
}
