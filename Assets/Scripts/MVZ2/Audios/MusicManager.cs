using MVZ2.Managers;
using MVZ2.UI;
using MVZ2Logic;
using PVZEngine;
using UnityEngine;
using UnityEngine.Audio;

namespace MVZ2.Audios
{
    public class MusicManager : MonoBehaviour, IMusicManager
    {
        public void Play(NamespaceID id)
        {
            var meta = main.ResourceManager.GetMusicMeta(id);
            if (meta == null)
                return;
            var clip = main.ResourceManager.GetMusicClip(meta.Path);
            Play(clip);
            musicID = id;
        }
        public void SetPlayingMusic(NamespaceID id)
        {
            var meta = main.ResourceManager.GetMusicMeta(id);
            if (meta == null)
                return;
            var clip = main.ResourceManager.GetMusicClip(meta.Path);
            SetPlayingMusic(clip);
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
            {
                musicSource.Play();
                return;
            }
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
        public void StopFade()
        {
            volumeFader.StopFade();
        }
        public void SetVolume(float volume)
        {
            volumeFader.Value = volume;
        }
        public float GetVolume()
        {
            return volumeFader.Value;
        }
        public void SetGlobalVolume(float volume)
        {
            mixer.SetFloat("MusicVolume", AudioHelper.PercentageToDbA(volume));
        }
        public void SetNormalizedMusicTime(float time)
        {
            if (!musicSource || !musicSource.clip)
                return;
            musicSource.time = time * musicSource.clip.length;
        }
        public float GetNormalizedMusicTime()
        {
            if (!musicSource || !musicSource.clip)
                return 0;
            return musicSource.time / musicSource.clip.length;
        }
        private void Awake()
        {
            volumeFader.OnValueChanged += value => musicSource.volume = value;
            volumeFader.SetValueWithoutNotify(musicSource.volume);
        }
        private void Play(AudioClip clip)
        {
            if (!clip)
                return;
            musicSource.clip = clip;
            IsPaused = false;
            musicSource.Play();
        }
        private void SetPlayingMusic(AudioClip clip)
        {
            if (!clip)
                return;
            musicSource.clip = clip;
            IsPaused = false;
            musicSource.time = 0;
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
