using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net.Core;
using MVZ2.GameContent;
using MVZ2.Level.UI;
using MVZ2.Talk;
using MVZ2.UI;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Game;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace MVZ2.Level
{
    using LevelEngine = PVZEngine.Level.LevelEngine;
    public class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法
        public void InitGame(Game game, NamespaceID areaID, NamespaceID stageID)
        {
            level = new LevelEngine(game, game);

            level.AddComponent(new AdviceComponent(level, this));
            level.AddComponent(new HeldItemComponent(level, this));
            level.AddComponent(new UIComponent(level, this));
            level.AddComponent(new LogicComponent(level, this));
            level.AddComponent(new SoundComponent(level, this));
            level.AddComponent(new TalkComponent(level, this));
            level.AddComponent(new MusicComponent(level, this));

            level.OnEntitySpawn += Engine_OnEntitySpawnCallback;
            level.OnEntityRemove += Engine_OnEntityRemoveCallback;
            level.OnGameOver += Engine_OnGameOverCallback;

            level.OnSeedPackChanged += Engine_OnSeedPackChangedCallback;
            level.OnSeedPackCountChanged += Engine_OnSeedPackCountChangedCallback;

            game.SetLevel(level);

            BuiltinCallbacks.PostHugeWaveApproach.Add(PostHugeWaveApproachCallback);
            BuiltinCallbacks.PostFinalWave.Add(PostFinalWaveCallback);

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
            StartAreaID = areaID;
            StartStageID = stageID;

            var startTalk = level.GetStartTalk();
            if (startTalk != null)
            {
                main.MusicManager.Play(MusicID.mainmenu);
                StartTalk(startTalk, 0, 2);
            }
            else
            {
                BeginLevel();
            }
        }
        public void Dispose()
        {
            if (optionsLogic != null)
            {
                optionsLogic.Dispose();
                optionsLogic = null;
            }
        }
        public void PlayReadySetBuild()
        {
            var ui = GetLevelUI();
            ui.SetReadySetBuildVisible(true);
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

            main.MusicManager.Play(level.GetMusicID());

            levelProgress = 0;
            bannerProgresses = new float[level.GetTotalFlags()];

            level.SetDifficulty(MainManager.OptionsManager.GetDifficulty());

            //level.SetEnergy(9990);
            //level.RechargeSpeed = 99;

            var levelUI = GetLevelUI();
            SetUIVisibleState(VisibleState.InLevel);

            UpdateBlueprints();
            UpdateLevelName();
            UpdateDifficultyName();

            level.Start();

            isGameStarted = true;
            UpdateFocusLost(Application.isFocused);
        }
        public float GetGameSpeed()
        {
            return speedUp ? 2 : 1;
        }
        public async Task RestartLevel()
        {
            var levelUI = GetLevelUI();
            levelUI.SetGameOverDialogInteractable(false);
            await main.LevelManager.GotoLevelSceneAsync();
            main.LevelManager.StartLevel(StartAreaID, StartStageID);
        }
        public bool IsGameRunning()
        {
            return isGameStarted && !isPaused && !isGameOver;
        }
        public bool IsGameStarted()
        {
            return isGameStarted;
        }
        public bool IsGameOver()
        {
            return isGameOver;
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
        public void StopLevel()
        {
            SetUIVisibleState(VisibleState.Nothing);
            isGameStarted = false;
        }
        public int GetCurrentFlag()
        {
            return level.CurrentFlag;
        }
        public EntityController GetEntityController(Entity entity)
        {
            return entities.FirstOrDefault(e => e.Entity == entity);
        }
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

        #region 对话
        public void StartTalk(NamespaceID groupId, int section, float delay = 0)
        {
            talkController.StartTalk(groupId, section, delay);
        }
        #endregion

        #region 手持物品
        public void SetHeldItemUI(NamespaceID heldType, int id, int priority, bool noCancel)
        {
            var ui = GetLevelUI();
            Sprite icon = null;
            LayerMask layerMask = Layers.GetMask(Layers.DEFAULT, Layers.PICKUP);
            if (heldType == HeldTypes.blueprint)
            {
                icon = GetHeldItemIcon(id);
                layerMask = Layers.GetMask(Layers.GRID, Layers.RAYCAST_RECEIVER);
            }
            else if (heldType == HeldTypes.pickaxe)
            {
                icon = main.LanguageManager.GetSprite(SpritePaths.pickaxe);
                layerMask = Layers.GetMask(Layers.DEFAULT, Layers.RAYCAST_RECEIVER);
            }
            else if (heldType == HeldTypes.starshard)
            {
                icon = main.LanguageManager.GetSprite(SpritePaths.GetStarshardIcon(level.AreaDefinition.GetID()));
                layerMask = Layers.GetMask(Layers.DEFAULT, Layers.RAYCAST_RECEIVER);
            }
            ui.SetHeldItemIcon(icon);
            ui.SetRaycasterMask(layerMask);
            raycaster.eventMask = layerMask;
        }
        #endregion

        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            gridLayout.OnPointerEnter += OnGridEnterCallback;
            gridLayout.OnPointerExit += OnGridExitCallback;
            gridLayout.OnPointerDown += OnGridPointerDownCallback;

            talkController.OnTalkAction += OnTalkActionCallback;
            talkController.OnTalkEnd += OnTalkEndCallback;

            levelCamera.SetPosition(cameraHousePosition, cameraHouseAnchor);

            HideGridSprites();
            var levelUI = GetLevelUI();
            standaloneUI.SetActive(standaloneUI == levelUI);
            mobileUI.SetActive(mobileUI == levelUI);

            levelUI.OnBlueprintPointerEnter += UI_OnBlueprintPointerEnterCallback;
            levelUI.OnBlueprintPointerExit += UI_OnBlueprintPointerExitCallback;
            levelUI.OnBlueprintPointerDown += UI_OnBlueprintPointerDownCallback;
            levelUI.OnPickaxePointerEnter += UI_OnPickaxePointerEnterCallback;
            levelUI.OnPickaxePointerExit += UI_OnPickaxePointerExitCallback;
            levelUI.OnPickaxePointerDown += UI_OnPickaxePointerDownCallback;
            levelUI.OnRaycastReceiverPointerDown += OnRaycastReceiverPointerDownCallback;
            levelUI.OnMenuButtonClick += OnMenuButtonClickCallback;
            levelUI.OnSpeedUpButtonClick += OnSpeedUpButtonClickCallback;
            levelUI.OnStarshardPointerDown += OnStarshardPointerDownCallback;
            levelUI.OnStartGameCalled += StartGame;
            levelUI.OnPauseDialogResumeClicked += OnPauseDialogResumeClickedCallback;

            levelUI.OnGameOverRetryButtonClicked += OnGameOverRetryButtonClickedCallback;
            levelUI.OnGameOverBackButtonClicked += OnGameOverBackButtonClickedCallback;

            levelUI.SetHeldItemIcon(null);
            levelUI.HideMoney();
            levelUI.SetProgressVisible(false);
            levelUI.SetHugeWaveTextVisible(false);
            levelUI.SetFinalWaveTextVisible(false);
            levelUI.SetReadySetBuildVisible(false);
            levelUI.SetPauseDialogActive(false);
            levelUI.SetOptionsDialogActive(false);
            levelUI.SetGameOverDialogActive(false);
            levelUI.SetYouDiedVisible(false);
            levelUI.HideTooltip();
            SetUIVisibleState(VisibleState.Nothing);
        }
        private void Update()
        {
            if (isGameOver)
            {
                if (killerEntity)
                {
                    killerEntity.UpdateMovement(Time.deltaTime);
                }
            }
            else if (!IsGameRunning())
            {
                foreach (var entity in entities.Where(e => CanUpdateBeforeGameStart(e.Entity)).ToArray())
                {
                    entity.UpdateMovement(Time.deltaTime);
                }
            }
            else
            {
                var gameSpeed = GetGameSpeed();
                var deltaTime = Time.deltaTime * gameSpeed;
                foreach (var entity in entities)
                {
                    entity.UpdateMovement(deltaTime);
                }
                var ui = GetLevelUI();
                ui.SetHeldItemPosition(levelCamera.Camera.ScreenToWorldPoint(Input.mousePosition));
                ui.SetEnergy(Mathf.FloorToInt(Mathf.Max(0, level.Energy - level.GetDelayedEnergy())).ToString());
                ui.SetPickaxeVisible(!level.IsHoldingPickaxe());
                ui.SetLevelTextAnimationSpeed(gameSpeed);
                UpdateLevelProgress();
                UpdateBlueprintRecharges();
                UpdateBlueprintDisabled();
                UpdateBlueprintTwinkle();
                UpdateStarshards();

                levelCamera.ShakeOffset = (Vector3)main.ShakeManager.GetShake2D();
            }
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
            else if (!IsGameRunning())
            {
                foreach (var entity in entities.ToArray())
                {
                    if (CanUpdateBeforeGameStart(entity.Entity))
                    {
                        entity.Entity.Update();
                        entity.UpdateModel(Time.fixedDeltaTime, 1);
                    }
                    else if (entity.Entity.IsPreviewEnemy())
                    {
                        entity.UpdateModel(Time.fixedDeltaTime, 1);
                    }
                }
                return;
            }

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
        private void OnApplicationFocus(bool focus)
        {
            UpdateFocusLost(focus);
        }
        #endregion

        #region 事件回调

        #region 逻辑方
        private void Engine_OnEntitySpawnCallback(Entity entity)
        {
            var entityController = Instantiate(entityTemplate.gameObject, LawnToTrans(entity.Pos), Quaternion.identity, entitiesRoot).GetComponent<EntityController>();
            entityController.Init(this, entity);
            entityController.OnPointerEnter += OnEntityPointerEnterCallback;
            entityController.OnPointerExit += OnEntityPointerExitCallback;
            entityController.OnPointerDown += OnEntityPointerDownCallback;
            entities.Add(entityController);
        }
        private void Engine_OnEntityRemoveCallback(Entity entity)
        {
            var entityController = GetEntityController(entity);
            if (entityController)
            {
                entityController.OnPointerEnter -= OnEntityPointerEnterCallback;
                entityController.OnPointerExit -= OnEntityPointerExitCallback;
                entityController.OnPointerDown -= OnEntityPointerDownCallback;
                Destroy(entityController.gameObject);
                entities.Remove(entityController);
            }
        }
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
        private void Engine_OnSeedPackChangedCallback(int index)
        {
            UpdateBlueprint(index);
        }
        private void Engine_OnSeedPackCountChangedCallback(int count)
        {
            UpdateBlueprints();
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

        #region UI方
        private void OnEntityPointerEnterCallback(EntityController entity, PointerEventData eventData)
        {
            entity.SetHovered(true);
            if (!IsGameRunning())
                return;
            if (Input.GetMouseButton((int)MouseButton.LeftMouse))
            {
                level.HoverOnEntity(entity.Entity);
            }
        }
        private void OnEntityPointerExitCallback(EntityController entity, PointerEventData eventData)
        {
            entity.SetHovered(false);
        }
        private void OnEntityPointerDownCallback(EntityController entityCtrl, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsGameRunning())
                return;

            var entity = entityCtrl.Entity;
            if (!level.IsEntityValidForHeldItem(entity))
                return;

            if (level.UseOnEntity(entity))
            {
                level.ResetHeldItem();
            }
        }
        private void OnGridEnterCallback(int lane, int column, PointerEventData data)
        {
            if (!IsGameRunning())
                return;
            var grid = gridLayout.GetGrid(lane, column);
            var color = Color.clear;
            if (level.IsHoldingItem())
            {
                color = level.IsGridValidForHeldItem(level.GetGrid(column, lane)) ? Color.green : Color.red;
            }
            grid.SetColor(color);
        }
        private void OnGridExitCallback(int lane, int column, PointerEventData data)
        {
            if (!IsGameRunning())
                return;
            var grid = gridLayout.GetGrid(lane, column);
            grid.SetColor(Color.clear);
        }
        private void OnGridPointerDownCallback(int lane, int column, PointerEventData data)
        {
            if (!IsGameRunning())
                return;
            if (data.button != PointerEventData.InputButton.Left)
                return;
            var grid = level.GetGrid(column, lane);
            if (!level.IsGridValidForHeldItem(grid))
                return;
            if (level.UseOnGrid(grid))
            {
                level.ResetHeldItem();
            }
        }
        private void OnTalkActionCallback(string cmd, string[] parameters)
        {
            BuiltinCallbacks.TalkAction.RunFiltered(cmd, talkController, cmd, parameters);
        }
        private void OnTalkEndCallback(NamespaceID mode)
        {
            var talkEndDefinition = Game.GetTalkEndDefinition(mode);
            if (talkEndDefinition != null)
            {
                talkEndDefinition.Execute(level);
            }
            else
            {
                if (level.IsCleared)
                {
                    ExitLevel();
                }
                else
                {
                    level.BeginLevel(LevelTransitions.DEFAULT);
                }
            }
        }
        private void UI_OnBlueprintPointerEnterCallback(int index, PointerEventData eventData)
        {
            var levelUI = GetLevelUI();
            var seedPack = level.GetSeedPackAt(index);
            if (seedPack == null)
                return;
            var seedDef = seedPack.Definition;
            if (seedDef == null || seedDef.GetSeedType() != SeedTypes.ENTITY)
                return;
            var entityID = seedDef.GetSeedEntityID();
            var name = main.ResourceManager.GetEntityName(entityID);
            var tooltip = main.ResourceManager.GetEntityTooltip(entityID);
            string error = null;
            if (!CanPickBlueprint(seedPack, out var errorMessage))
            {
                error = main.LanguageManager._(errorMessage);
            }
            var viewData = new TooltipViewData()
            {
                name = name,
                error = error,
                description = tooltip
            };
            levelUI.ShowTooltipOnBlueprint(index, viewData);
        }
        private void UI_OnBlueprintPointerExitCallback(int index, PointerEventData eventData)
        {
            var levelUI = GetLevelUI();
            levelUI.HideTooltip();
        }
        private void UI_OnBlueprintPointerDownCallback(int index, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            ClickBlueprint(index);
        }
        private void OnRaycastReceiverPointerDownCallback(LevelUI.Receiver receiver, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsGameRunning())
                return;

            LawnArea area = LawnArea.Main;
            switch (receiver)
            {
                case LevelUI.Receiver.Side:
                    area = LawnArea.Side;
                    break;
                case LevelUI.Receiver.Bottom:
                    area = LawnArea.Bottom;
                    break;
            }
            level.UseOnLawn(area);
        }
        private void UI_OnPickaxePointerEnterCallback(PointerEventData eventData)
        {
            var levelUI = GetLevelUI();
            var viewData = new TooltipViewData()
            {
                name = MainManager.LanguageManager._(StringTable.TOOLTIP_DIG_CONTRAPTION),
                error = level.IsPickaxeDisabled() ? MainManager.LanguageManager._(level.GetPickaxeDisableMessage()) : null,
                description = null
            };
            levelUI.ShowTooltipOnPickaxe(viewData);
        }
        private void UI_OnPickaxePointerExitCallback(PointerEventData eventData)
        {
            var levelUI = GetLevelUI();
            levelUI.HideTooltip();
        }
        private void UI_OnPickaxePointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            ClickPickaxe();
        }
        private void OnMenuButtonClickCallback()
        {
            SetGamePaused(true);
            level.PlaySound(SoundID.pause);
            ShowOptionsDialog();
        }
        private void OnOptionsMenuCloseCallback()
        {
            SetGamePaused(false);
        }
        private void OnSpeedUpButtonClickCallback()
        {
            SwitchSpeedUp();
        }
        private void OnStarshardPointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            ClickStarshard();
        }
        private void OnPauseDialogResumeClickedCallback()
        {
            SetGamePaused(false);
        }
        #region 游戏结束
        private async void OnGameOverRetryButtonClickedCallback()
        {
            await RestartLevel();
        }
        private async void OnGameOverBackButtonClickedCallback()
        {
            await RestartLevel();
        }
        #endregion

        #endregion

        #endregion
        private void ClickPickaxe()
        {
            if (level.IsPickaxeDisabled())
                return;
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(SoundID.tap);
                }
                return;
            }
            level.PlaySound(SoundID.pickaxe);
            level.SetHeldItem(HeldTypes.pickaxe, 0, 0);
        }
        private void ClickStarshard()
        {
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(SoundID.tap);
                }
                return;
            }
            if (level.GetStarshardCount() <= 0)
            {
                level.PlaySound(SoundID.buzzer);
                //return;
            }
            level.SetHeldItem(HeldTypes.starshard, 0, 0);
        }

        #region 蓝图
        private void UpdateBlueprints()
        {
            var count = level.GetSeedPackCount();
            var levelUI = GetLevelUI();
            levelUI.SetBlueprintCount(count);
        }
        private void UpdateBlueprint(int index)
        {
            var seed = level.GetSeedPackAt(index);
            BlueprintViewData viewData = GetSeedPackViewData(seed);
            var levelUI = GetLevelUI();
            levelUI.SetBlueprintAt(index, viewData);
        }
        private BlueprintViewData GetSeedPackViewData(SeedPack seed)
        {
            BlueprintViewData viewData;
            if (seed == null)
            {
                viewData = new BlueprintViewData()
                {
                    empty = true,
                };
            }
            else
            {
                var seedDef = seed.Definition;
                var sprite = GetBlueprintIcon(seedDef);
                viewData = new BlueprintViewData()
                {
                    icon = sprite,
                    cost = seed.GetCost().ToString(),
                    triggerActive = seedDef.IsTriggerActive(),
                    triggerCost = seedDef.GetTriggerCost().ToString(),
                };
            }
            return viewData;
        }
        private void UpdateBlueprintRecharges()
        {
            var levelUI = GetLevelUI();
            var seeds = level.GetAllSeedPacks();
            for (int i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                if (seed == null)
                    continue;
                var maxCharge = seed.GetMaxRecharge();
                levelUI.SetBlueprintRecharge(i, maxCharge == 0 ? 0 : 1 - seed.GetRecharge() / maxCharge);
            }
        }
        private void UpdateBlueprintDisabled()
        {
            var levelUI = GetLevelUI();
            var seeds = level.GetAllSeedPacks();
            for (int i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                if (seed == null)
                    continue;
                levelUI.SetBlueprintDisabled(i, level.IsHoldingBlueprint(i) || !CanPickBlueprint(seed));
            }
        }
        private void UpdateBlueprintTwinkle()
        {
            var levelUI = GetLevelUI();
            var seeds = level.GetAllSeedPacks();
            for (int i = 0; i < seeds.Length; i++)
            {
                var seed = seeds[i];
                if (seed == null)
                    continue;
                levelUI.SetBlueprintTwinkle(i, seed.IsTwinkling());
            }
        }
        private void UpdateStarshards()
        {
            var levelUI = GetLevelUI();
            levelUI.SetStarshardCount(level.GetStarshardCount(), 3);
        }
        private bool CanPickBlueprint(SeedPack seed)
        {
            return CanPickBlueprint(seed, out _);
        }
        private bool CanPickBlueprint(SeedPack seed, out string errorMessage)
        {
            if (seed == null)
            {
                errorMessage = null;
                return false;
            }
            if (!seed.IsCharged())
            {
                errorMessage = StringTable.TOOLTIP_RECHARGING;
                return false;
            }
            if (level.Energy < seed.GetCost())
            {
                errorMessage = StringTable.TOOLTIP_NOT_ENOUGH_ENERGY;
                return false;
            }
            if (seed.IsDisabled())
            {
                errorMessage = seed.GetDisableMessage();
                return false;
            }
            errorMessage = null;
            return true;
        }
        private Sprite GetBlueprintIcon(int i)
        {
            var seeds = level.GetAllSeedPacks();
            var seed = seeds[i];
            return GetBlueprintIcon(seed);
        }
        private Sprite GetBlueprintIcon(SeedPack seed)
        {
            if (seed == null)
                return null;
            var seedDef = seed.Definition;
            return GetBlueprintIcon(seedDef);
        }
        private Sprite GetBlueprintIcon(SeedDefinition seedDef)
        {
            if (seedDef == null)
                return null;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                if (MainManager.IsMobile())
                {
                    return MainManager.ResourceManager.GetSprite(entityID.spacename, $"mobile_blueprint/{entityID.path}");
                }
            }
            return GetHeldItemIcon(seedDef);
        }
        private Sprite GetHeldItemIcon(int i)
        {
            var seeds = level.GetAllSeedPacks();
            var seed = seeds[i];
            return GetHeldItemIcon(seed);
        }
        private Sprite GetHeldItemIcon(SeedPack seed)
        {
            if (seed == null)
                return null;
            var seedDef = seed.Definition;
            return GetHeldItemIcon(seedDef);
        }
        private Sprite GetHeldItemIcon(SeedDefinition seedDef)
        {
            if (seedDef == null)
                return null;
            Sprite sprite = null;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                var modelID = entityID.ToModelID(ModelID.TYPE_ENTITY);
                sprite = MainManager.ResourceManager.GetModelIcon(modelID);
            }
            return sprite;
        }
        private void ClickBlueprint(int index)
        {
            if (level.IsHoldingItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(SoundID.tap);
                }
                return;
            }
            if (!CanPickBlueprint(level.GetSeedPackAt(index)))
            {
                level.PlaySound(SoundID.buzzer);
                return;
            }
            level.PlaySound(SoundID.pick);
            SelectBlueprint(index);
        }
        private void SelectBlueprint(int index)
        {
            level.SetHeldItem(HeldTypes.blueprint, index, 0);
        }
        #endregion

        #region 暂停
        private void SetGamePaused(bool paused)
        {
            isPaused = paused;
            if (isPaused)
            {
                main.MusicManager.Pause();
            }
            else
            {
                main.MusicManager.Resume();

                if (optionsLogic != null)
                {
                    optionsLogic.Dispose();
                    optionsLogic = null;
                }
                var levelUI = GetLevelUI();
                levelUI.SetPauseDialogActive(false);
                levelUI.SetOptionsDialogActive(false);
            }
        }
        private void UpdateFocusLost(bool focus)
        {
            if (isPaused)
                return;
            if (!IsGameStarted())
                return;
            if (focus)
                return;
            if (!main.OptionsManager.GetPauseOnFocusLost())
                return;

            SetGamePaused(true);
            ShowPausedDialog();
        }
        #endregion
        public LevelUI GetLevelUI()
        {
            return MainManager.IsMobile() ? mobileUI : standaloneUI;
        }
        private void ShowOptionsDialog()
        {
            var levelUI = GetLevelUI();
            levelUI.SetOptionsDialogActive(true);

            optionsLogic = new OptionsLogicLevel(levelUI.OptionsDialog, this);
            optionsLogic.InitDialog();
            optionsLogic.OnClose += OnOptionsMenuCloseCallback;
            levelUI.ResetPauseDialogPosition();
        }
        private void ShowPausedDialog()
        {
            var levelUI = GetLevelUI();
            var spriteReference = pauseImages.Random(uiRandom);
            levelUI.SetPauseDialogActive(isPaused);
            levelUI.SetPauseDialogImage(main.LanguageManager.GetSprite(spriteReference));
            levelUI.ResetPauseDialogPosition();
        }
        private void HideGridSprites()
        {
            foreach (var grid in gridLayout.GetGrids())
            {
                grid.SetColor(Color.clear);
            }
        }
        private void UpdateLevelProgress()
        {
            var ui = GetLevelUI();
            var deltaTime = Time.deltaTime;
            var totalFlags = level.GetTotalFlags();
            if (bannerProgresses == null || bannerProgresses.Length != totalFlags)
            {
                var newProgresses = new float[totalFlags];
                if (bannerProgresses != null)
                {
                    bannerProgresses.CopyTo(newProgresses, 0);
                }
                bannerProgresses = newProgresses;
            }
            for (int i = 0; i < bannerProgresses.Length; i++)
            {
                bannerProgresses[i] = Mathf.Clamp01(bannerProgresses[i] + level.CurrentWave >= i * level.GetWavesPerFlag() ? deltaTime : -deltaTime);
            }
            int totalWaveCount = level.GetTotalWaveCount();
            float targetProgress = totalWaveCount <= 0 ? 0 : level.CurrentWave / (float)totalWaveCount;
            int progressDirection = Math.Sign(targetProgress - levelProgress);
            if (progressDirection != 0)
            {
                levelProgress += Time.deltaTime * 0.1f * progressDirection;
                var newDirection = Mathf.Sign(targetProgress - levelProgress);
                if (progressDirection != newDirection)
                {
                    levelProgress = targetProgress;
                }
            }
            ui.SetProgressVisible(level.LevelProgressVisible);
            ui.SetProgress(levelProgress);
            ui.SetBannerProgresses(bannerProgresses);
        }
        private void UpdateLevelName()
        {
            string name = level.GetLevelName();
            if (string.IsNullOrEmpty(name))
            {
                name = StringTable.LEVEL_NAME_UNKNOWN;
            }
            var levelUI = GetLevelUI();
            var levelName = main.LanguageManager._p(StringTable.CONTEXT_LEVEL_NAME, name);
            levelUI.SetLevelName(levelName);
        }
        private void UpdateDifficultyName()
        {
            var difficultyName = main.ResourceManager.GetDifficultyName(level.Difficulty);
            var levelUI = GetLevelUI();
            levelUI.SetDifficulty(difficultyName);
        }
        private void SwitchSpeedUp()
        {
            speedUp = !speedUp;
            GetLevelUI().SetSpeedUp(speedUp);
            level.PlaySound(speedUp ? SoundID.fastForward : SoundID.slowDown);
        }
        private void SetUIVisibleState(VisibleState state)
        {
            var levelUI = GetLevelUI();
            var toolsVisible = state == VisibleState.InLevel || state == VisibleState.ChoosingBlueprints;
            levelUI.SetEnergyVisible(toolsVisible);
            levelUI.SetBlueprintsVisible(toolsVisible);
            levelUI.SetPickaxeSlotVisible(toolsVisible);
            levelUI.SetTopRightVisible(toolsVisible);
            levelUI.SetTriggerSlotVisible(toolsVisible && main.SaveManager.IsTriggerUnlocked());

            var inlevelVisible = state == VisibleState.InLevel;
            levelUI.SetStarshardVisible(inlevelVisible && main.SaveManager.IsStarshardUnlocked());
            levelUI.SetSpeedUpVisible(inlevelVisible);
            levelUI.SetLevelNameVisible(inlevelVisible);
        }
        private void UpdateInput()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
            {
                foreach (var enemy in level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsEnemy(SelfFaction) && !e.IsDead))
                {
                    enemy.Die();
                }
            }
