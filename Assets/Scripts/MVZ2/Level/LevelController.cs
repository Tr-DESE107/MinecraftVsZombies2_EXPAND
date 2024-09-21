using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.GameContent;
using MVZ2.UI;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Game;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法

        #region 游戏流程
        public void InitGame(Game game, NamespaceID areaID, NamespaceID stageID)
        {
            level = new LevelEngine(game, game);

            ApplyComponents(level);
            game.SetLevel(level);

            AddLevelCallbacks();

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
            StartAreaID = areaID;

            var startTalk = level.GetStartTalk();
            if (startTalk != null)
            {
                Main.MusicManager.Play(MusicID.mainmenu);
                StartTalk(startTalk, 0, 2);
            }
            else
            {
                BeginLevel();
            }
        }
        public void BeginLevel()
        {
            BeginLevel(LevelTransitions.DEFAULT);
        }
        public void BeginLevel(string transition)
        {
            if (transition == LevelTransitions.TO_LAWN)
            {
                StartCoroutine(GameStartToLawnTransition());
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
            if (level.StageDefinition is IPreviewStage preview)
            {
                preview.RemovePreviewEnemies(level);
            }

            Main.MusicManager.Play(level.GetMusicID());

            levelProgress = 0;
            bannerProgresses = new float[level.GetTotalFlags()];

            level.SetDifficulty(Main.OptionsManager.GetDifficulty());

            //level.SetEnergy(9990);
            //level.RechargeSpeed = 99;

            SetUIVisibleState(VisibleState.InLevel);

            UpdateBlueprintCount();
            UpdateLevelName();
            UpdateDifficultyName();
            UpdateLevelUI(GetGameSpeed());

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
            var levelUI = GetLevelUI();
            levelUI.SetGameOverDialogInteractable(false);
            Dispose();
            await Main.LevelManager.GotoLevelSceneAsync();
            Main.LevelManager.StartLevel(StartAreaID, StartStageID);
        }
        public void GameOver(Entity killer)
        {
            killerID = killer.Definition.GetID();
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
            isGameStarted = false;
        }
        public void Dispose()
        {
            BuiltinCallbacks.PostHugeWaveApproach.Remove(PostHugeWaveApproachCallback);
            BuiltinCallbacks.PostFinalWave.Remove(PostFinalWaveCallback);
            if (optionsLogic != null)
            {
                optionsLogic.Dispose();
                optionsLogic = null;
            }
        }
        public async Task ExitLevel()
        {
            var lastMapID = Main.SaveManager.GetLastMapID();
            if (NamespaceID.IsValid(lastMapID))
            {
                //TODO
            }
            else
            {
                Dispose();
                await Main.LevelManager.ExitLevelSceneAsync();
                Main.Scene.DisplayPage(MainScenePageType.Mainmenu);
            }
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
            Main.LevelManager.RemoveLevelState(StartStageID);
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
            var levelUI = GetLevelUI();
            standaloneUI.SetActive(standaloneUI == levelUI);
            mobileUI.SetActive(mobileUI == levelUI);

            levelUI.OnStartGameCalled += StartGame;

            Awake_Blueprints();
            Awake_UI();
        }
        private void Update()
        {
            float gameSpeed = 0;
            if (isGameOver)
            {
                if (killerEntity)
                {
                    killerEntity.UpdateMovement();
                }
            }
            else if (!IsGameRunning())
            {
                foreach (var entity in entities.Where(e => CanUpdateBeforeGameStart(e.Entity)).ToArray())
                {
                    entity.UpdateMovement();
                }
            }
            else
            {
                gameSpeed = GetGameSpeed();
                var deltaTime = Time.deltaTime * gameSpeed;
                foreach (var entity in entities)
                {
                    entity.UpdateMovement();
                }
                AdvanceLevelProgress();

                var levelUI = GetLevelUI();
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
                levelUI.SetHeldItemPosition(heldItemPosition);
                UpdateLevelUI(gameSpeed);

                levelCamera.ShakeOffset = (Vector3)Main.ShakeManager.GetShake2D();
            }
            UpdateGridHighlight();
            UpdateInput();
        }
        private void FixedUpdate()
        {
            if (isGameOver)
            {
                if (killerEntity)
                {
                    var pos = killerEntity.Entity.Pos;
                    pos.x -= 1;
                    pos.z = pos.z * 0.5f + level.GetDoorZ() * 0.5f;
                    pos.y = pos.y * 0.5f + level.GetGroundY(pos.x, pos.z) * 0.5f;
                    killerEntity.Entity.Pos = pos;
                    killerEntity.UpdateModel(Time.fixedDeltaTime, 1);
                }
                return;
            }

            if (!IsGameRunning())
            {
                foreach (var entity in entities.ToArray())
                {
                    bool modelActive = false;
                    if (CanUpdateBeforeGameStart(entity.Entity))
                    {
                        entity.Entity.Update();
                        modelActive = true;
                    }
                    else if (entity.Entity.IsPreviewEnemy())
                    {
                        modelActive = true;
                    }
                    float deltaTime = modelActive ? Time.fixedDeltaTime : 0;
                    float simulationSpeed = modelActive ? 1 : 0;
                    entity.UpdateModel(deltaTime, simulationSpeed);
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
                    level.Update();
                    foreach (var entity in entities.ToArray())
                    {
                        entity.UpdateModel(Time.fixedDeltaTime, gameSpeed);
                    }
                    UpdateEnemyCry();
                }
            }
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
        private void PostHugeWaveApproachCallback(LevelEngine level)
        {
            var ui = GetLevelUI();
            ui.SetHugeWaveTextVisible(true);
        }
        private void PostFinalWaveCallback(LevelEngine level)
        {
            var ui = GetLevelUI();
            ui.SetFinalWaveTextVisible(true);
        }

        #endregion

        #region 暂停
        private void Pause()
        {
            isPaused = true;
            Main.MusicManager.Pause();
        }
        private void Resume()
        {
            isPaused = false;
            Main.MusicManager.Resume();

            if (optionsLogic != null)
            {
                optionsLogic.Dispose();
                optionsLogic = null;
            }
            var levelUI = GetLevelUI();
            levelUI.SetPauseDialogActive(false);
            levelUI.SetOptionsDialogActive(false);
            levelUI.SetLevelLoadedDialogVisible(false);
            levelLoaded = false;
        }
        private void UpdateFocusLost(bool focus)
        {
            if (isPaused)
                return;
            if (!IsGameStarted())
                return;
            if (focus)
                return;
            if (!Main.OptionsManager.GetPauseOnFocusLost())
                return;

            Pause();
            ShowPausedDialog();
        }
        #endregion

        #region 游戏结束
        private void SetGameOver()
        {
            isGameOver = true;
            level.PlaySound(SoundID.loseMusic);
            view.SetDoorVisible(false);
            level.HideAdvice();
            SetUIVisibleState(VisibleState.Nothing);
        }
        #endregion

        private void SwitchSpeedUp()
        {
            speedUp = !speedUp;
            GetLevelUI().SetSpeedUp(speedUp);
            level.PlaySound(speedUp ? SoundID.fastForward : SoundID.slowDown);
        }

        private void UpdateInput()
        {
            UpdatePointer();
            UpdateKeys();
        }
        private void UpdatePointer()
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
        private T GetRaycastComponent<T>(Vector2 screenPosition) where T : Component
        {
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            raycaster.Raycast(new PointerEventData(EventSystem.current) { position = screenPosition }, raycastResults);
            foreach (var raycastResult in raycastResults)
            {
                if (!raycastResult.isValid)
                    continue;
                var grid = raycastResult.gameObject.GetComponentInParent<T>();
                if (grid)
                    return grid;
            }
            return null;
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
                foreach (var enemy in level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsEnemy(SelfFaction) && !e.IsDead))
                {
                    enemy.Die();
                }
            }
            if (isGameStarted && !isGameOver)
            {
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    Main.LevelManager.SaveLevel();
                    Debug.Log("Game Saved!");
                }
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    Debug.Log("Restarting Game...");
                    _ = ReloadLevel();
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
                            level.PlaySound(SoundID.pause);
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
                            level.PlaySound(SoundID.pause);
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
                        level.PlaySound(SoundID.tap);
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
            }
        }
        private bool CanUpdateBeforeGameStart(Entity entity)
        {
            return entity.Type == EntityTypes.CART && entity.State == EntityStates.IDLE;
        }
        #endregion

        #region 属性字段
        public float LawnToTransScale => 1 / transToLawnScale;
        public float TransToLawnScale => transToLawnScale;
        public Game Game => Main.Game;
        private MainManager Main => MainManager.Instance;
        private LevelEngine level;
        private bool isPaused = false;
        private bool levelLoaded = true;
        private bool isGameStarted;
        private bool isGameOver;
        private bool speedUp;
        private float gameRunTimeModular;
        private NamespaceID killerID;
        private EntityController killerEntity;
        private string deathMessage;

        #region 保存属性
        public NamespaceID StartAreaID { get; set; }
        public NamespaceID StartStageID { get; set; }
        public NamespaceID CurrentMusic
        {
            get => Main.MusicManager.GetCurrentMusicID();
            set => Main.MusicManager.Play(value);
        }
        public float MusicTime
        {
            get => Main.MusicManager.Time;
            set => Main.MusicManager.Time = value;
        }
        #endregion

        [Header("Main")]
        [SerializeField]
        private LevelView view;
        [SerializeField]
        private float transToLawnScale = 100;
        #endregion
    }
}
