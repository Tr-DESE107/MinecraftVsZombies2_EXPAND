﻿using System.Collections.Generic;
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
            var soundsMeta = main.ResourceManager.GetSoundMetaList(id.SpaceName);
            var meta = main.ResourceManager.GetSoundMeta(id);
            if (soundsMeta == null || meta == null)
                return null;
            var sample = meta.GetRandomSample();
            if (sample == null)
                return null;
            var clip = main.ResourceManager.GetSoundClip(sample.path);
            if (!clip)
                return null;
            var maxCount = meta.maxCount;
            var sameSoundSources = soundSources.Where(s => s.SoundID == id);
            if (maxCount > 0 && sameSoundSources.Count() >= maxCount)
            {
                RemoveSoundSource(sameSoundSources.FirstOrDefault());
            }
            var source = Instantiate(soundTemplate, pos, Quaternion.identity, soundSourceRoot);
            source.Volume = 1;
            source.SoundID = id;
            source.gameObject.name = id.ToString();

            var audioSource = source.AudioSource;
            audioSource.clip = clip;
            audioSource.pitch = pitch;
            audioSource.spatialBlend = spatialBlend;
            audioSource.priority = meta.priority;
            soundSources.Add(source);
            audioSource.gameObject.SetActive(true);
            return audioSource;
        }
        #region 循环音效
        public bool PlayLoopSound(NamespaceID id)
        {
            if (id == null)
                return false;
            if (IsPlayingLoopSound(id))
                return false;
            var soundsMeta = main.ResourceManager.GetSoundMetaList(id.SpaceName);
            var meta = main.ResourceManager.GetSoundMeta(id);
            if (soundsMeta == null || meta == null)
                return false;
            var sample = meta.GetRandomSample();
            if (sample == null)
                return false;
            var clip = main.ResourceManager.GetSoundClip(sample.path);
            if (!clip)
                return false;
            var source = Instantiate(loopSoundTemplate, Vector3.zero, Quaternion.identity, loopSoundSourceRoot);
            source.SoundID = id;
            source.gameObject.name = id.ToString();

            var audioSource = source.AudioSource;
            audioSource.clip = clip;
            audioSource.priority = meta.priority;
            loopSoundSources.Add(id, source);
            audioSource.gameObject.SetActive(true);
            return true;
        }
        public bool StopLoopSound(NamespaceID id)
        {
            if (id == null)
                return false;
            if (!IsPlayingLoopSound(id))
                return false;

            if (!loopSoundSources.TryGetValue(id, out var source))
                return false;

            source.AudioSource.Stop();
            Destroy(source.gameObject);
            loopSoundSources.Remove(id);
            return true;
        }
        public void StartFadeLoopSound(NamespaceID id, float target, float time)
        {
            if (!loopSoundSources.TryGetValue(id, out var source))
                return;
            source.StartFade(target, time);
        }
        public void StopFadeLoopSound(NamespaceID id)
        {
            if (!loopSoundSources.TryGetValue(id, out var source))
                return;
            source.StopFade();
        }
        public bool IsPlayingLoopSound(NamespaceID id)
        {
            return loopSoundSources.ContainsKey(id);
        }
        public void SetLoopSoundPosition(NamespaceID id, Vector3 position)
        {
            if (!loopSoundSources.TryGetValue(id, out var source))
                return;
            source.transform.position = position;
        }
        public float GetLoopSoundIntensity(NamespaceID id)
        {
            if (!loopSoundSources.TryGetValue(id, out var source))
                return -1;
            return source.Intensity;
        }
        public void SetLoopSoundIntensity(NamespaceID id, float intensity)
        {
            if (!loopSoundSources.TryGetValue(id, out var source))
                return;
            source.Intensity = intensity;
            UpdateLoopSound(id, source, intensity);
        }
        #endregion
        private void Update()
        {
            foreach (var source in soundSources.ToArray())
            {
                if (!source.AudioSource.isPlaying)
                {
                    RemoveSoundSource(source);
                }
            }
        }
        private void UpdateLoopSound(NamespaceID id, SoundSource source, float intensity)
        {
            var meta = Main.ResourceManager.GetSoundMeta(id);
            if (meta == null)
            {
                source.Volume = intensity;
                source.Pitch = 1;
                return;
            }

            var pitchStart = meta.loopPitchStart;
            var pitchEnd = meta.loopPitchEnd;
            var pitch = (pitchEnd - pitchStart) * intensity + pitchStart;
            source.Volume = intensity;
            source.Pitch = pitch;
        }
        private void RemoveSoundSource(SoundSource source)
        {
            soundSources.Remove(source);
            Destroy(source.gameObject);
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private AudioMixer mixer;
        [SerializeField]
        private Transform soundSourceRoot;
        [SerializeField]
        private Transform loopSoundSourceRoot;
        [SerializeField]
        private SoundSource soundTemplate;
        [SerializeField]
        private SoundSource loopSoundTemplate;

        [Header("AudioClips")]
        private List<SoundSource> soundSources = new List<SoundSource>();
        private Dictionary<NamespaceID, SoundSource> loopSoundSources = new Dictionary<NamespaceID, SoundSource>();
    }
}