#endif
            if (!isGameOver)
            {
                if (isGameStarted)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        SetGamePaused(!isPaused);
                        if (isPaused)
                        {
                            level.PlaySound(SoundID.pause);
                            ShowPausedDialog();
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        SetGamePaused(!isPaused);
                        if (isPaused)
                        {
                            level.PlaySound(SoundID.pause);
                            ShowOptionsDialog();
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
        private void UpdateEnemyCry()
        {
            cryTimeCheckTimer.Run();
            if (cryTimeCheckTimer.Expired)
            {
                cryTimeCheckTimer.Reset();

                var crySoundEnemies = GetCrySoundEnemies();
                var enemyCount = crySoundEnemies.Count();
                float t = Mathf.Clamp01((float)(enemyCount - MinEnemyCryCount) / (MaxEnemyCryCount - MinEnemyCryCount));
                maxCryTime = (int)Mathf.Lerp(MaxCryInterval, MinCryInterval, t);
            }
            cryTimer.Run();
            if (cryTimer.MaxFrame - cryTimer.Frame >= maxCryTime)
            {
                cryTimer.Reset();

                var crySoundEnemies = GetCrySoundEnemies();
                var enemyCount = crySoundEnemies.Count();
                if (enemyCount <= 0)
                    return;
                var crySoundEnemy = crySoundEnemies.Random(uiRandom);
                level.PlaySound(crySoundEnemy.GetCrySound(), crySoundEnemy.Pos);
            }
        }
        private IEnumerable<Entity> GetCrySoundEnemies()
        {
            var enemies = level.GetEntities(EntityTypes.ENEMY);
            if (enemies.Length <= 0)
                return Enumerable.Empty<Entity>();
            return enemies.Where(e => e.GetCrySound() != null);
        }
        private bool CanUpdateBeforeGameStart(Entity entity)
        {
            return entity.Type == EntityTypes.CART && entity.State == EntityStates.IDLE;
        }
        private void ExitLevel()
        {
            // TODO
        }

        #region 游戏结束
        private void ShowGameOverDialog()
        {
            string message;
            if (killerID != null)
            {
                var entityDef = Game.GetEntityDefinition(killerID);
                message = entityDef.GetDeathMessage();
            }
            else
            {
                message = deathMessage;
            }
            var levelUI = GetLevelUI();
            levelUI.SetGameOverDialogActive(true);
            levelUI.SetGameOverDialogMessage(main.LanguageManager._p(StringTable.CONTEXT_DEATH_MESSAGE, message));
        }
        private void SetGameOver()
        {
            isGameOver = true;
            level.PlaySound(SoundID.loseMusic);
            view.SetDoorVisible(false);
            SetUIVisibleState(VisibleState.Nothing);
        }
        #endregion

        #region 场景过渡
        private IEnumerator MoveCameraLawn(Vector3 target, Vector2 targetAnchor, float maxTime)
        {
            float time = 0;
            Vector3 start = levelCamera.CameraPosition;
            Vector2 startAnchor = levelCamera.CameraAnchor;
            while (time < maxTime)
            {
                time = Mathf.Clamp(time + Time.deltaTime, 0, maxTime);
                var lerp = cameraMoveCurve.Evaluate(time / maxTime);
                var pos = Vector3.Lerp(start, target, lerp);
                var anchor = Vector2.Lerp(startAnchor, targetAnchor, lerp);
                levelCamera.SetPosition(pos, anchor);
                yield return null;
            }
        }
        private IEnumerator MoveCameraToHouse()
        {
            return MoveCameraLawn(cameraHousePosition, cameraHouseAnchor, 1f);
        }
        private IEnumerator MoveCameraToLawn()
        {
            return MoveCameraLawn(cameraLawnPosition, cameraLawnAnchor, 1f);
        }
        private IEnumerator MoveCameraToChoose()
        {
            return MoveCameraLawn(cameraChoosePosition, cameraChooseAnchor, 1f);
        }
        private IEnumerator GameStartToLawnTransition()
        {
            main.MusicManager.Play(MusicID.choosing);
            yield return new WaitForSeconds(1);
            yield return MoveCameraToLawn();
            yield return new WaitForSeconds(0.5f);
            StartGame();
        }
        private IEnumerator GameStartTransition()
        {
            main.MusicManager.Play(MusicID.choosing);
            if (level.StageDefinition is IPreviewStage preview)
            {
                preview.CreatePreviewEnemies(level, BuiltinLevel.GetEnemySpawnRect());
            }
            yield return new WaitForSeconds(1);
            yield return MoveCameraToChoose();
            yield return new WaitForSeconds(1);

            var seedPacks = MainManager.LevelManager.GetSeedPacksID();
            level.SetSeedPackCount(seedPacks.Length);
            level.ReplaceSeedPacks(seedPacks);

            yield return MoveCameraToLawn();
            level.PrepareForBattle();
            yield return new WaitForSeconds(0.5f);
            PlayReadySetBuild();
        }
        private IEnumerator GameOverByEnemyTransition()
        {
            main.MusicManager.Stop();
            yield return new WaitForSeconds(1);
            yield return MoveCameraToHouse();
            yield return new WaitForSeconds(3);
            level.PlaySound(SoundID.hit);
            yield return new WaitForSeconds(0.5f);
            level.PlaySound(SoundID.hit);
            yield return new WaitForSeconds(0.5f);
            level.PlaySound(SoundID.hit);
            yield return new WaitForSeconds(0.5f);
            level.PlaySound(SoundID.scream);
            var levelUI = GetLevelUI();
            levelUI.SetYouDiedVisible(true);
            yield return new WaitForSeconds(4);
            ShowGameOverDialog();
        }
        private IEnumerator GameOverNoEnemyTransition()
        {
            main.MusicManager.Stop();
            level.PlaySound(SoundID.scream);
            var levelUI = GetLevelUI();
            levelUI.SetYouDiedVisible(true);
            yield return new WaitForSeconds(4);
            ShowGameOverDialog();
        }
        #endregion

        #endregion

        #region 属性字段
        public const int SelfFaction = 0;
        public const int EnemyFaction = 1;
        public const int MinEnemyCryCount = 1;
        public const int MaxEnemyCryCount = 20;
        public const int MinCryInterval = 60;
        public const int MaxCryInterval = 300;
        public NamespaceID StartAreaID { get; private set; }
        public NamespaceID StartStageID { get; private set; }
        public float LawnToTransScale => 1 / transToLawnScale;
        public float TransToLawnScale => transToLawnScale;
        public MainManager MainManager => main;
        public Game Game => MainManager.Game;
        public float MusicTime
        {
            get => main.MusicManager.Time;
            set => main.MusicManager.Time = value;
        }
        private bool isPaused = false;
        private List<EntityController> entities = new List<EntityController>();
        private LevelEngine level;
        private MainManager main => MainManager.Instance;
        private bool isGameStarted;
        private bool isGameOver;
        private bool speedUp;
        private float gameRunTimeModular;
        private FrameTimer cryTimer = new FrameTimer(MaxCryInterval);
        private FrameTimer cryTimeCheckTimer = new FrameTimer(7);
        private int maxCryTime = MaxCryInterval;
        private RandomGenerator uiRandom = new RandomGenerator(Guid.NewGuid().GetHashCode());
        private NamespaceID killerID;
        private EntityController killerEntity;
        private string deathMessage;


        private float levelProgress;
        private float[] bannerProgresses;

        private OptionsLogicLevel optionsLogic;

        [SerializeField]
        private LevelView view;
        [SerializeField]
        private float transToLawnScale = 100;

        [Header("Cameras")]
        [SerializeField]
        private LevelCamera levelCamera;
        [SerializeField]
        private AnimationCurve cameraMoveCurve;
        [SerializeField]
        private Vector3 cameraHousePosition = new Vector3(0, 3, -10);
        [SerializeField]
        private Vector2 cameraHouseAnchor = new Vector2(0, 0.5f);
        [SerializeField]
        private Vector3 cameraLawnPosition = new Vector3(10.2f, 3, -10);
        [SerializeField]
        private Vector2 cameraLawnAnchor = new Vector2(1, 0.5f);
        [SerializeField]
        private Vector3 cameraChoosePosition = new Vector3(14, 3, -10);
        [SerializeField]
        private Vector2 cameraChooseAnchor = new Vector2(1, 0.5f);


        [SerializeField]
        private Physics2DRaycaster raycaster;
        [SerializeField]
        private EntityController entityTemplate;
        [SerializeField]
        private Transform entitiesRoot;
        [SerializeField]
        private GridLayoutController gridLayout;
        [SerializeField]
        private LevelUI standaloneUI;
        [SerializeField]
        private LevelUI mobileUI;
        [SerializeField]
        private List<SpriteReference> pauseImages = new List<SpriteReference>();
        [SerializeField]
        private TalkController talkController;
        #endregion

        #region 内嵌类
        public enum VisibleState
        {
            Nothing,
            ChoosingBlueprints,
            InLevel,
        }
        public enum CameraPosition
        {
            House,
            Lawn,
            Choose,
        }
        #endregion
    }
}
