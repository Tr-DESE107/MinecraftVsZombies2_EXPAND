﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Audios;
using MVZ2.Cameras;
using MVZ2.Entities;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.Games;
using MVZ2.Level.Components;
using MVZ2.Localization;
using MVZ2.Logic.Level;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Options;
using MVZ2.Saves;
using MVZ2.Scenes;
using MVZ2.Talks;
using MVZ2.UI;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    using VisibleState = MVZ2.Level.UI.LevelUIPreset.VisibleState;
    public interface ILevelController : ILevelHeldItemController, ILevelUIController, ILevelTransitionController
    {
        Game Game { get; }
        ILevelUI GetUI();
        LevelEngine GetEngine();
        Camera GetCamera();
        ILevelBlueprintController BlueprintController { get; }
        ILevelBlueprintChooseController BlueprintChoosePart { get; }

        bool CanChooseBlueprints();
        void OpenAlmanac();
        void OpenStore();

        float GetTwinkleAlpha();
    }
    public interface ILevelHeldItemController
    {
        bool IsTriggerSwapped();
    }
    public interface ILevelUIController
    {
        void ShowTooltip(ITooltipSource source);
        void HideTooltip();
    }
    public interface ILevelTransitionController
    {
        IEnumerator GameStartToLawnTransition();
        IEnumerator MoveCameraToLawn();
        IEnumerator MoveCameraToChoose();
    }
    public partial class LevelController : MonoBehaviour, ILevelController, IDisposable
    {
        #region 公有方法

        #region 游戏流程
        public void InitLevel(Game game, NamespaceID areaID, NamespaceID stageID, int seed = 0)
        {
            SetActive(true);
            rng = new RandomGenerator(Guid.NewGuid().GetHashCode());

            var collisionSystem = GetCollisionSystem();
            level = new LevelEngine(game, game, game, collisionSystem);
            InitLevelEngine(level, game, areaID, stageID);

            var option = new LevelOption()
            {
                CardSlotCount = 10,
                StarshardSlotCount = 10,
                LeftFaction = SelfFaction,
                RightFaction = EnemyFaction,
                MaxEnergy = 9990,
                TPS = 30
            };
            level.Init(areaID, stageID, option, seed);
            level.SetArtifactRNG(level.CreateRNG());
            InitGrids();
            level.Setup();

            var uiPreset = GetUIPreset();
            uiPreset.UpdateFrame(0);
            SetStarshardIcon();
            SetUnlockedUIActive();
            UpdateLighting();
        }
        public void StartLevelIntro(float delay)
        {
            _ = StartLevelIntroAsync(delay);
        }
        public void StartLevelTransition()
        {
            string transition = level.StageDefinition.GetStartTransition() ?? LevelTransitions.DEFAULT;
            if (transition == LevelTransitions.INSTANT)
            {
                GameStartInstantTransition();
            }
            else if (transition == LevelTransitions.TO_LAWN)
            {
                StartCoroutine(GameStartToLawnInstantTransition());
            }
            else
            {
                StartCoroutine(GameStartTransition());
            }
        }
        public void StartGame()
        {
            if (isGameStarted)
                return;
            level.ResetHeldItem();
            level.RemovePreviewEnemies();
            var starshardSlots = Saves.GetStarshardSlots();
            level.SetStarshardSlotCount(starshardSlots);

            // 设置蓝图。
            BlueprintChoosePart.ApplyChoose();

            Music.Play(level.GetMusicID());
            MusicTime = 0;
            MusicTrackWeight = 0;

            levelProgress = 0;
            bannerProgresses = new float[level.GetTotalFlags()];

            // 设置UI可见状态
            ui.SetBlueprintsSortingToChoosing(false);
            SetUIVisibleState(VisibleState.InLevel);
            // 可解锁UI
            SetUnlockedUIActive();
            // 关卡名
            UpdateLevelName();
            // 设置难度和名称
            UpdateDifficulty();
            // 能量、关卡进度条、手持物品、蓝图状态、星之碎片
            UpdateInLevelUI(0);
            // 金钱
            UpdateMoney();

            foreach (var part in parts)
            {
                part.PostLevelStart();
            }

            var uiPreset = GetUIPreset();
            uiPreset.SetReceiveRaycasts(true);
            uiPreset.UpdateFrame(0);

            level.Start();

            SetGameStarted(true);
            UpdateFocusLost(Application.isFocused);
        }
        public Task RestartLevel()
        {
            RemoveLevelState();
            return ReloadLevel();
        }
        public async Task ReloadLevel()
        {
            Saves.SaveModDatas();
            Dispose();
            await LevelManager.GotoLevelSceneAsync();
            LevelManager.InitLevel(StartAreaID, StartStageID, exitTarget: exitTarget);
        }
        public void GameOver(Entity killer)
        {
            killerID = killer.GetDefinitionID();
            killerEntity = GetEntityController(killer);
            SetGameOver();
            StartCoroutine(GameOverByEnemyTransition());
        }
        public void GameOver(string deathMessage)
        {
            this.deathMessage = deathMessage;
            SetGameOver();
            StartCoroutine(GameOverNoEnemyTransition());
        }
        public void GameOverInstantly(string deathMessage)
        {
            this.deathMessage = deathMessage;
            SetGameOver();
            Music.Stop();
            ShowGameOverDialog();
        }
        public void StopLevel()
        {
            level.ResetHeldItem();
            level.Triggers.RunCallback(LogicLevelCallbacks.POST_LEVEL_STOP, new LevelCallbackParams(level));
            SetUIVisibleState(VisibleState.Nothing);
            pointingGrid = -1;
            pointingGridPointerId = -1;
            level.ClearEnergyDelayedEntities();
            level.ClearDelayedMoney();
            UpdateGridHighlight();
            SetGameStarted(false);
            Saves.SaveModDatas();
        }
        public void Dispose()
        {
            if (optionsLogic != null)
            {
                optionsLogic.Dispose();
                optionsLogic = null;
            }
            Music.SetVolume(1);
            Music.SetTrackWeight(0);
            if (level != null)
            {
                foreach (var component in level.GetComponents())
                {
                    if (component is IMVZ2LevelComponent comp)
                    {
                        comp.PostDispose();
                    }
                }
                level.StopAllLoopSounds();
                level.Dispose();
            }
            Game.SetLevel(null);
            isDisposed = true;
        }
        public async Task ExitLevelToNote(NamespaceID id)
        {
            var buttonText = Localization._(Vanilla.VanillaStrings.CONTINUE);
            Sounds.Play2D(VanillaSoundID.paper);
            Scene.DisplayNote(id, buttonText);
            SetActive(false);
            await ExitScene();
            Main.GraphicsManager.ResetLighting();
        }
        public void SetExitTarget(LevelExitTarget target)
        {
            exitTarget = target;
        }
        public async Task ExitLevel()
        {
            switch (exitTarget)
            {
                case LevelExitTarget.Minigame:
                    Scene.DisplayArcade(() => Scene.DisplayMainmenu());
                    Scene.DisplayArcadeMinigames();
                    break;
                case LevelExitTarget.Puzzle:
                    Scene.DisplayArcade(() => Scene.DisplayMainmenu());
                    Scene.DisplayArcadePuzzles();
                    break;
                default:
                    Scene.GotoMapOrMainmenu();
                    break;
            }
            SetActive(false);
            await ExitScene();
            Main.GraphicsManager.ResetLighting();
        }
        public bool IsGameRunning()
        {
            return isGameStarted && !isPaused && !isGameOver;
        }
        public bool IsGamePaused()
        {
            return isPaused;
        }
        public bool IsGameStarted()
        {
            return isGameStarted;
        }
        public bool IsGameOver()
        {
            return isGameOver;
        }
        #endregion

        #region 方位
        public Vector3 LawnToTrans(Vector3 pos)
        {
            pos *= LawnToTransScale;
            Vector3 vector = new Vector3(pos.x, pos.z + pos.y, pos.z);
            vector += transform.position;
            return vector;
        }
        public Vector3 TransToLawn(Vector3 pos)
        {
            pos -= transform.position;
            Vector3 vector = new Vector3(pos.x, pos.y - pos.z, pos.z);
            vector *= TransToLawnScale;
            return vector;
        }
        #endregion

        public int GetCurrentFlag()
        {
            return level.CurrentFlag;
        }
        public float GetGameSpeed()
        {
            return speedUp ? Main.OptionsManager.GetFastForwardMultiplier() : 1;
        }
        public void RemoveLevelState()
        {
            LevelManager.RemoveLevelState(StartStageID);
        }
        public NamespaceID GetStartAreaID()
        {
            return StartAreaID;
        }
        public NamespaceID GetStartStageID()
        {
            return StartStageID;
        }

        public void UpdateLogic()
        {
            if (isGameOver)
            {
                var killerCtrl = killerEntity;
                if (killerCtrl)
                {
                    var killerEnt = killerCtrl.Entity;
                    var pos = killerEnt.Position;
                    pos.x -= 1;
                    pos.z = pos.z * 0.5f + level.GetDoorZ() * 0.5f;
                    pos.y = pos.y * 0.5f + level.GetGroundY(pos.x, pos.z) * 0.5f;
                    killerEnt.Position = pos;
                    killerCtrl.UpdateFixed();

                    var passenger = killerEnt.GetRideablePassenger();
                    if (passenger != null)
                    {
                        passenger.Position = pos + killerEnt.GetPassengerOffset();
                        var passengerCtrl = GetEntityController(passenger);
                        if (passengerCtrl)
                        {
                            passengerCtrl.UpdateFixed();
                        }
                    }
                }
                foreach (var entity in entities.ToArray())
                {
                    if (CanUpdateAfterGameOver(entity.Entity))
                    {
                        entity.Entity.Update();
                        entity.UpdateFixed();
                    }
                }
                return;
            }
            else
            {
                if (!IsGameRunning())
                {
                    foreach (var entity in entities.ToArray())
                    {
                        bool canRunBeforeGameStart = IsGameStarted() || CanUpdateBeforeGameStart(entity.Entity);
                        bool canRunInPause = !IsGamePaused() || CanUpdateInPause(entity.Entity);
                        if (canRunBeforeGameStart && canRunInPause)
                        {
                            entity.Entity.Update();
                            entity.UpdateFixed();
                        }
                    }
                }
                else
                {
                    var gameSpeed = GetGameSpeed();
                    var times = (int)gameSpeed;
                    gameRunTimeModular += gameSpeed - times;
                    if (gameRunTimeModular > 1)
                    {
                        times += (int)gameRunTimeModular;
                        gameRunTimeModular %= 1;
                    }
                    for (int time = 0; time < times; time++)
                    {
                        // 用于中断循环。防止Update后游戏结束，然后执行两次。
                        if (!IsGameRunning())
                            break;
                        var entitiesCache = entities.ToArray();
                        level.Update();
                        foreach (var entity in entitiesCache)
                        {
                            entity.UpdateFixed();
                        }
                        gridLayout.UpdateGridsFixed();
                        foreach (var part in parts)
                        {
                            part.UpdateLogic();
                        }
                        ui.UpdateHeldItemModelFixed();
                        UpdateEnemyCry();
                    }
                }
            }
        }
        public void UpdateFrame(float deltaTime)
        {
            float gameSpeed = isGameOver ? 1 : GetGameSpeed();
            bool gameRunning = IsGameRunning();
            // 更新实体动画。
            foreach (var entity in entities)
            {
                bool modelActive = false;
                var ent = entity.Entity;
                if (isGameOver)
                {
                    // 如果游戏结束，则只有在实体是杀死玩家的实体，或者在游戏结束后能行动时，才会动起来。
                    var killerCtrl = killerEntity;
                    var killerEnt = killerCtrl?.Entity;
                    modelActive = CanUpdateAfterGameOver(ent) || ent == killerEnt || ent == killerEnt?.GetRideablePassenger();
                }
                else
                {
                    // 游戏没有结束，则只有在游戏运行中，或者实体可以在游戏开始前行动，或者实体是预览敌人时，才会动起来。
                    bool canRunBeforeGameStart = IsGameStarted() || CanUpdateBeforeGameStart(ent);
                    bool canRunInPause = !IsGamePaused() || CanUpdateInPause(ent);
                    modelActive = (canRunBeforeGameStart && canRunInPause) || ent.IsPreviewEnemy();
                }
                float speed = modelActive ? gameSpeed : 0;
                entity.SetSimulationSpeed(speed);
                entity.UpdateFrame(deltaTime * speed);
            }
            if (!isGameOver)
            {
                // 游戏运行时更新UI。
                if (gameRunning)
                {
                    AdvanceLevelProgress();

                    UpdateHeldItemPosition();
                    UpdateInLevelUI(deltaTime * gameSpeed);
                }
                // 更新手持物品。
                var speed = gameRunning ? gameSpeed : 0;
                ui.UpdateHeldItemModelFrame(deltaTime * speed);
                ui.SetHeldItemModelSimulationSpeed(speed);
                UpdateTwinkle(gameRunning ? deltaTime : 0);
            }
            // 更新光标。
            UpdateHeldItemCursor();
            UpdateTooltip();

            levelCamera.ShakeOffset = (Vector3)Shakes.GetShake2D();

            bool paused = IsGamePaused();
            // 暂停时显示金钱。
            if (paused)
            {
                ShowMoney();
            }

            // 设置射线检测。
            ui.SetRaycastDisabled(IsInputDisabled());

            // 更新UI。
            var uiSimulationSpeed = paused ? 0 : gameSpeed;
            var uiDeltaTime = deltaTime * uiSimulationSpeed;

            var uiPreset = GetUIPreset();
            uiPreset.UpdateFrame(uiDeltaTime);
            UpdateGridHighlight();
            UpdateInput();

            // 更新场景。
            if (model)
            {
                model.UpdateFrame(uiDeltaTime);
                model.SetSimulationSpeed(uiSimulationSpeed);
            }

            if (level != null)
            {
                // 设置光照。
                float darknessSpeed = 2;
                if (!IsGameStarted() || IsGameOver())
                {
                    darknessSpeed = -2;
                }
                darknessFactor = Mathf.Clamp01(darknessFactor + darknessSpeed * uiDeltaTime);
                UpdateLighting();
                ui.SetScreenCover(level.GetScreenCover());
                UpdateCamera();
                UpdateMoney();
                ValidateHeldItem();
                UpdateEntityHighlight();
                foreach (var component in level.GetComponents())
                {
                    if (component is IMVZ2LevelComponent comp)
                    {
                        comp.UpdateFrame(deltaTime, uiSimulationSpeed);
                    }
                }
                foreach (var part in parts)
                {
                    part.UpdateFrame(deltaTime, uiSimulationSpeed);
                }
            }
        }
        public void PauseGame(int pauseLevel = 0)
        {
            if (isPaused)
            {
                if (pauseLevel > this.pauseLevel)
                {
                    this.pauseLevel = pauseLevel;
                }
                return;
            }
            this.pauseLevel = pauseLevel;
            isPaused = true;
            Music.Pause();
        }
        public bool ResumeGame(int level = 0)
        {
            if (!isPaused || level < pauseLevel)
                return false;
            if (Main.Scene.HasDialog())
                return false;
            pauseLevel = 0;
            isPaused = false;
            Music.Resume();

            if (optionsLogic != null)
            {
                optionsLogic.Dispose();
                optionsLogic = null;
            }
            ui.SetPauseDialogActive(false);
            ui.SetOptionsDialogActive(false);
            ui.SetLevelLoadedDialogVisible(false);
            levelLoaded = false;
            return true;
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public void SetMusicLowQuality(bool lowQuality)
        {
            normalAudioListener.SetActive(!lowQuality);
            lowQualityAudioListener.SetActive(lowQuality);
        }

        #region 关卡模型
        public void SetModelPreset(string name)
        {
            if (!model)
                return;
            model.SetPreset(name);
        }
        public void TriggerModelAnimator(string name)
        {
            if (!model)
                return;
            model.TriggerAnimator(name);
        }
        public void SetModelAnimatorBool(string name, bool value)
        {
            if (!model)
                return;
            model.SetAnimatorBool(name, value);
        }
        public void SetModelAnimatorInt(string name, int value)
        {
            if (!model)
                return;
            model.SetAnimatorInt(name, value);
        }
        public void SetModelAnimatorFloat(string name, float value)
        {
            if (!model)
                return;
            model.SetAnimatorFloat(name, value);
        }
        #endregion
        public ILevelUI GetUI()
        {
            return ui;
        }
        public LevelEngine GetEngine()
        {
            return level;
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            parts = new ILevelControllerPart[]
            {
                blueprintController,
                blueprintChooseController,
            };
            Awake_Grids();

            talkController.OnTalkAction += UI_OnTalkActionCallback;

            levelCamera.SetPosition(cameraHousePosition, cameraHouseAnchor);

            ClearGridHighlight();

            ui.SetMobile(Main.IsMobile());

            ui.OnExitLevelToNoteCalled += UI_OnExitLevelToNoteCalledCallback;
            ui.OnStartGameCalled += StartGame;

            foreach (var controller in parts)
            {
                controller.Init(this);
            }

            Awake_UI();
        }
        private void OnApplicationFocus(bool focus)
        {
            UpdateFocusLost(focus);
        }
        #endregion

        #region 事件回调

        private void Engine_OnGameOverCallback(int type, Entity killer, string message)
        {
            switch (type)
            {
                case GameOverTypes.ENEMY:
                    GameOver(killer);
                    break;
                case GameOverTypes.NO_ENEMY:
                    GameOver(message);
                    break;
                case GameOverTypes.INSTANT:
                    GameOverInstantly(message);
                    break;
            }
        }
        private async void Engine_OnClearCallback()
        {
            RemoveLevelState();
            Saves.Unlock(VanillaSaveExt.GetLevelClearUnlockID(level.StageID));
            Saves.AddLevelDifficultyRecord(level.StageID, level.Difficulty);
            Saves.SaveModDatas();

            var mapTalks = level.GetTalksOfType(StageMetaTalk.TYPE_MAP);
            if (mapTalks != null)
            {
                foreach (var mapTalk in mapTalks)
                {
                    if (level.IsRerun && !mapTalk.ShouldRepeat(Main.SaveManager))
                        continue;
                    if (!Main.ResourceManager.CanStartTalk(mapTalk.Value, mapTalk.StartSection))
                        continue;
                    Saves.SetMapTalk(mapTalk.Value);
                    break;
                }
            }

            var endTalks = level.GetTalksOfType(StageMetaTalk.TYPE_END);
            float transitionDelay = 3;
            if (endTalks != null)
            {
                NamespaceID talkID = null;
                foreach (var endTalk in endTalks)
                {
                    if (level.IsRerun && !endTalk.ShouldRepeat(Main.SaveManager))
                        continue;
                    if (!Main.ResourceManager.CanStartTalk(endTalk.Value, endTalk.StartSection))
                        continue;
                    talkID = endTalk.Value;
                    break;
                }
                if (NamespaceID.IsValid(talkID))
                {
                    await talkController.SimpleStartTalkAsync(talkID, 0, 5, () => transitionDelay = 0);
                }
            }
            StartExitLevelTransition(transitionDelay);
        }
        private async void UI_OnExitLevelToNoteCalledCallback()
        {
            await ExitLevelToNote(exitTargetNoteID);
        }
        private void PostWaveFinishedCallback(LevelCallbacks.PostWaveParams param, CallbackResult result)
        {
            UpdateLevelName();
        }
        private void PostHugeWaveApproachCallback(LevelCallbackParams param, CallbackResult result)
        {
            var ui = GetUIPreset();
            ui.ShowHugeWaveText();
        }
        private void PostFinalWaveCallback(LevelCallbackParams param, CallbackResult result)
        {
            var ui = GetUIPreset();
            ui.ShowFinalWaveText();
        }

        #endregion

        #region 暂停
        private void UpdateFocusLost(bool focus)
        {
            if (IsInputDisabled())
                return;
            if (!IsGameRunning())
                return;
            if (focus)
                return;
            if (!Options.GetPauseOnFocusLost())
                return;
            if (IsPauseDisabled())
                return;
            PauseGame();
            ShowPausedDialog();
        }
        private bool IsPauseDisabled()
        {
            if (level == null)
                return true;
            return level.IsPauseDisabled();
        }
        #endregion

        #region 游戏结束
        private void SetGameOver()
        {
            isGameOver = true;
            level.PlaySound(VanillaSoundID.loseMusic);
            model.SetDoorVisible(false);
            level.HideAdvice();
            SetUIVisibleState(VisibleState.Nothing);
            RemoveLevelState();
        }
        #endregion

        private void SwitchSpeedUp()
        {
            speedUp = !speedUp;
            GetUIPreset().SetSpeedUp(speedUp);
            level.PlaySound(speedUp ? VanillaSoundID.fastForward : VanillaSoundID.slowDown);
        }

        private async Task ExitScene()
        {
            Saves.SaveModDatas();
            Dispose();
            await LevelManager.ExitLevelSceneAsync();
        }
        private void UpdateInput()
        {
            if (IsInputDisabled())
                return;
            UpdatePointerRelease();
            UpdateKeys();
        }
        private void UpdatePointerRelease()
        {
            if (Input.touchCount > 0)
            {
                foreach (var position in Main.InputManager.GetTouchUps())
                {
                    OnPointerRelease(position);
                }
            }
            else
            {
                foreach (var position in Main.InputManager.GetMouseUps(MouseButtons.LEFT))
                {
                    OnPointerRelease(position);
                }
            }
        }
        private void OnPointerRelease(PointerPositionParams pointer)
        {
            var eventSystem = EventSystem.current;
            var results = new List<RaycastResult>();
            var pointerId = InputManager.GetPointerIdByButtonAndType(pointer.button, pointer.type);
            var eventData = new PointerEventData(eventSystem)
            {
                position = pointer.position,
                button = (PointerEventData.InputButton)pointer.button,
                pointerId = pointerId,
            };
            eventSystem.RaycastAll(eventData, results);
            var first = results.FirstOrDefault(r => r.gameObject);
            if (first.isValid)
            {
                eventData.pointerCurrentRaycast = first;
                ExecuteEvents.ExecuteHierarchy<IPointerReleaseHandler>(first.gameObject, eventData, (x, y) => x.OnPointerRelease(ExecuteEvents.ValidateEventData<PointerEventData>(y)));
            }
        }
        private void UpdateKeys()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
            {
                foreach (var enemy in level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsHostile(SelfFaction) && !e.IsDead))
                {
                    enemy.Die();
                }
            }
            if (isGameStarted && !isGameOver)
            {
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    LevelManager.SaveLevel();
                    Debug.Log("Game Saved!");
                }
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    Debug.Log("Restarting Game...");
                    _ = ReloadLevel();
                }
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                var spawnDef = level.Content.GetSpawnDefinition(VanillaSpawnID.zombie);
                for (int i = 0; i < 50; i++)
                {
                    level.SpawnEnemyAtRandomLane(spawnDef);
                }
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                foreach (var boss in level.FindEntities(e => e.Type == EntityTypes.BOSS && !e.IsDead))
                {
                    boss.Die();
                }
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                var contraptions = Main.SaveManager.GetUnlockedContraptions();
                var grids = level.GetAllGrids();
                for (int i = 0; i < contraptions.Length; i++)
                {
                    var contraption = contraptions[i];
                    var grid = grids.FirstOrDefault(g => g.CanSpawnEntity(contraption));
                    if (grid == null)
                        continue;
                    var spawnParams = CommandBlock.GetImitateSpawnParams(contraption);
                    var command = grid.SpawnPlacedEntity(VanillaContraptionID.commandBlock, spawnParams);
                }
            }
