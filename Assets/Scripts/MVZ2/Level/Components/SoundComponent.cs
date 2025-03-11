using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Vanilla;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVZ2.Level.Components
{
    public partial class SoundComponent : MVZ2Component, ISoundComponent
    {
        public SoundComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public override void UpdateFrame(float deltaTime, float simulationSpeed)
        {
            base.UpdateFrame(deltaTime, simulationSpeed);
            loopSoundBuffer.Clear();
            loopSoundBuffer.AddRange(loopSounds.Keys);
            foreach (var id in loopSoundBuffer)
            {
                UpdateLoopSoundEntities(id);
            }
            UpdatePlayingLoopSounds(deltaTime);
        }
        public void PlaySound(NamespaceID id, Vector3 position, float pitch = 1, float volume = 1)
        {
            var source = Main.SoundManager.Play(id, Controller.LawnToTrans(position), pitch, 1);
            if (!source)
                return;
            source.volume = volume;
        }
        public void PlaySound(NamespaceID id, float pitch = 1, float volume = 1)
        {
            var source = Main.SoundManager.Play(id, Vector3.zero, pitch, 0);
            if (!source)
                return;
            source.volume = volume;
        }
        public bool IsPlayingSound(NamespaceID id)
        {
            return Main.SoundManager.IsPlaying(id);
        }
        #region —≠ª∑“Ù–ß
        public bool IsPlayingLoopSound(NamespaceID id)
        {
            return playingLoopSounds.Contains(id);
        }
        public void StopAllLoopSounds()
        {
            foreach (var sound in playingLoopSounds)
            {
                Main.SoundManager.StopLoopSound(sound);
            }
            loopSounds.Clear();
            playingLoopSounds.Clear();
        }
        public bool HasLoopSoundEntity(NamespaceID id, long entityId)
        {
            if (!loopSounds.TryGetValue(id, out var hashSet))
            {
                return false;
            }
            return hashSet.Contains(entityId);
        }
        public bool AddLoopSoundEntity(NamespaceID id, long entityId)
        {
            if (!loopSounds.TryGetValue(id, out var hashSet))
            {
                hashSet = new HashSet<long>();
                loopSounds.Add(id, hashSet);
            }
            return hashSet.Add(entityId);
        }
        public bool RemoveLoopSoundEntity(NamespaceID id, long entityId)
        {
            if (!loopSounds.TryGetValue(id, out var hashSet))
                return false;
            if (hashSet.Remove(entityId))
            {
                if (hashSet.Count <= 0)
                    loopSounds.Remove(id);

                return true;
            }
            return false;
        }
        public bool HasLoopSoundEntities(NamespaceID id)
        {
            if (!loopSounds.TryGetValue(id, out var hashSet))
                return false;
            return hashSet.Count > 0;
        }
        public NamespaceID[] GetLoopSounds()
        {
            return loopSounds.Keys.ToArray();
        }
        private void PlayLoopSound(NamespaceID id)
        {
            Main.SoundManager.PlayLoopSound(id);
            playingLoopSounds.Add(id);
        }
        private void StopLoopSound(NamespaceID id)
        {
            Main.SoundManager.StopLoopSound(id);
            playingLoopSounds.Remove(id);
        }
        private void SetLoopSoundPosition(NamespaceID id, Vector3 position)
        {
            var pos = Controller.LawnToTrans(position);
            Main.SoundManager.SetLoopSoundPosition(id, pos);
        }
        private float GetLoopSoundVolume(NamespaceID id)
        {
            return Main.SoundManager.GetLoopSoundVolume(id);
        }
        private void SetLoopSoundVolume(NamespaceID id, float volume)
        {
            Main.SoundManager.SetLoopSoundVolume(id, volume);
        }
        private void UpdateLoopSoundEntities(NamespaceID id)
        {
            var entities = loopSounds[id];
            entities.RemoveWhere(id =>
            {
                var ent = Level.FindEntityByID(id);
                return ent == null || !ent.Exists();
            });
            if (entities.Count <= 0)
            {
                loopSounds.Remove(id);
                return;
            }
            if (!Controller.IsGameRunning())
            {
                return;
            }
            var entityID = entities.FirstOrDefault();
            var entity = Level.FindEntityByID(entityID);
            if (!IsPlayingLoopSound(id))
            {
                PlayLoopSound(id);
            }
            SetLoopSoundPosition(id, entity.Position);
        }
        private void UpdatePlayingLoopSounds(float deltaTime)
        {
            for (int i = playingLoopSounds.Count - 1; i >= 0; i--)
            {
                var soundID = playingLoopSounds[i];
                if (HasLoopSoundEntities(soundID) && Controller.IsGameRunning())
                    continue;
                var volume = GetLoopSoundVolume(soundID);
                volume -= deltaTime;
                SetLoopSoundVolume(soundID, volume);
                if (volume <= 0)
                {
                    StopLoopSound(soundID);
                }
            }
        }
        #endregion
        public override ISerializableLevelComponent ToSerializable()
        {
            return new SerializableSoundComponent()
            {
                loopSounds = loopSounds.Select(p => new SerializableLoopSoundItem(p.Key, p.Value.ToArray())).ToArray()
            };
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
            base.LoadSerializable(seri);
            if (seri is not SerializableSoundComponent serializable)
                return;
            loopSounds = serializable.loopSounds.ToDictionary(p => p.id, p => new HashSet<long>(p.entities));
        }
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "sound");
        private Dictionary<NamespaceID, HashSet<long>> loopSounds = new Dictionary<NamespaceID, HashSet<long>>();
        private List<NamespaceID> playingLoopSounds = new List<NamespaceID>();
        private List<NamespaceID> loopSoundBuffer = new List<NamespaceID>();
    }
    public class SerializableSoundComponent : ISerializableLevelComponent
    {
        public SerializableLoopSoundItem[] loopSounds;
    }
    [Serializable]
    public class SerializableLoopSoundItem
    {
        public SerializableLoopSoundItem(NamespaceID id, long[] entities)
        {
            this.id = id;
            this.entities = entities;
        }
        public NamespaceID id;
        public long[] entities;
    }
}