using MVZ2.Managers;
using MVZ2.UI;
using MVZ2Logic;
using Newtonsoft.Json.Linq;
using PVZEngine;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.Audio;

namespace MVZ2.Audios
{
    public class MusicManager : MonoBehaviour, IMusicManager
    {
        #region 播放
        public void Play(NamespaceID id)
        {
            var meta = main.ResourceManager.GetMusicMeta(id);
            if (meta == null)
                return;
            var mainTrack = main.ResourceManager.GetMusicClip(meta.MainTrack);
            var subTrack = main.ResourceManager.GetMusicClip(meta.SubTrack);
            musicID = id;
            SetSourceClip(mainTrackSource, mainTrack);
            SetSourceClip(subTrackSource, subTrack);
            IsPaused = false;

            PlaySource(mainTrackSource);
            PlaySource(subTrackSource);
        }
        public void SetPlayingMusic(NamespaceID id)
        {
            var meta = main.ResourceManager.GetMusicMeta(id);
            if (meta == null)
                return;
            var mainTrack = main.ResourceManager.GetMusicClip(meta.MainTrack);
            var subTrack = main.ResourceManager.GetMusicClip(meta.SubTrack);
            musicID = id;
            SetSourceClip(mainTrackSource, mainTrack);
            SetSourceClip(subTrackSource, subTrack);
            IsPaused = false;
            Time = 0;
        }
        private void SetSourceClip(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.gameObject.SetActive(clip);
            UpdateTrackWeight();
        }
        private void PlaySource(AudioSource source)
        {
            if (!source.isActiveAndEnabled)
                return;
            source.Play();
        }
        #endregion

        #region 暂停、还原
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
        #endregion

        #region 停止
        public void Stop()
        {
            IsPaused = false;
            mainTrackSource.Stop();
            subTrackSource.Stop();
            musicID = null;
        }
        #endregion

        #region 当前音乐
        public NamespaceID GetCurrentMusicID()
        {
            return musicID;
        }
        public bool IsPlaying(NamespaceID id)
        {
            return musicID == id;
        }
        #endregion

        #region 渐变
        public void StartFade(float target, float duration)
        {
            volumeFader.StartFade(target, duration);
        }
        public void StopFade()
        {
            volumeFader.StopFade();
        }
        #endregion

        #region 音量
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
        #endregion

        #region 副轨权重
        public float GetTrackWeight()
        {
            return trackWeight;
        }
        public void SetTrackWeight(float weight)
        {
            trackWeight = weight;
            UpdateTrackWeight();
        }
        private void UpdateTrackWeight()
        {
            float w = subTrackSource.isActiveAndEnabled ? trackWeight : 0;
            mixer.SetFloat("MainWeight", AudioHelper.PercentageToDbA(1 - w));
            mixer.SetFloat("SubWeight", AudioHelper.PercentageToDbA(w));
        }
        #endregion

        #region 时间
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
        #endregion

        private void Awake()
        {
            volumeFader.OnValueChanged += value =>
            {
                mixer.SetFloat("FadeVolume", AudioHelper.PercentageToDbA(value));
            };
            volumeFader.SetValueWithoutNotify(1);
            SetTrackWeight(0);
        }
        private void Update()
        {
            if (subTrackSource.isActiveAndEnabled && Mathf.Abs(subTrackSource.timeSamples - mainTrackSource.timeSamples) >= 1000)
            {
                subTrackSource.timeSamples = mainTrackSource.timeSamples;
            }
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
