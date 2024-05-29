using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Seeds;
using MVZ2.Level.UI;
using MVZ2.UI;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using LawnGrid = PVZEngine.LawnGrid;

namespace MVZ2.Level
{
    public class LevelController : MonoBehaviour
    {
        #region 公有方法
        public void SetMainManager(MainManager main)
        {
            this.main = main;
        }
        public void StartGame()
        {
            var vanilla = new Vanilla.VanillaMod();
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
            level = new PVZEngine.Level(vanilla);
            level.OnEntitySpawn += OnEntitySpawnCallback;
            level.OnEntityRemove += OnEntityRemoveCallback;
            level.OnPlaySoundPosition += OnPlaySoundPositionCallback;
            level.OnPlaySound += OnPlaySoundCallback;
            level.OnShakeScreen += OnShakeScreenCallback;
            level.OnHeldItemChanged += OnHeldItemChangedCallback;
            level.OnHeldItemReset += OnHeldItemResetCallback;
            level.SetSeedPacks(new NamespaceID[]
            {
                ContraptionID.dispenser,
                ContraptionID.furnace,
                ContraptionID.obsidian,
                ContraptionID.mineTNT,
                null,
                null
            });
            level.Init(AreaID.day, StageID.prologue, option);

            var cartRef = level.GetProperty<NamespaceID>(AreaProperties.CART_REFERENCE);
            level.SpawnCarts(cartRef, MVZ2Level.CART_START_X, 20);
            level.Spawn<Miner>(new Vector3(600, 0, 60), null);
            level.ResetHeldItem();
            level.Start(LevelDifficulty.Normal);

            levelProgress = 0;
            bannerProgresses = new float[level.GetTotalFlags()];

            //level.SetEnergy(9990);
            //level.RechargeSpeed = 99;

            var levelUI = GetLevelUI();
            standaloneUI.SetActive(standaloneUI == levelUI);
            mobileUI.SetActive(mobileUI == levelUI);
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
                    switch (heldItemType)
                    {
                        case HeldTypes.PICKAXE:
                            return CanDigContraption(entity);
                        case HeldTypes.STARSHARD:
                            return entity.CanEvoke();
                    }
                    break;
            }
            return false;
        }
        public float GetGameSpeed()
        {
            return speedUp ? 2 : 1;
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            gridLayout.OnPointerEnter += OnGridEnterCallback;
            gridLayout.OnPointerExit += OnGridExitCallback;
            gridLayout.OnPointerDown += OnGridPointerDownCallback;


            HideGridSprites();
            var levelUI = GetLevelUI();
            levelUI.OnBlueprintPointerDown += OnBlueprintPointerDownCallback;
            levelUI.OnRaycastReceiverPointerDown += OnRaycastReceiverPointerDownCallback;
            levelUI.OnPickaxePointerDown += OnPickaxePointerDownCallback;
            levelUI.OnMenuButtonClick += OnMenuButtonClickCallback;
            levelUI.OnSpeedUpButtonClick += OnSpeedUpButtonClickCallback;
            levelUI.OnStarshardPointerDown += OnStarshardPointerDownCallback;
            levelUI.SetHeldItemIcon(null);
        }
        private void Update()
        {
            if (!isGameStarted || isPaused)
                return;

            var deltaTime = Time.deltaTime * GetGameSpeed();
            foreach (var entity in entities)
            {
                entity.UpdateView(deltaTime);
            }
            InputUpdate();
            var ui = GetLevelUI();
            ui.SetHeldItemPosition(levelCamera.ScreenToWorldPoint(Input.mousePosition));
            ui.SetEnergy(Mathf.FloorToInt(Mathf.Max(0, level.Energy - level.GetDelayedEnergy())).ToString());
            ui.SetPickaxeVisible(!IsHoldingPickaxe());
            UpdateLevelProgress();
            UpdateBlueprintRecharges();
            UpdateBlueprintDisabled();
            UpdateStarshards();

            var cameraOffset = Vector3.zero;
            foreach (var shake in cameraShakes)
            {
                cameraOffset += (Vector3)shake.GetShake2D();
            }
            cameraRoot.transform.position = cameraPosition + cameraOffset;
        }
        private void FixedUpdate()
        {
            if (!isGameStarted || isPaused)
            {
                foreach (var entity in entities.Where(e => e.Entity.Type == EntityTypes.CART && e.Entity.State == EntityStates.IDLE).ToArray())
                {
                    entity.UpdateLogic(Time.fixedDeltaTime, 1);
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
                    entity.UpdateLogic(Time.fixedDeltaTime, gameSpeed);
                }
                foreach (var shake in cameraShakes)
                {
                    shake.timeout--;
                }
                cameraShakes.RemoveAll(s => s.timeout <= 0);
            }
        }
        #endregion

        #region 事件回调

        #region 逻辑方
        private void OnEntitySpawnCallback(Entity entity)
        {
            var entityController = Instantiate(entityTemplate.gameObject, entity.Pos.LawnToTrans(), Quaternion.identity, entitiesRoot).GetComponent<EntityController>();
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
            main.SoundManager.Play(soundID, lawnPos.LawnToTrans(), pitch, 1);
        }
        private void OnPlaySoundCallback(NamespaceID soundID, float pitch)
        {
            main.SoundManager.Play(soundID, Vector3.zero, pitch, 0);
        }
        private void OnShakeScreenCallback(float startAmplitude, float endAmplitude, int time)
        {
            cameraShakes.Add(new Shake(startAmplitude * PositionHelper.LAWN_TO_TRANS_SCALE, endAmplitude * PositionHelper.LAWN_TO_TRANS_SCALE, time));
        }
        private void OnHeldItemChangedCallback(int heldType, int id, int priority, bool noCancel)
        {
            if (heldItemType > 0 && heldItemPriority > priority)
                return;
            SetHeldItem(heldType, id, priority, noCancel);
        }
        private void OnHeldItemResetCallback()
        {
            SetHeldItem(0, 0, 0, false);
        }
        #endregion

