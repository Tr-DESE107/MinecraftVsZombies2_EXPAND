using System.Collections.Generic;
using System.IO;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource Play(NamespaceID id, Vector3 pos)
        {
            var soundsMeta = main.ResourceManager.GetSoundsMeta(id.spacename);
            var res = main.ResourceManager.GetSoundResource(id);
            if (soundsMeta == null || res == null)
                return null;
            var sample = res.GetRandomSample();
            if (sample == null)
                return null;
            var path = Path.Combine(soundsMeta.root, sample.path).Replace("\\", "/");
            var clip = main.ResourceManager.GetAudioClip(id.spacename, path);
            return Play(clip, pos, res.priority);
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
        private Transform soundSourceRoot;
        [SerializeField]
        private AudioSource soundTemplate;

        [Header("AudioClips")]
        private List<AudioSource> soundSources = new List<AudioSource>();
    }
}
