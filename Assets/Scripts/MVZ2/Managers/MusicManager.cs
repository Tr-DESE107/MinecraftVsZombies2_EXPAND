using System.Collections.Generic;
using System.IO;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class MusicManager : MonoBehaviour
    {
        public void Play(NamespaceID id)
        {
            var clip = main.ResourceManager.GetMusicClip(id);
            Play(clip);
        }
        public void Pause()
        {
            if (IsPaused)
                return;
            IsPaused = true;
            musicSource.Pause();
        }
        public void Resume()
        {
            if (!IsPaused)
                return;
            IsPaused = false;
            musicSource.UnPause();
        }
        public void Play(AudioClip clip)
        {
            if (!clip)
                return;
            musicSource.clip = clip;
            IsPaused = false;
            musicSource.Play();
        }
        public MainManager Main => main;
        public float Time 
        {
            get => musicSource.time;
            set => musicSource.time = value;
        }
        public bool IsPaused { get; private set; }
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private AudioSource musicSource;
    }
}
