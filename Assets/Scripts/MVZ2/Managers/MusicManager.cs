using MVZ2.Resources;
using MVZ2.UI;
using PVZEngine;
using UnityEngine;
using UnityEngine.Audio;

namespace MVZ2.Managers
{
    public class MusicManager : MonoBehaviour, IMusicManager
    {
        public void Play(NamespaceID id)
        {
            var clip = main.ResourceManager.GetMusicClip(id);
            Play(clip);
            musicID = id;
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
        public void Stop()
        {
            IsPaused = false;
            musicSource.Stop();
            musicID = null;
        }
        public NamespaceID GetCurrentMusicID()
        {
            return musicID;
        }
        public bool IsPlaying(NamespaceID id)
        {
            return musicID == id;
        }
        public void StartFade(float target, float duration)
        {
            volumeFader.StartFade(target, duration);
        }
        public void SetVolume(float volume)
        {
            musicSource.volume = volume;
        }
        public void SetGlobalVolume(float volume)
        {
            mixer.SetFloat("MusicVolume", AudioHelper.PercentageToDbA(volume));
        }
        private void Awake()
        {
            volumeFader.OnValueChanged += value => SetVolume(value);
        }
        private void Play(AudioClip clip)
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
        private NamespaceID musicID;
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private AudioMixer mixer;
        [SerializeField]
        private AudioSource musicSource;
        [SerializeField]
        private FloatFader volumeFader;
    }
}
