using System.Collections.Generic;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource Play(NamespaceID id, Vector3 pos)
        {
            var res = main.ResourceManager.GetAudioResource(id);
            if (res == null)
                return null;
            return Play(res, pos);
        }
        public AudioSource Play(AudioResource resource, Vector3 pos, float pitch = 1, float spatialBlend = 1)
        {
            if (resource == null)
                return null;
            var source = Instantiate(soundTemplate, pos, Quaternion.identity, soundSourceRoot);
            var index = UnityEngine.Random.Range(0, resource.clips.Length);
            var clip = resource.clips[index];
            source.clip = clip;
            source.pitch = pitch;
            source.spatialBlend = spatialBlend;
            source.priority = resource.priority;
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
