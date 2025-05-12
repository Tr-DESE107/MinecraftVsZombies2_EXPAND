using MVZ2.Managers;
using MVZ2.UI;
using MVZ2Logic;
using PVZEngine;
using UnityEditor.Hardware;
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
            var mainTrack = main.ResourceManager.GetMusicClip(meta.MainTrack);
            var subTrack = main.ResourceManager.GetMusicClip(meta.SubTrack) ?? mainTrack;
            Play(mainTrack, subTrack);
            musicID = id;
        }
        public void SetPlayingMusic(NamespaceID id)
        {
            var meta = main.ResourceManager.GetMusicMeta(id);
            if (meta == null)
                return;
            var mainTrack = main.ResourceManager.GetMusicClip(meta.MainTrack);
            var subTrack = main.ResourceManager.GetMusicClip(meta.SubTrack) ?? mainTrack;
            SetPlayingMusic(mainTrack, subTrack);
            musicID = id;
        }
        public void Pause()
        {
            if (IsPaused)
                return;
            IsPaused = true;
            mainTrackSource.Pause();
            subTrackSource.Pause();
        }
        public void Resume()
        {
            if (!IsPaused)
            {
                mainTrackSource.Play();
                subTrackSource.Play();
                return;
            }
            IsPaused = false;
            mainTrackSource.UnPause();
            subTrackSource.UnPause();
        }
        public void Stop()
        {
            IsPaused = false;
            mainTrackSource.Stop();
            subTrackSource.Stop();
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
        public float GetTrackWeight()
        {
            return trackWeight;
        }
        public void SetTrackWeight(float weight)
        {
            trackWeight = weight;
            mixer.SetFloat("MainWeight", AudioHelper.PercentageToDbA(1 - trackWeight));
            mixer.SetFloat("SubWeight", AudioHelper.PercentageToDbA(trackWeight));
        }
        public void SetNormalizedMusicTime(float time)
        {
            if (!mainTrackSource || !mainTrackSource.clip)
                return;
            Time = time * mainTrackSource.clip.length;
        }
        public float GetNormalizedMusicTime()
        {
            if (!mainTrackSource || !mainTrackSource.clip)
                return 0;
            return Time / mainTrackSource.clip.length;
        }
        private void Awake()
        {
            volumeFader.OnValueChanged += value =>
            {
                mixer.SetFloat("FadeVolume", AudioHelper.PercentageToDbA(value));
            };
            volumeFader.SetValueWithoutNotify(1);
            SetTrackWeight(0);
        }
        private void Play(AudioClip main, AudioClip sub)
        {
            SetClips(main, sub);
            mainTrackSource.Play();
            subTrackSource.Play();
        }
        private void SetPlayingMusic(AudioClip main, AudioClip sub)
        {
            SetClips(main, sub);
            Time = 0;
        }
        private void SetClips(AudioClip mainTrack, AudioClip subTrack)
        {
            mainTrackSource.clip = mainTrack;
            subTrackSource.clip = subTrack;
            IsPaused = false;
        }
        public MainManager Main => main;
        public float Time
        {
            get => mainTrackSource.time;
            set
            {
                mainTrackSource.time = value;
                subTrackSource.time = value;
            }
        }
        public bool IsPaused { get; private set; }
        private NamespaceID musicID;
        private float trackWeight;
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private AudioMixer mixer;
        [SerializeField]
        private AudioSource mainTrackSource;
        [SerializeField]
        private AudioSource subTrackSource;
        [SerializeField]
        private FloatFader volumeFader;
    }
}
