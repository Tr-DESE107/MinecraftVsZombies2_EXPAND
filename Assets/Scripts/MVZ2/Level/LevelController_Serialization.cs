using System;
using System.Linq;
using System.Runtime.Serialization;
using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.Games;
using MVZ2.Level.Components;
using MVZ2.Level.UI;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Serialization;
using Tools;
using UnityEngine;

namespace MVZ2.Level
{
    using VisibleState = MVZ2.Level.UI.LevelUIPreset.VisibleState;
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法
        public SerializableLevelController SaveGame()
        {
            return new SerializableLevelController()
            {
                identifiers = Main.LevelManager.GetLevelStateIdentifierList(),

                bannerProgresses = bannerProgresses?.ToArray(),
                levelProgress = levelProgress,

                maxCryTime = maxCryTime,
                cryTimer = cryTimer,
                cryTimeCheckTimer = cryTimeCheckTimer,

                musicID = CurrentMusic,
                musicTime = MusicTime,

                blueprintsActive = BlueprintsActive,
                pickaxeActive = PickaxeActive,
                starshardActive = StarshardActive,
                triggerActive = TriggerActive,

                entities = entities.Select(e => e.ToSerializable()).ToArray(),

                level = SerializeLevel(),
                uiPreset = GetUIPreset().ToSerializable()
            };
        }
        public void LoadGame(SerializableLevelController seri, Game game, NamespaceID areaID, NamespaceID stageID)
        {
            if (!Main.LevelManager.GetLevelStateIdentifierList().Compare(seri.identifiers))
            {
                ShowLevelErrorLoadingDialog();
                return;
            }

            try
            {
                level = DeserializeLevel(seri.level, game);
                AddLevelCallbacks();
                CreateLevelModel(level.AreaID);
                level.IsRerun = Main.SaveManager.IsLevelCleared(stageID);

                bannerProgresses = seri.bannerProgresses?.ToArray();
                levelProgress = seri.levelProgress;

                maxCryTime = seri.maxCryTime;
                cryTimer = seri.cryTimer;
                cryTimeCheckTimer = seri.cryTimeCheckTimer;

                CurrentMusic = seri.musicID;
                MusicTime = seri.musicTime;

                BlueprintsActive = seri.blueprintsActive;
                PickaxeActive = seri.pickaxeActive;
                StarshardActive = seri.starshardActive;
                TriggerActive = seri.triggerActive;

                var uiPreset = GetUIPreset();
                uiPreset.LoadFromSerializable(seri.uiPreset);
                uiPreset.UpdateFrame(0);

                foreach (var entity in level.GetEntities())
                {
                    var controller = CreateControllerForEntity(entity);

                    var seriEntity = seri.entities.FirstOrDefault(e => e.id == entity.ID);
                    if (seriEntity == null)
                        throw new SerializationException($"Could not find entity data with id {entity.ID} in the level state data.");
                    controller.LoadFromSerializable(seriEntity);
                    controller.UpdateFrame(0);
                }
            }
            catch (Exception e)
            {
                ShowLevelErrorLoadingDialog(e);
                Debug.LogException(e);
                return;
            }

            level.ResetHeldItem();
            UpdateBlueprintsView();
            UpdateLevelName();
            UpdateLevelProgress();
            UpdateDifficultyName();
            UpdateLevelUI();
            SetUIVisibleState(VisibleState.InLevel);
            SetUnlockedUIVisible();
            ShowMoney();

            SetCameraPosition(LevelCameraPosition.Lawn);

            isGameStarted = true;

            foreach (var component in level.GetComponents())
            {
                if (component is MVZ2Component comp)
                {
                    comp.PostLevelLoad();
                }
            }

            Pause();
            ShowLevelLoadedDialog();
            levelLoaded = true;
        }

        #endregion

        #region 私有方法

        #region 序列化
        private void AddLevelCallbacks()
        {
            level.OnEntitySpawn += Engine_OnEntitySpawnCallback;
            level.OnEntityRemove += Engine_OnEntityRemoveCallback;
            level.OnGameOver += Engine_OnGameOverCallback;

            level.OnSeedPackChanged += Engine_OnSeedPackChangedCallback;
            level.OnSeedPackCountChanged += Engine_OnSeedPackCountChangedCallback;

            level.OnClear += Engine_OnClearCallback;

            BuiltinCallbacks.PostHugeWaveApproach.Add(PostHugeWaveApproachCallback);
            BuiltinCallbacks.PostFinalWave.Add(PostFinalWaveCallback);
        }
        private void ApplyComponents(LevelEngine level)
        {
            level.AddComponent(new AdviceComponent(level, this));
            level.AddComponent(new HeldItemComponent(level, this));
            level.AddComponent(new UIComponent(level, this));
            level.AddComponent(new LogicComponent(level, this));
            level.AddComponent(new SoundComponent(level, this));
            level.AddComponent(new TalkComponent(level, this));
            level.AddComponent(new MusicComponent(level, this));
            level.AddComponent(new MoneyComponent(level, this));
        }
        private SerializableLevel SerializeLevel()
        {
            return level.Serialize();
        }
        private LevelEngine DeserializeLevel(SerializableLevel seri, Game game)
        {
            var level = LevelEngine.Deserialize(seri, game, game);
            ApplyComponents(level);
            level.DeserializeComponents(seri);
            game.SetLevel(level);
            return level;
        }
        #endregion

        #endregion
    }
    public class SerializableLevelController
    {
        public LevelDataIdentifierList identifiers;

        public float levelProgress;
        public float[] bannerProgresses;

        public NamespaceID musicID;
        public float musicTime;

        public bool blueprintsActive;
        public bool pickaxeActive;
        public bool starshardActive;
        public bool triggerActive;

        public int maxCryTime;
        public FrameTimer cryTimer;
        public FrameTimer cryTimeCheckTimer;

        public SerializableEntityController[] entities;
        public SerializableLevel level;

        public SerializableLevelUIPreset uiPreset;
    }
}
