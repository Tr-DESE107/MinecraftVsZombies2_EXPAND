using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVZ2.Audios;
using MVZ2.Cameras;
using MVZ2.Entities;
using MVZ2.GameContent.Enemies;
using MVZ2.Games;
using MVZ2.Grids;
using MVZ2.Localization;
using MVZ2.Managers;
using MVZ2.Options;
using MVZ2.Saves;
using MVZ2.Scenes;
using MVZ2.UI;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.Talk;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    using VisibleState = MVZ2.Level.UI.LevelUIPreset.VisibleState;
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法

        #region 游戏流程
        public void InitLevel(Game game, NamespaceID areaID, NamespaceID stageID)
        {

            level = new LevelEngine(game, game, game);
            InitLevelEngine(level, game, areaID, stageID);

            var option = new LevelOption()
            {
                CardSlotCount = 10,
                StarshardSlotCount = 10,
                LeftFaction = SelfFaction,
                RightFaction = EnemyFaction,
                StartEnergy = 50,
                MaxEnergy = 9990,
                TPS = 30
            };
            level.Init(areaID, stageID, option);
            level.SetupArea();

            var uiPreset = GetUIPreset();
            uiPreset.UpdateFrame(0);
            SetUnlockedUIActive();
        }
        public void StartLevelIntro(float delay)
        {
            if (delay <= 0)
            {
                StartLevelIntroInstant();
            }
            else
            {
                StartCoroutine(StartLevelIntroDelayed(delay));
            }
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

            for (int i = 0; i < chosenBlueprints.Count; i++)
            {
                var index = chosenBlueprints[i];
                var blueprintID = choosingBlueprints[index];
                level.ReplaceSeedPackAt(i, blueprintID);
            }
            chosenBlueprints.Clear();
            choosingBlueprints = null;

            Music.Play(level.GetMusicID());

            levelProgress = 0;
            bannerProgresses = new float[level.GetTotalFlags()];

            SetUIVisibleState(VisibleState.InLevel);
            SetUnlockedUIActive();

            UpdateBlueprintCount();
            UpdateLevelName();
            UpdateDifficulty();
            UpdateLevelUI();

            var uiPreset = GetUIPreset();
            uiPreset.SetBlockRaycasts(true);

            level.Start();

            isGameStarted = true;
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
            LevelManager.InitLevel(StartAreaID, StartStageID);
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
            ShowGameOverDialog();
        }
        public void StopLevel()
        {
            SetUIVisibleState(VisibleState.Nothing);
            pointingGridLane = -1;
            pointingGridColumn = -1;
            level.ClearEnergyDelayedEntities();
            level.ClearDelayedMoney();
            UpdateGridHighlight();
            isGameStarted = false;
            Saves.SaveModDatas();
        }
        public void Dispose()
        {
            if (optionsLogic != null)
            {
                optionsLogic.Dispose();
                optionsLogic = null;
            }
            level?.StopAllLoopSounds();
            level?.Dispose();
        }
        public async Task ExitLevelToNote(NamespaceID id)
        {
            await ExitScene();
            var buttonText = Localization._(Vanilla.VanillaStrings.CONTINUE);
            Sounds.Play2D(VanillaSoundID.paper);
            Scene.DisplayNote(id, buttonText);
        }
        public async Task ExitLevel()
        {
            await ExitScene();
            BackToMapOrMainmenu();
        }
        public void BackToMapOrMainmenu()
        {
            Scene.GotoMapOrMainmenu();
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
            return speedUp ? 2 : 1;
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
                if (killerEntity)
                {
                    var pos = killerEntity.Entity.Position;
                    pos.x -= 1;
                    pos.z = pos.z * 0.5f + level.GetDoorZ() * 0.5f;
                    pos.y = pos.y * 0.5f + level.GetGroundY(pos.x, pos.z) * 0.5f;
                    killerEntity.Entity.Position = pos;
                    killerEntity.UpdateFixed();
                }
                return;
            }
            else
            {
                if (!IsGameRunning())
                {
                    foreach (var entity in entities.ToArray())
                    {
                        if (CanUpdateBeforeGameStart(entity.Entity))
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
                        UpdateEnemyCry();
                    }
                }
            }
        }
        public void UpdateFrame(float deltaTime)
        {
            float gameSpeed = GetGameSpeed();
            if (isGameOver)
            {
                if (killerEntity)
                {
                    float simulationSpeed = 1;
                    killerEntity.SetSimulationSpeed(simulationSpeed);
                    killerEntity.UpdateFrame(deltaTime);
                }
            }
            else
            {
                // 更新实体动画。
                // 只有在游戏运行中，或者实体可以在游戏开始前行动，或者实体是预览敌人时，才会动起来。
                foreach (var entity in entities)
                {
                    bool modelActive = IsGameRunning() || CanUpdateBeforeGameStart(entity.Entity) || entity.Entity.IsPreviewEnemy();
                    float simulationSpeed = modelActive ? gameSpeed : 0;
                    entity.SetSimulationSpeed(simulationSpeed);
                    entity.UpdateFrame(deltaTime * simulationSpeed);
                }

                // 游戏运行时更新UI。
                if (IsGameRunning())
                {
                    AdvanceLevelProgress();

                    bool isPressing = Input.touchCount > 0 || Input.GetMouseButton(0);
                    Vector2 heldItemPosition;
                    if (Main.IsMobile() && !isPressing)
                    {
                        heldItemPosition = new Vector2(-1000, -1000);
                    }
                    else
                    {
                        heldItemPosition = levelCamera.Camera.ScreenToWorldPoint(Input.mousePosition);
                    }
                    ui.SetHeldItemPosition(heldItemPosition);
                    UpdateLevelUI();

                    levelCamera.ShakeOffset = (Vector3)Shakes.GetShake2D();
                }
            }
            if (isPaused)
            {
                ShowMoney();
            }
            ui.SetRaycastDisabled(IsInputDisabled());
            if (level != null)
            {
                ui.SetNightValue(level.GetNightValue());
            }
            var uiPreset = GetUIPreset();
            var uiDeltaTime = IsGamePaused() ? 0 : deltaTime * gameSpeed;
            uiPreset.UpdateFrame(uiDeltaTime);
            UpdateGridHighlight();
            UpdateInput();
        }
        public void Pause()
        {
            isPaused = true;
            Music.Pause();
        }
        public void Resume()
        {
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
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            Awake_Grids();

            talkController.OnTalkAction += UI_OnTalkActionCallback;
            talkController.OnTalkEnd += UI_OnTalkEndCallback;

            levelCamera.SetPosition(cameraHousePosition, cameraHouseAnchor);

            ClearGridHighlight();

            ui.OnExitLevelToNoteCalled += UI_OnExitLevelToNoteCalledCallback;
            var uiPreset = GetUIPreset();
            standaloneUI.SetActive(standaloneUI == uiPreset);
            mobileUI.SetActive(mobileUI == uiPreset);

            uiPreset.OnStartGameCalled += StartGame;

            Awake_Blueprints();
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
        private void Engine_OnClearCallback()
        {
            LevelManager.RemoveLevelState(StartStageID);
            Saves.Unlock(VanillaSaveExt.GetLevelClearUnlockID(level.StageID));
            Saves.AddLevelDifficultyRecord(level.StageID, level.Difficulty);
            Saves.SaveModDatas();
            Saves.SetMapTalk(level.GetMapTalk());

            var stageID = level.StageID;
            var endTalk = level.GetEndTalk() ?? new NamespaceID(stageID.spacename, $"{stageID.path}_over");
            bool played = false;
            if (!level.IsRerun && talkController.CanStartTalk(endTalk))
            {
                played = talkController.TryStartTalk(endTalk, 0, 5);
            }
            if (!played)
            {
                StartExitLevelTransition(3);
            }
        }
        private async void UI_OnExitLevelToNoteCalledCallback()
        {
            await ExitLevelToNote(exitTargetNoteID);
        }
        private void PostHugeWaveApproachCallback(LevelEngine level)
        {
            var ui = GetUIPreset();
            ui.ShowHugeWaveText();
        }
        private void PostFinalWaveCallback(LevelEngine level)
        {
            var ui = GetUIPreset();
            ui.ShowFinalWaveText();
        }

        #endregion

        #region 暂停
        private void UpdateFocusLost(bool focus)
        {
            if (isPaused)
                return;
            if (!IsGameStarted())
                return;
            if (focus)
                return;
            if (!Options.GetPauseOnFocusLost())
                return;

            Pause();
            ShowPausedDialog();
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
            UpdatePointerUp();
            UpdateKeys();
        }
        private void UpdatePointerUp()
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnLeftPointerUp(Input.mousePosition);
            }
            var touches = Input.touches;
            for (int i = 0; i < touches.Length; i++)
            {
                var touch = touches[i];
                if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                {
                    OnLeftPointerUp(touch.position);
                }
            }
        }
        private GameObject GetRaycastGameObject(Vector2 screenPosition)
        {
            var eventSystem = EventSystem.current;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            eventSystem.RaycastAll(new PointerEventData(eventSystem) { position = screenPosition }, raycastResults);
            foreach (var raycastResult in raycastResults)
            {
                if (!raycastResult.isValid)
                    continue;
                return raycastResult.gameObject;
            }
            return null;
        }
        private void OnLeftPointerUp(Vector2 screenPosition)
        {
            if (!Main.IsMobile())
                return;
            var gameObject = GetRaycastGameObject(screenPosition);
            if (!gameObject || !gameObject.activeInHierarchy)
                return;
            var grid = gameObject.GetComponentInParent<GridController>();
            if (grid)
            {
                ClickOnGrid(grid.Lane, grid.Column);
                return;
            }

            var receiver = gameObject.GetComponentInParent<RaycastReceiver>();
            if (receiver)
            {
                ClickOnReceiver(receiver);
                return;
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
                var spawnDef = level.Content.GetSpawnDefinition(VanillaEnemyID.zombie);
                for (int i = 0; i < 30; i++)
                {
                    level.SpawnEnemy(spawnDef, 2);
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
                            Pause();
                            level.PlaySound(VanillaSoundID.pause);
                            ShowPausedDialog();
                        }
                        else
                        {
                            Resume();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (!isPaused)
                        {
                            Pause();
                            level.PlaySound(VanillaSoundID.pause);
                            ShowOptionsDialog();
                        }
                        else
                        {
                            Resume();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    SwitchSpeedUp();
                }
            }


            if (IsGameRunning())
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (level.CancelHeldItem())
                    {
                        level.PlaySound(VanillaSoundID.tap);
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                    {
                        var index = i == 0 ? 9 : i - 1;
                        ClickBlueprint(index);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ClickPickaxe();
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    ClickStarshard();
                }
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    ClickTrigger();
                }
            }
        }
        private bool CanUpdateBeforeGameStart(Entity entity)
        {
            return entity.CanUpdateBeforeGameStart();
        }
        private bool IsInputDisabled()
        {
            return level == null || level.IsCleared || isOpeningAlmanac;
        }

        private void CreateLevelModel(NamespaceID areaId)
        {
            var areaMeta = Resources.GetAreaMeta(areaId);
            if (areaMeta == null)
                return;
            var modelPrefab = Resources.GetAreaModel(areaMeta.model);
            if (modelPrefab == null)
                return;
            model = Instantiate(modelPrefab.gameObject, modelRoot).GetComponent<AreaModel>();
        }
        private IEnumerator StartLevelIntroDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartLevelIntroInstant();
        }
        private void StartLevelIntroInstant()
        {
            SetCameraPosition(level.StageDefinition.GetStartCameraPosition());

            var startTalk = level.GetStartTalk() ?? level.StageID;
            bool played = false;
            if (!level.IsRerun && talkController.CanStartTalk(startTalk))
            {
                played = talkController.TryStartTalk(startTalk, 0, 2);
            }
            if (played)
            {
                Music.Play(VanillaMusicID.mainmenu);
            }
            else
            {
                level.BeginLevel();
            }
        }
        private void InitLevelEngine(LevelEngine level, Game game, NamespaceID areaID, NamespaceID stageID)
        {
            ApplyComponents(level);
            game.SetLevel(level);

            levelRaycaster.Init(level);
            AddLevelCallbacks();
            CreateLevelModel(areaID);
            talkSystem = new LevelTalkSystem(level, talkController);

            level.IsRerun = Saves.IsLevelCleared(stageID);
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
        private bool levelLoaded = false;
        private bool isGameStarted;
        private bool isGameOver;
        private bool isOpeningAlmanac;
        private bool speedUp;
        private float gameRunTimeModular;
        private NamespaceID killerID;
        private EntityController killerEntity;
        private string deathMessage;
        private NamespaceID exitTargetNoteID;
        private AreaModel model;
        public NamespaceID StartAreaID { get; set; }
        public NamespaceID StartStageID { get; set; }

        #region 保存属性
        public NamespaceID CurrentMusic
        {
            get => Music.GetCurrentMusicID();
            set => Music.Play(value);
        }
        public float MusicTime
        {
            get => Music.Time;
            set => Music.Time = value;
        }
        public bool BlueprintsActive
        {
            get => blueprintsActive;
            set
            {
                blueprintsActive = value;
                var uiPreset = GetUIPreset();
                uiPreset.SetBlueprintsActive(value);
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
        private bool blueprintsActive = true;
        private bool pickaxeActive = true;
        private bool starshardActive = true;
        private bool triggerActive = true;
        #endregion

        [Header("Main")]
        [SerializeField]
        private LevelUI ui;
        [SerializeField]
        private Transform modelRoot;
        #endregion
    }
}