#endif
            if (!isGameOver)
            {
                if (isGameStarted && !levelLoaded)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (!isPaused)
                        {
                            if (!IsPauseDisabled())
                            {
                                PauseGame();
                                level.PlaySound(VanillaSoundID.pause);
                                ShowPausedDialog();
                            }
                        }
                        else
                        {
                            ResumeGame();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (!isPaused)
                        {
                            if (!IsPauseDisabled())
                            {
                                PauseGame();
                                level.PlaySound(VanillaSoundID.pause);
                                ShowOptionsDialog();
                            }
                        }
                        else
                        {
                            ResumeGame();
                        }
                    }
                }
                if (Input.GetKeyDown(Options.GetKeyBinding(HotKeys.fastForward)))
                {
                    SwitchSpeedUp();
                }
            }


            if (IsGameRunning())
            {
                bool conveyor = level.IsConveyorMode();
                int seedCount = conveyor ? level.GetConveyorSeedPackCount() : level.GetSeedSlotCount();
                for (int i = 0; i < seedCount; i++)
                {
                    var key = Options.GetBlueprintKeyBinding(i);
                    if (Input.GetKeyDown(key))
                    {
                        SeedPack seedPack = conveyor ? level.GetConveyorSeedPackAt(i) : level.GetSeedPackAt(i);
                        if (seedPack == null)
                            break;
                        var target = new HeldItemTargetBlueprint(level, i, conveyor);
                        var pointerParams = new PointerInteractionData()
                        {
                            pointer = new PointerData()
                            {
                                button = (int)key,
                                type = PointerTypes.KEY,
                            },
                            interaction = PointerInteraction.Key
                        };
                        level.DoHeldItemPointerEvent(target, pointerParams);
                    }
                }
                if (Input.GetKeyDown(Options.GetKeyBinding(HotKeys.pickaxe)))
                {
                    ClickPickaxe();
                }
                if (Input.GetKeyDown(Options.GetKeyBinding(HotKeys.starshard)))
                {
                    ClickStarshard();
                }
                if (Input.GetKeyDown(Options.GetKeyBinding(HotKeys.trigger)))
                {
                    ClickTrigger();
                }
            }
        }
        private bool CanUpdateBeforeGameStart(Entity entity)
        {
            return entity.CanUpdateBeforeGameStart();
        }
        private bool CanUpdateInPause(Entity entity)
        {
            return entity.CanUpdateInPause();
        }
        private bool CanUpdateAfterGameOver(Entity entity)
        {
            return entity.CanUpdateAfterGameOver();
        }
        private bool IsInputDisabled()
        {
            return level == null || level.IsCleared || isOpeningAlmanac || isOpeningStore || inputAndUIDisabled;
        }

        private void CreateLevelModel(NamespaceID areaId)
        {
            var areaDef = Game.GetAreaDefinition(areaId);
            if (areaDef == null)
                return;
            var modelID = areaDef.GetModelID();
            if (modelID == null)
                return;
            var modelPrefab = Resources.GetAreaModel(modelID);
            if (modelPrefab == null)
                return;
            model = Instantiate(modelPrefab.gameObject, modelRoot).GetComponent<AreaModel>();
        }
        private void InitLevelModel(NamespaceID stageId)
        {
            var stageMeta = Resources.GetStageMeta(stageId);
            if (stageMeta == null)
                return;
            SetModelPreset(stageMeta.ModelPreset);
        }
        private async Task StartLevelIntroAsync(float delay)
        {
            if (delay > 0)
            {
                await Main.CoroutineManager.DelaySeconds(delay);
            }
            SetCameraPosition(level.StageDefinition.GetStartCameraPosition());

            var startTalks = level.GetTalksOfType(StageMetaTalk.TYPE_START);
            if (startTalks != null)
            {
                NamespaceID talkID = null;
                foreach (var startTalk in startTalks)
                {
                    if (level.IsRerun && !startTalk.ShouldRepeat(Main.SaveManager))
                        continue;
                    if (!Main.ResourceManager.CanStartTalk(startTalk.Value, startTalk.StartSection))
                        continue;
                    talkID = startTalk.Value;
                    break;
                }
                if (NamespaceID.IsValid(talkID))
                {
                    await talkController.SimpleStartTalkAsync(talkID, 0, 2, () =>
                    {
                        if (!level.NoStartTalkMusic())
                        {
                            Music.Play(VanillaMusicID.mainmenu);
                        }
                    });
                }
            }
            level.BeginLevel();
        }
        private void InitLevelEngine(LevelEngine level, Game game, NamespaceID areaID, NamespaceID stageID)
        {
            ApplyComponents(level);
            game.SetLevel(level);

            levelRaycaster.Init(level);
            AddLevelCallbacks();

            CreateLevelModel(areaID);
            InitLevelModel(stageID);

            talkSystem = new LevelTalkSystem(level, talkController);

            level.IsRerun = Saves.IsLevelCleared(stageID);
        }
        private void SetGameStarted(bool value)
        {
            isGameStarted = value;
        }
        private void UpdateLighting()
        {
            var background = Color.white;
            var global = Color.white;
            if (level != null)
            {
                background = level.GetBackgroundLight();
                global = Color.Lerp(Color.white, level.GetGlobalLight(), darknessFactor);
            }
            SetLighting(background, global);
        }
        private void SetLighting(Color night, Color darkness)
        {
            Main.GraphicsManager.SetLighting(night, darkness);
            model?.SetLighting(darkness);
        }
        #endregion

        #region 属性字段
        public float LawnToTransScale => LevelManager.LawnToTransScale;
        public float TransToLawnScale => LevelManager.TransToLawnScale;
        public Game Game => Main.Game;
        private MainManager Main => MainManager.Instance;
        private SaveManager Saves => Main.SaveManager;
        private MusicManager Music => Main.MusicManager;
        private LevelManager LevelManager => Main.LevelManager;
        private LanguageManager Localization => Main.LanguageManager;
        private ResourceManager Resources => Main.ResourceManager;
        private SoundManager Sounds => Main.SoundManager;
        private MainSceneController Scene => Main.Scene;
        private OptionsManager Options => Main.OptionsManager;
        private ShakeManager Shakes => Main.ShakeManager;
        private LevelEngine level;
        private bool isPaused = false;
        private int pauseLevel = 0;
        private bool levelLoaded = false;
        private bool isGameStarted;
        private bool isGameOver;
        private bool isOpeningAlmanac;
        private bool isOpeningStore;
        private bool speedUp;
        private float gameRunTimeModular;
        private NamespaceID killerID;
        private EntityController killerEntity;
        private string deathMessage;
        private float darknessFactor = 1;
        private NamespaceID exitTargetNoteID;
        private AreaModel model;
        private bool isDisposed;
        public NamespaceID StartAreaID { get; set; }
        public NamespaceID StartStageID { get; set; }

        #region 保存属性
        public NamespaceID CurrentMusic
        {
            get => Music.GetCurrentMusicID();
            set
            {
                if (NamespaceID.IsValid(value))
                {
                    Music.Play(value);
                }
                else
                {
                    Music.Stop();
                }
            }
        }
        public float MusicTime
        {
            get => Music.Time;
            set => Music.Time = value;
        }
        public float MusicVolume
        {
            get => Music.GetVolume();
            set => Music.SetVolume(value);
        }
        public float MusicTrackWeight
        {
            get => Music.GetTrackWeight();
            set => Music.SetTrackWeight(value);
        }
        public bool EnergyActive
        {
            get => energyActive;
            set
            {
                energyActive = value;
                var uiPreset = GetUIPreset();
                uiPreset.SetEnergyActive(value);
            }
        }
        public bool BlueprintsActive
        {
            get => blueprintsActive;
            set
            {
                blueprintsActive = value;
                ui.Blueprints.SetBlueprintsActive(value);
            }
        }
        public bool PickaxeActive
        {
            get => pickaxeActive;
            set
            {
                pickaxeActive = value;
                var uiPreset = GetUIPreset();
                uiPreset.SetPickaxeActive(value);
            }
        }
        public bool StarshardActive
        {
            get => starshardActive;
            set
            {
                starshardActive = value;
                var uiPreset = GetUIPreset();
                uiPreset.SetStarshardActive(value);
            }
        }
        public bool TriggerActive
        {
            get => triggerActive;
            set
            {
                triggerActive = value;
                var uiPreset = GetUIPreset();
                uiPreset.SetTriggerActive(value);
            }
        }
        private bool energyActive = true;
        private bool blueprintsActive = true;
        private bool pickaxeActive = true;
        private bool starshardActive = true;
        private bool triggerActive = true;
        #endregion

        private ILevelControllerPart[] parts;
        private LevelExitTarget exitTarget;
        [Header("Main")]
        [SerializeField]
        private LevelUI ui;
        [SerializeField]
        private Transform modelRoot;

        [Header("Audio")]
        [SerializeField]
        private GameObject normalAudioListener;
        [SerializeField]
        private GameObject lowQualityAudioListener;
        #endregion
    }
}
