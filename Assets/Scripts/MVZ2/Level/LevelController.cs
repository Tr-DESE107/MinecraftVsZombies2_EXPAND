using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net.Core;
using MVZ2.GameContent;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Stages;
using MVZ2.Level.UI;
using MVZ2.Talk;
using MVZ2.UI;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Game;
using PVZEngine.LevelManagement;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace MVZ2.Level
{
    using Level = PVZEngine.LevelManagement.Level;
    public class LevelController : MonoBehaviour
    {
        #region 公有方法
        public void InitGame(Game game)
        {
            var vanilla = new Vanilla.VanillaMod(game);
            level = new Level(game);
            level.OnEntitySpawn += OnEntitySpawnCallback;
            level.OnEntityRemove += OnEntityRemoveCallback;
            level.OnPlaySoundPosition += OnPlaySoundPositionCallback;
            level.OnPlaySound += OnPlaySoundCallback;
            level.OnShakeScreen += OnShakeScreenCallback;
            level.OnHeldItemChanged += OnHeldItemChangedCallback;
            level.OnGameOver += OnGameOverCallback;

            level.OnShowDialog += Logic_OnShowDialogCallback;
            level.OnShowMoney += Logic_OnShowMoneyCallback;
            level.OnBeginLevel += Logic_OnBeginLevelCallback;

            level.OnShowAdvice += Logic_OnShowAdviceCallback;
            level.OnHideAdvice += Logic_OnHideAdviceCallback;
            game.SetLevel(level);

            VanillaCallbacks.PostHugeWaveApproach.Add(PostHugeWaveApproachCallback);
            VanillaCallbacks.PostFinalWave.Add(PostFinalWaveCallback);

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
            level.Init(AreaID.day, StageID.prologue, option);

            level.Spawn<Miner>(new Vector3(600, 0, 60), null);

            var startTalk = level.GetStartTalk();
            if (startTalk != null)
            {
                talkController.StartTalk(startTalk, 0, 2);
            }
            else
            {
                BeginLevel();
            }

            level.SetSeedPacks(new NamespaceID[]
            {
                ContraptionID.dispenser,
                ContraptionID.furnace,
                ContraptionID.obsidian,
                ContraptionID.mineTNT,
                EnemyID.zombie,
                null
            });

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
            level.Start(LevelDifficulty.Normal);

            main.MusicManager.Play(level.GetMusicID());

            levelProgress = 0;
            bannerProgresses = new float[level.GetTotalFlags()];

            //level.SetEnergy(9990);
            //level.RechargeSpeed = 99;

            var levelUI = GetLevelUI();
            SetUIVisibleState(VisibleState.InLevel);
            levelUI.SetBlueprintCount(level.GetAllSeedPacks().Length);

            UpdateBlueprints();
            UpdateLevelName();
            UpdateDifficultyName();

            isGameStarted = true;
        }
        public bool IsEntityValidForHeldItem(Entity entity)
        {
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    switch (level.HeldItemType)
                    {
                        case HeldTypes.PICKAXE:
                            return CanDigContraption(entity);
                        case HeldTypes.STARSHARD:
                            return entity.CanEvoke();
                    }
                    break;
                case EntityTypes.PICKUP:
                    switch (level.HeldItemType)
                    {
                        case HeldTypes.NONE:
                            return !entity.IsCollected();
                    }
                    break;
                case EntityTypes.CART:
                    switch (level.HeldItemType)
                    {
                        case HeldTypes.NONE:
                            return !entity.IsCartTriggered();
                    }
                    break;
            }
            return false;
        }
        public float GetGameSpeed()
        {
            return speedUp ? 2 : 1;
        }
        public bool IsGameRunning()
        {
            return isGameStarted && !isPaused && !isGameOver;
        }
        public bool IsGameOver()
        {
            return isGameOver;
        }
        public void GameOver(Entity killer)
        {
            killerID = killer.Definition.GetID();
            killerEntity = entities.FirstOrDefault(e => e.Entity == killer);
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

            levelUI.OnBlueprintPointerDown += OnBlueprintPointerDownCallback;
            levelUI.OnRaycastReceiverPointerDown += OnRaycastReceiverPointerDownCallback;
            levelUI.OnPickaxePointerDown += OnPickaxePointerDownCallback;
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
            levelUI.SetGameOverDialogActive(false);
            levelUI.SetYouDiedVisible(false);
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
                ui.SetPickaxeVisible(!IsHoldingPickaxe());
                ui.SetLevelTextAnimationSpeed(gameSpeed);
                UpdateLevelProgress();
                UpdateBlueprintRecharges();
                UpdateBlueprintDisabled();
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
                level.Update();
                foreach (var entity in entities.ToArray())
                {
                    entity.UpdateModel(Time.fixedDeltaTime, gameSpeed);
                }
                UpdateEnemyCry();
            }
        }
        #endregion

        #region 事件回调

        #region 逻辑方
        private void OnEntitySpawnCallback(Entity entity)
        {
            var entityController = Instantiate(entityTemplate.gameObject, LawnToTrans(entity.Pos), Quaternion.identity, entitiesRoot).GetComponent<EntityController>();
            entityController.Init(this, entity);
            entityController.OnPointerEnter += OnEntityPointerEnterCallback;
            entityController.OnPointerExit += OnEntityPointerExitCallback;
            entityController.OnPointerDown += OnEntityPointerDownCallback;
            entities.Add(entityController);
        }
        private void OnEntityRemoveCallback(Entity entity)
        {
            var entityController = entities.FirstOrDefault(e => e.Entity == entity);
            if (entityController)
            {
                entityController.OnPointerEnter -= OnEntityPointerEnterCallback;
                entityController.OnPointerExit -= OnEntityPointerExitCallback;
                entityController.OnPointerDown -= OnEntityPointerDownCallback;
                Destroy(entityController.gameObject);
                entities.Remove(entityController);
            }
        }
        private void OnPlaySoundPositionCallback(NamespaceID soundID, Vector3 lawnPos, float pitch)
        {
            main.SoundManager.Play(soundID, LawnToTrans(lawnPos), pitch, 1);
        }
        private void OnPlaySoundCallback(NamespaceID soundID, float pitch)
        {
            main.SoundManager.Play(soundID, Vector3.zero, pitch, 0);
        }
        private void OnShakeScreenCallback(float startAmplitude, float endAmplitude, int time)
        {
            main.ShakeManager.AddShake(startAmplitude * LawnToTransScale, endAmplitude * LawnToTransScale, time / (float)level.TPS);
        }
        private void OnHeldItemChangedCallback(int heldType, int id, int priority, bool noCancel)
        {
            SetHeldItemUI(heldType, id, priority, noCancel);
        }
        private void OnGameOverCallback(int type, Entity killer, string message)
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
        private void Logic_OnShowDialogCallback(string title, string desc, string[] options, Action<int> onSelect)
        {
            main.Scene.ShowDialog(title, desc, options, onSelect);
        }
        private void Logic_OnShowMoneyCallback()
        {
            var levelUI = GetLevelUI();
            levelUI.ResetMoneyFadeTime();
        }
        private void Logic_OnBeginLevelCallback(string transition)
        {
            BeginLevel(transition);
        }
        private void Logic_OnShowAdviceCallback(string advice)
        {
            var ui = GetLevelUI();
            ui.ShowAdvice(advice);
        }
        private void Logic_OnHideAdviceCallback()
        {
            var ui = GetLevelUI();
            ui.HideAdvice();
        }
        private void PostHugeWaveApproachCallback(Level level)
        {
            var ui = GetLevelUI();
            ui.SetHugeWaveTextVisible(true);
        }
        private void PostFinalWaveCallback(Level level)
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
            switch (entity.Entity.Type)
            {
                case EntityTypes.PICKUP:
                    if (level.HeldItemType == HeldTypes.NONE && Input.GetMouseButton((int)MouseButton.LeftMouse))
                    {
                        var pickup = entity.Entity;
                        if (!pickup.IsCollected())
                            pickup.Collect();
                    }
                    break;
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
            if (!IsEntityValidForHeldItem(entity))
                return;

            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    switch (level.HeldItemType)
                    {
                        case HeldTypes.PICKAXE:
                            entity.Die();
                            level.ResetHeldItem();
                            break;
                        case HeldTypes.STARSHARD:
                            entity.Evoke();
                            level.ResetHeldItem();
                            break;
                    }
                    break;
                case EntityTypes.PICKUP:
                    if (level.HeldItemType == HeldTypes.NONE)
                    {
                        entity.Collect();
                    }
                    break;
                case EntityTypes.CART:
                    if (level.HeldItemType == HeldTypes.NONE)
                    {
                        entity.TriggerCart();
                    }
                    break;
            }
        }
        private void OnGridEnterCallback(int lane, int column, PointerEventData data)
        {
            var grid = gridLayout.GetGrid(lane, column);
            var color = Color.clear;
            if (level.HeldItemType > 0)
            {
                color = CanPlaceOnGrid(level.HeldItemType, level.HeldItemID, level.GetGrid(column, lane)) ? Color.green : Color.red;
            }
            grid.SetColor(color);
        }
        private void OnGridExitCallback(int lane, int column, PointerEventData data)
        {
            var grid = gridLayout.GetGrid(lane, column);
            grid.SetColor(Color.clear);
        }
        private void OnGridPointerDownCallback(int lane, int column, PointerEventData data)
        {
            if (data.button != PointerEventData.InputButton.Left)
                return;
            if (!CanPlaceOnGrid(level.HeldItemType, level.HeldItemID, level.GetGrid(column, lane)))
                return;
            switch (level.HeldItemType)
            {
                case HeldTypes.BLUEPRINT:
                    var seed = level.GetSeedPackAt(level.HeldItemID);
                    if (seed == null)
                        break;
                    var seedDef = seed.Definition;
                    if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                    {
                        var x = level.GetEntityColumnX(column);
                        var z = level.GetEntityLaneZ(lane);
                        var y = level.GetGroundY(x, z);
                        var position = new Vector3(x, y, z);
                        var entityID = seedDef.GetSeedEntityID();
                        var entityDef = Game.GetEntityDefinition(entityID);
                        level.Spawn(entityID, position, null);
                        level.AddEnergy(-seedDef.GetCost());
                        level.SetRechargeTimeToUsed(seed);
                        seed.ResetRecharge();
                        level.ResetHeldItem();
                        level.PlaySound(entityDef.GetPlaceSound(), position);
                    }
                    break;
            }
        }
        private void OnTalkActionCallback(string cmd, string[] parameters)
        {
            VanillaCallbacks.TalkAction.RunFiltered(cmd, talkController, cmd, parameters);
        }
        private void OnTalkEndCallback(string mode)
        {
            switch (mode)
            {
                case "start_tutorial":
                    level.ChangeStage(StageID.tutorial);
                    level.BeginLevel(LevelTransitions.TO_LAWN);
                    break;
                case "start_starshard_tutorial":
                    level.ChangeStage(StageID.starshard_tutorial);
                    level.BeginLevel(LevelTransitions.TO_LAWN);
                    break;
                case "start_trigger_tutorial":
                    level.ChangeStage(StageID.trigger_tutorial);
                    level.BeginLevel(LevelTransitions.TO_LAWN);
                    break;

                default:
                    if (level.IsCleared)
                    {
                        ExitLevel();
                    }
                    else
                    {
                        level.BeginLevel(LevelTransitions.DEFAULT);
                    }
                    break;
            }
        }
        private void OnBlueprintPointerDownCallback(int index, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            ClickBlueprint(index);
        }
        private void OnRaycastReceiverPointerDownCallback(LevelUI.Receiver receiver)
        {
            switch (receiver)
            {
                case LevelUI.Receiver.Side:
                    switch (level.HeldItemType)
                    {
                        case HeldTypes.BLUEPRINT:
                        case HeldTypes.PICKAXE:
                        case HeldTypes.STARSHARD:
                            if (CancelHeldItem())
                            {
                                level.PlaySound(SoundID.tap);
                            }
                            break;
                    }
                    break;
                case LevelUI.Receiver.Lawn:
                case LevelUI.Receiver.Bottom:
                    switch (level.HeldItemType)
                    {
                        case HeldTypes.PICKAXE:
                        case HeldTypes.STARSHARD:
                            CancelHeldItem();
                            break;
                    }
                    break;
            }
        }
        private void OnPickaxePointerDownCallback(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (level.IsPickaxeDisabled())
                return;
            ClickPickaxe();
        }
        private void OnMenuButtonClickCallback()
        {
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
            await Retry();
        }
        private async void OnGameOverBackButtonClickedCallback()
        {
            await Retry();
        }
        #endregion

        #endregion

        #endregion
        private void SetHeldItemUI(int heldType, int id, int priority, bool noCancel)
        {
            var ui = GetLevelUI();
            Sprite icon = null;
            LayerMask layerMask = Layers.GetMask(Layers.DEFAULT, Layers.PICKUP);
            switch (heldType)
            {
                case HeldTypes.BLUEPRINT:
                    icon = GetBlueprintIcon(id);
                    layerMask = Layers.GetMask(Layers.GRID, Layers.RAYCAST_RECEIVER);
                    break;
                case HeldTypes.PICKAXE:
                    icon = main.ResourceManager.GetSprite(SpritePaths.pickaxe);
                    layerMask = Layers.GetMask(Layers.DEFAULT, Layers.RAYCAST_RECEIVER);
                    break;
                case HeldTypes.STARSHARD:
                    icon = main.ResourceManager.GetSprite(SpritePaths.GetStarshardIcon(level.AreaDefinition.GetID()));
                    layerMask = Layers.GetMask(Layers.DEFAULT, Layers.RAYCAST_RECEIVER);
                    break;
            }
            ui.SetHeldItemIcon(icon);
            ui.SetRaycasterMask(layerMask);
            raycaster.eventMask = layerMask;
        }
        private bool CancelHeldItem()
        {
            if (level.HeldItemType <= 0 || level.HeldItemNoCancel)
                return false;
            level.ResetHeldItem();
            return true;
        }
        private bool CanDigContraption(Entity entity)
        {
            return entity.GetFaction() == SelfFaction;
        }
        private bool IsHoldingPickaxe()
        {
            return level.HeldItemType == HeldTypes.PICKAXE;
        }
        private bool IsHoldingStarshard()
        {
            return level.HeldItemType == HeldTypes.STARSHARD;
        }
        private void ClickPickaxe()
        {
            if (level.HeldItemType > 0)
            {
                if (CancelHeldItem())
                {
                    level.PlaySound(SoundID.tap);
                }
                return;
            }
            level.PlaySound(SoundID.pickaxe);
            level.SetHeldItem(HeldTypes.PICKAXE, 0, 0);
        }
        private void ClickStarshard()
        {
            if (level.HeldItemType > 0)
            {
                if (CancelHeldItem())
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
            level.SetHeldItem(HeldTypes.STARSHARD, 0, 0);
        }

        #region 蓝图
        private void UpdateBlueprints()
        {
            var levelUI = GetLevelUI();
            var seeds = level.GetAllSeedPacks(true);
            var viewDatas = new BlueprintViewData[seeds.Length];
            for (int i = 0; i < viewDatas.Length; i++)
            {
                var seed = seeds[i];
                var seedDef = seed.Definition;
                var sprite = GetBlueprintIcon(seedDef);
                var viewData = new BlueprintViewData()
                {
                    icon = sprite,
                    cost = seed.GetCost().ToString(),
                    triggerActive = seedDef.IsTriggerActive(),
                    triggerCost = seedDef.GetTriggerCost().ToString(),
                };
                viewDatas[i] = viewData;
            }
            levelUI.SetBlueprints(viewDatas);
        }
        private void UpdateBlueprintRecharges()
        {
            var levelUI = GetLevelUI();
            var seeds = level.GetAllSeedPacks(true);
            var recharges = new float[seeds.Length];
            for (int i = 0; i < recharges.Length; i++)
            {
                var seed = seeds[i];
                var maxCharge = seed.GetMaxRecharge();
                recharges[i] = maxCharge == 0 ? 1 : seed.GetRecharge() / maxCharge;
            }
            levelUI.SetBlueprintRecharges(recharges);
        }
        private void UpdateBlueprintDisabled()
        {
            var levelUI = GetLevelUI();
            var seeds = level.GetAllSeedPacks(true);
            var disabled = new bool[seeds.Length];
            for (int i = 0; i < disabled.Length; i++)
            {
                var seed = seeds[i];
                disabled[i] = IsHoldingBlueprint(i) || !CanPickBlueprint(seed);
            }
            levelUI.SetBlueprintDisabled(disabled);
        }
        private void UpdateStarshards()
        {
            var levelUI = GetLevelUI();
            levelUI.SetStarshardCount(level.GetStarshardCount(), 3);
        }
        private bool IsHoldingBlueprint(int i)
        {
            return level.HeldItemType == HeldTypes.BLUEPRINT && level.HeldItemID == i;
        }
        private bool CanPickBlueprint(SeedPack seed)
        {
            if (seed == null)
                return false;
            return level.Energy >= seed.GetCost() && seed.IsCharged() && !seed.IsDisabled();
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
            Sprite sprite = null;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var modelID = seedDef.GetSeedEntityID().ToModelID(ModelID.TYPE_ENTITY);
                sprite = main.ResourceManager.GetModelIcon(modelID);
            }
            return sprite;
        }
        private void ClickBlueprint(int index)
        {
            if (level.HeldItemType > 0)
            {
                if (CancelHeldItem())
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
            level.SetHeldItem(HeldTypes.BLUEPRINT, index, 0);
        }
        #endregion

        #region 暂停
        public void SwitchPauseGame()
        {
            SetGamePaused(!isPaused);
        }
        public void SetGamePaused(bool paused)
        {
            isPaused = paused;
            var levelUI = GetLevelUI();
            if (paused)
            {
                level.PlaySound(SoundID.pause);
                var spriteReference = pauseImages.Random(uiRandom);
                levelUI.SetPauseDialogImage(main.ResourceManager.GetSprite(spriteReference));
                main.MusicManager.Pause();
            }
            else
            {
                main.MusicManager.Resume();
            }
            levelUI.SetPauseDialogActive(isPaused);
            levelUI.ResetPauseDialogPosition();
        }
        #endregion

        private bool CanPlaceOnGrid(int heldType, int heldId, LawnGrid grid)
        {
            switch (heldType)
            {
                case HeldTypes.BLUEPRINT:
                    var seed = level.GetSeedPackAt(heldId);
                    if (seed == null)
                        break;
                    var seedDef = seed.Definition;
                    if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                    {
                        var entityID = seedDef.GetSeedEntityID();
                        var entityDef = Game.GetEntityDefinition(entityID);
                        if (entityDef.Type == EntityTypes.PLANT)
                        {
                            if (!grid.CanPlace(entityDef))
                                return false;
                        }
                    }
                    break;
            }
            return true;
        }
        private LevelUI GetLevelUI()
        {
#if UNITY_ANDROID || UNITY_IOS
            return mobileUI;
#else
            return standaloneUI;
#endif
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
            var levelUI = GetLevelUI();
            if (!levelNames.TryGetValue(level.StageDefinition.GetID(), out var name))
            {
                name = StringTable.LEVEL_NAME_UNKNOWN;
            }
            var levelName = main.LanguageManager._p(StringTable.CONTEXT_LEVEL_NAME, name);
            levelUI.SetLevelName(levelName);
        }
        private void UpdateDifficultyName()
        {
            var levelUI = GetLevelUI();
            if (!difficultyNames.TryGetValue(level.Difficulty, out var name))
            {
                name = StringTable.DIFFICULTY_UNKNOWN;
            }
            var difficultyName = main.LanguageManager._p(StringTable.CONTEXT_DIFFICULTY, name);
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

            var inlevelVisible = state == VisibleState.InLevel;
            levelUI.SetStarshardVisible(inlevelVisible);
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
                if (isGameStarted && Input.GetKeyDown(KeyCode.Space))
                {
                    SwitchPauseGame();
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
                    if (CancelHeldItem())
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
        private void BeginLevel()
        {
            BeginLevel(LevelTransitions.DEFAULT);
        }
        private void BeginLevel(string transition)
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
        private void ExitLevel()
        {
            // TODO
        }
        private async Task Retry()
        {
            var levelUI = GetLevelUI();
            levelUI.SetGameOverDialogInteractable(false);
            await main.LevelManager.GotoLevelScene();
            main.LevelManager.StartLevel();
        }

        #region 游戏结束
        private void ShowGameOverDialog()
        {
            var message = killerID != null ? DeathMessages.GetByEntityID(killerID) : deathMessage;
            var levelUI = GetLevelUI();
            levelUI.SetGameOverDialogActive(true);
            levelUI.SetGameOverDialogMessage(main.LanguageManager._p(DeathMessages.CONTEXT, message));
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
                preview.CreatePreviewEnemies(level, MVZ2Level.GetEnemySpawnRect());
            }
            yield return new WaitForSeconds(1);
            yield return MoveCameraToChoose();
            yield return new WaitForSeconds(1);
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
        public static Dictionary<NamespaceID, string> levelNames = new Dictionary<NamespaceID, string>()
        {
            { StageID.prologue, StringTable.LEVEL_NAME_PROLOGUE }
        };
        public static Dictionary<int, string> difficultyNames = new Dictionary<int, string>()
        {
            { LevelDifficulty.Easy, StringTable.DIFFICULTY_EASY },
            { LevelDifficulty.Normal, StringTable.DIFFICULTY_NORMAL },
            { LevelDifficulty.Hard, StringTable.DIFFICULTY_HARD },
        };
        public float LawnToTransScale => 1 / transToLawnScale;
        public float TransToLawnScale => transToLawnScale;
        public MainManager MainManager => main;
        public IGame Game => level.Game;
        public float MusicTime
        {
            get => main.MusicManager.Time;
            set => main.MusicManager.Time = value;
        }
        private bool isPaused = false;
        private List<EntityController> entities = new List<EntityController>();
        private Level level;
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
