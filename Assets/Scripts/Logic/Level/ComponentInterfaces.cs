using System;
using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2Logic.Level.Components
{
    public interface IAdviceComponent : ILevelComponent
    {
        void ShowAdvice(string context, string textKey, int priority, int timeout);
        void HideAdvice();
    }
    public interface IHeldItemComponent : ILevelComponent
    {
        void SetHeldItem(NamespaceID type, long id, int priority, bool noCancel = false);
        void ResetHeldItem();
        bool CancelHeldItem();
        NamespaceID HeldItemType { get; }
        long HeldItemID { get; }
        int HeldItemPriority { get; }
        bool HeldItemNoCancel { get; }
    }
    public interface ILogicComponent : ILevelComponent
    {
        void BeginLevel();
        void StopLevel();
    }
    public interface IMusicComponent : ILevelComponent
    {
        void Play(NamespaceID id);
        void Stop();
    }
    public interface ISoundComponent : ILevelComponent
    {
        void PlaySound(NamespaceID id, Vector3 position, float pitch = 1);
        void PlaySound(NamespaceID id, float pitch = 1);
        void StopAllLoopSounds();
        bool IsPlayingLoopSound(NamespaceID id);
        bool AddLoopSoundEntity(NamespaceID id, long entityId);
        bool RemoveLoopSoundEntity(NamespaceID id, long entityId);
        bool HasLoopSoundEntities(NamespaceID id);
        NamespaceID[] GetLoopSounds();
    }
    public interface ITalkComponent : ILevelComponent
    {
        void StartTalk(NamespaceID id, int section, float delay = 1);
        void TryStartTalk(NamespaceID id, int section, float delay = 1, Action<bool> onFinished = null);
    }
    public interface IUIComponent : ILevelComponent
    {
        void ShakeScreen(float startAmplitude, float endAmplitude, int time);
        void ShowMoney();
        void SetBlueprintsActive(bool visible);
        void SetPickaxeActive(bool visible);
        void SetStarshardActive(bool visible);
        void SetTriggerActive(bool visible);
        void SetHintArrowPointToBlueprint(int index);
        void SetHintArrowPointToPickaxe();
        void SetHintArrowPointToStarshard();
        void SetHintArrowPointToEntity(Entity entity);
        void HideHintArrow();
    }
    public interface IMoneyComponent : ILevelComponent
    {
        void AddMoney(int value);
        int GetMoney();
        int GetDelayedMoney();
        void AddDelayedMoney(Entity entity, int value);
        bool RemoveDelayedMoney(Entity entity);
        void ClearDelayedMoney();
    }
    public interface ILightComponent : ILevelComponent
    {
        bool IsIlluminated(Entity entity);
        long GetIlluminationLightSourceID(Entity entity);
        IEnumerable<long> GetAllIlluminationLightSources(Entity entity);
    }
}
