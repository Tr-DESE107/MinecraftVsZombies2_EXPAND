using System;
using System.Linq;
using System.Runtime.Serialization;
using MVZ2.Entities;
using MVZ2.Games;
using MVZ2.Level.Components;
using MVZ2.Level.UI;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Saves;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Level;
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
                identifiers = LevelManager.GetLevelStateIdentifierList(),

                bannerProgresses = bannerProgresses?.ToArray(),
                levelProgress = levelProgress,
                bossProgress = bossProgress,
                progressBarMode = progressBarMode,
                bossProgressBarStyle = bossProgressBarStyle,

                maxCryTime = maxCryTime,
                cryTimer = cryTimer,
                cryTimeCheckTimer = cryTimeCheckTimer,

                musicID = CurrentMusic,
                musicTime = MusicTime,
                musicVolume = MusicVolume,

                energyActive = EnergyActive,
                blueprintsActive = BlueprintsActive,
                pickaxeActive = PickaxeActive,
                starshardActive = StarshardActive,
                triggerActive = TriggerActive,

                isConveyorMode = isConveyorMode,
                conveyorSeedPositions = conveyorSeedPositions.ToArray(),

                entities = entities.Select(e => e.ToSerializable()).ToArray(),

                level = SerializeLevel(),
                uiPreset = GetUIPreset().ToSerializable()
            };
        }
        public void LoadGame(SerializableLevelController seri, Game game, NamespaceID areaID, NamespaceID stageID)
        {
            if (!LevelManager.GetLevelStateIdentifierList().Compare(seri.identifiers))
            {
                ShowLevelErrorLoadingDialog();
                return;
            }

            try
            {
                level = LevelEngine.Deserialize(seri.level, game, game, game);
                InitLevelEngine(level, game, areaID, stageID);

                level.DeserializeComponents(seri.level);
                bannerProgresses = seri.bannerProgresses?.ToArray();
                levelProgress = seri.levelProgress;
                bossProgress = seri.bossProgress;
                progressBarMode = seri.progressBarMode;
                bossProgressBarStyle = seri.bossProgressBarStyle;

                maxCryTime = seri.maxCryTime;
                cryTimer = seri.cryTimer;
                cryTimeCheckTimer = seri.cryTimeCheckTimer;

                CurrentMusic = seri.musicID;
                MusicTime = seri.musicTime;
                MusicVolume = seri.musicVolume;

                EnergyActive = seri.energyActive;
                BlueprintsActive = seri.blueprintsActive;
                PickaxeActive = seri.pickaxeActive;
                StarshardActive = seri.starshardActive;
                TriggerActive = seri.triggerActive;

                SetConveyorMode(seri.isConveyorMode);
                conveyorSeedPositions = seri.conveyorSeedPositions.ToList();

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
            // 设置UI可见状态
            SetUIVisibleState(VisibleState.InLevel);
            // 相机位置
            SetCameraPosition(LevelCameraPosition.Lawn);
            // 手持物品
            level.ResetHeldItem();
            // 经典蓝图和传送带蓝图
            InitBlueprints();
            UpdateClassicBlueprintCount();
            UpdateConveyorBlueprintCount();
            // 关卡名
            UpdateLevelName();
            // 关卡进度条
            RefreshProgressBar();
            // 难度名称
            UpdateDifficultyName();
            // 能量、关卡进度条、手持物品、蓝图状态、星之碎片
            UpdateInLevelUI();
            // 金钱
            UpdateMoney();
            ShowMoney();

            // 游戏开始状态
            SetGameStarted(true);

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
            level.OnSeedSlotCountChanged += Engine_OnSeedPackCountChangedCallback;
            level.OnConveyorSeedAdded += Engine_OnConveyorSeedPackAddedCallback;
            level.OnConveyorSeedRemoved += Engine_OnConveyorSeedPackRemovedCallback;
            level.OnConveyorSeedSlotCountChanged += Engine_OnConveyorSeedPackCountChangedCallback;

            level.OnClear += Engine_OnClearCallback;

            level.AddTrigger(VanillaCallbacks.POST_HUGE_WAVE_APPROACH, PostHugeWaveApproachCallback);
            level.AddTrigger(VanillaCallbacks.POST_FINAL_WAVE, PostFinalWaveCallback);
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
            level.AddComponent(new LightComponent(level, this));
        }
        private SerializableLevel SerializeLevel()
        {
            return level.Serialize();
        }
        #endregion

        #endregion
    }
    public class SerializableLevelController
    {
        public LevelDataIdentifierList identifiers;

        public float levelProgress;
        public float[] bannerProgresses;
        public float bossProgress;
        public NamespaceID bossProgressBarStyle;
        public bool progressBarMode;

        public NamespaceID musicID;
        public float musicTime;
        public float musicVolume;

        public bool energyActive;
        public bool blueprintsActive;
        public bool pickaxeActive;
        public bool starshardActive;
        public bool triggerActive;

        public bool isConveyorMode;
        public float[] conveyorSeedPositions;

        public int maxCryTime;
        public FrameTimer cryTimer;
        public FrameTimer cryTimeCheckTimer;

        public SerializableEntityController[] entities;
        public SerializableLevel level;

        public SerializableLevelUIPreset uiPreset;
    }
}