        #region UI方
        private void OnEntityPointerEnterCallback(EntityController entity, PointerEventData eventData)
        {
            entity.SetHovered(true);
            switch (entity.Entity.Type)
            {
                case EntityTypes.PICKUP:
                    if (heldItemType == HeldTypes.NONE && Input.GetMouseButton((int)MouseButton.LeftMouse))
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
        private void OnEntityPointerDownCallback(EntityController entity, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            switch (entity.Entity.Type)
            {
                case EntityTypes.PLANT:
                    switch (heldItemType)
                    {
                        case HeldTypes.PICKAXE:
                            if (CanDigContraption(entity.Entity))
                            {
                                entity.Entity.Die();
                                level.ResetHeldItem();
                            }
                            break;
                        case HeldTypes.STARSHARD:
                            if (entity.Entity.CanEvoke())
                            {
                                entity.Entity.Evoke();
                                level.ResetHeldItem();
                            }
                            break;
                    }
                    break;
                case EntityTypes.PICKUP:
                    if (heldItemType == HeldTypes.NONE)
                    {
                        var pickup = entity.Entity;
                        if (!pickup.IsCollected())
                            pickup.Collect();
                    }
                    break;
            }
        }
        private void OnGridEnterCallback(int lane, int column, PointerEventData data)
        {
            var grid = gridLayout.GetGrid(lane, column);
            var color = Color.clear;
            if (heldItemType > 0)
            {
                color = CanPlaceOnGrid(heldItemType, heldItemID, level.GetGrid(column, lane)) ? Color.green : Color.red;
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
            if (!CanPlaceOnGrid(heldItemType, heldItemID, level.GetGrid(column, lane)))
                return;
            switch (heldItemType)
            {
                case HeldTypes.BLUEPRINT:
                    var seed = level.GetSeedPackAt(heldItemID);
                    if (seed == null)
                        break;
                    var seedDef = level.GetSeedDefinition(seed.SeedReference);
                    if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                    {
                        var x = level.GetEntityColumnX(column);
                        var z = level.GetEntityLaneZ(lane);
                        var y = level.GetGroundY(x, z);
                        var position = new Vector3(x, y, z);
                        var entityID = seedDef.GetSeedEntityID();
                        var entityDef = level.GetEntityDefinition(entityID);
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
                    switch (heldItemType)
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
                    switch (heldItemType)
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
        #endregion

        #endregion
        private void SetHeldItem(int heldType, int id, int priority, bool noCancel)
        {
            heldItemType = heldType;
            heldItemID = id;
            heldItemPriority = priority;
            heldItemNoCancel = noCancel;

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
            if (heldItemType <= 0 || heldItemNoCancel)
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
            return heldItemType == HeldTypes.PICKAXE;
        }
        private bool IsHoldingStarshard()
        {
            return heldItemType == HeldTypes.STARSHARD;
        }
        private void ClickPickaxe()
        {
            if (heldItemType > 0)
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
            if (heldItemType > 0)
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
                var seedDef = level.GetSeedDefinition(seed.SeedReference);
                var sprite = GetBlueprintIcon(seedDef);
                var viewData = new BlueprintViewData()
                {
                    icon = sprite,
                    cost = seed.Cost.ToString(),
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
                recharges[i] = seed.MaxRecharge == 0 ? 1 : seed.Recharge / (float)seed.MaxRecharge;
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
            return heldItemType == HeldTypes.BLUEPRINT && heldItemID == i;
        }
        private bool CanPickBlueprint(SeedPack seed)
        {
            if (seed == null)
                return false;
            return level.Energy >= seed.Cost && seed.IsCharged();
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
            var seedDef = level.GetSeedDefinition(seed.SeedReference);
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
            if (heldItemType > 0)
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

        private bool CanPlaceOnGrid(int heldType, int heldId, LawnGrid grid)
        {
            switch (heldType)
            {
                case HeldTypes.BLUEPRINT:
                    var seed = level.GetSeedPackAt(heldId);
                    if (seed == null)
                        break;
                    var seedDef = level.GetSeedDefinition(seed.SeedReference);
                    if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                    {
                        var entityID = seedDef.GetSeedEntityID();
                        var entityDef = level.GetEntityDefinition(entityID);
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
            float targetProgress = level.CurrentWave / (float)level.GetTotalWaveCount();
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
        }
        private void InputUpdate()
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
            if (Input.GetMouseButtonDown(1))
            {
                if (CancelHeldItem())
                {
                    level.PlaySound(SoundID.tap);
                }
            }
            for (int i = 0; i < 10 ; i++)
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
            if (Input.GetKeyDown(KeyCode.F))
            {
                SwitchSpeedUp();
            }
        }
        #endregion

        #region 属性字段
        public const int SelfFaction = 0;
        public const int EnemyFaction = 1;
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
        public MainManager MainManager => main;
        private bool isPaused = false;
        private List<EntityController> entities = new List<EntityController>();
        private PVZEngine.Level level;
        private MainManager main;
        private bool isGameStarted;
        private int heldItemType;
        private int heldItemID;
        private int heldItemPriority;
        private bool heldItemNoCancel;
        private List<Shake> cameraShakes = new List<Shake>();
        private bool speedUp;
        private float gameRunTimeModular;


        private float levelProgress;
        private float[] bannerProgresses;

        [SerializeField]
        private Vector3 cameraPosition;
        [SerializeField]
        private Transform cameraRoot;
        [SerializeField]
        private Camera levelCamera;
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
        #endregion
    }
}
