using System;
using System.Collections.Generic;
using MVZ2.HeldItems;
using MVZ2Logic.Artifacts;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;
using UnityEngine;

namespace MVZ2Logic.Level.Components
{
    public interface IAdviceComponent : ILevelComponent
    {
        void ShowAdvice(string context, string textKey, int priority, int timeout, params string[] args);
        void HideAdvice();
    }
    public interface IHeldItemComponent : ILevelComponent
    {
        void SetHeldItem(NamespaceID type, long id, int priority, bool noCancel = false);
        void SetHeldItem(IHeldItemData data);
        void ResetHeldItem();
        bool CancelHeldItem();
        IModelInterface GetHeldItemModelInterface();
        IHeldItemData Data { get; }
    }
    public interface ILogicComponent : ILevelComponent
    {
        void BeginLevel();
        void StopLevel();
        void SaveStateData();
        bool IsGamePaused();
        bool IsGameStarted();
        bool IsGameOver();
        bool IsGameRunning();
    }
    public interface IMusicComponent : ILevelComponent
    {
        void Play(NamespaceID id);
        void Stop();
        bool IsPlayingMusic(NamespaceID id);
        void SetPlayingMusic(NamespaceID id);
        float GetMusicVolume();
        void SetMusicVolume(float volume);
        float GetSubtrackWeight();
        void SetSubtrackWeight(float volume);
    }
    public interface ISoundComponent : ILevelComponent
    {
        void PlaySound(NamespaceID id, Vector3 position, float pitch = 1, float volume = 1);
        void PlaySound(NamespaceID id, float pitch = 1, float volume = 1);
        void StopAllLoopSounds();
        bool IsPlayingSound(NamespaceID id);
        bool IsPlayingLoopSound(NamespaceID id);
        bool HasLoopSoundEntity(NamespaceID id, long entityId);
        bool AddLoopSoundEntity(NamespaceID id, long entityId);
        bool RemoveLoopSoundEntity(NamespaceID id, long entityId);
        bool HasLoopSoundEntities(NamespaceID id);
        NamespaceID[] GetLoopSounds();
    }
    public interface ITalkComponent : ILevelComponent
    {
        bool CanStartTalk(NamespaceID id, int section);
        void StartTalk(NamespaceID id, int section, float delay = 1, Action onEnd = null);
        bool WillSkipTalk(NamespaceID id, int section);
        void SkipTalk(NamespaceID id, int section, Action onSkipped = null);
        void SimpleStartTalk(NamespaceID groupId, int section, float delay = 0, Action onSkipped = null, Action onStarted = null, Action onEnd = null)
        {
            if (!CanStartTalk(groupId, section))
            {
                onEnd?.Invoke();
                return;
            }
            if (WillSkipTalk(groupId, section))
            {
                SkipTalk(groupId, section, () =>
                {
                    onSkipped?.Invoke();
                    onEnd?.Invoke();
                });
            }
            else
            {
                StartTalk(groupId, section, delay, onEnd);
                onStarted?.Invoke();
            }
        }
    }
    public interface IBlueprintComponent : ILevelComponent
    {
        void SetConveyorMode(bool value);
        bool IsConveyorMode();
    }
    public interface IUIComponent : ILevelComponent
    {
        Vector3 ScreenToLawnPositionByY(Vector2 screenPosition, float y);
        Vector3 ScreenToLawnPositionByZ(Vector2 screenPosition, float y);
        Vector3 ScreenToLawnPositionByRelativeY(Vector2 screenPosition, float relativeY);
        void ShakeScreen(float startAmplitude, float endAmplitude, int time);
        void ShowMoney();
        void SetMoneyFade(bool fade);
        void SetEnergyActive(bool visible);
        void SetBlueprintsActive(bool visible);
        void SetPickaxeActive(bool visible);
        void SetStarshardActive(bool visible);
        void SetTriggerActive(bool visible);
        void SetHintArrowPointToBlueprint(int index);
        void SetHintArrowPointToPickaxe();
        void SetHintArrowPointToTrigger();
        void SetHintArrowPointToStarshard();
        void SetHintArrowPointToEntity(Entity entity);
        void HideHintArrow();
        void SetProgressBarToBoss(NamespaceID barStyle);
        void SetProgressBarToStage();
        void PauseGame(int level = 0);
        void ResumeGame(int level = 0);
        void SetUIAndInputDisabled(bool value);
        void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null);
        void SetAreaModelPreset(string name);
        void TriggerModelAnimator(string name);
        void SetModelAnimatorBool(string name, bool value);
        void SetModelAnimatorInt(string name, int value);
        void SetModelAnimatorFloat(string name, float value);
        void UpdateLevelName();
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
        void GetIlluminatingEntities(Entity entity, HashSet<long> results);
    }
    public interface IArtifactComponent : ILevelComponent
    {
        void SetSlotCount(int count);
        int GetSlotCount();
        void ReplaceArtifacts(ArtifactDefinition[] definitions);
        Artifact[] GetArtifacts();
        bool HasArtifact(NamespaceID artifactID);
        int GetArtifactIndex(NamespaceID artifactID);
        Artifact GetArtifactAt(int index);
    }
}
