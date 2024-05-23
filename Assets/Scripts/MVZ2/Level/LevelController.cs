using System.Collections.Generic;
using System.Linq;
using Codice.CM.Common;
using MVZ2.GameContent;
using MVZ2.Level.UI;
using MVZ2.UI;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;
using UnityEngine.EventSystems;

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
            var vanilla = new Vanilla.Vanilla();
            var option = new GameOption()
            {
                CardSlotCount = 10,
                StarshardSlotCount = 10,
                LeftFaction = 0,
                RightFaction = 1,
                StartEnergy = 50,
                MaxEnergy = 9990,
                TPS = 30
            };
            level = new Game(vanilla);
            level.OnEntitySpawn += OnEntitySpawnCallback;
            level.OnEntityRemove += OnEntityRemoveCallback;
            level.OnPlaySoundPosition += OnPlaySoundPositionCallback;
            level.OnPlaySound += OnPlaySoundCallback;
            level.OnHeldItemChanged += OnHeldItemChangedCallback;
            level.OnHeldItemReset += OnHeldItemResetCallback;
            level.SetSeedPacks(new NamespaceID[] { ContraptionID.dispenser });
            level.Init(0, AreaID.day, StageID.prologue, option);
            level.SetEnergy(9990);
            level.ResetHeldItem();

            UpdateBlueprints();

            isGameStarted = true;
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
            standaloneUI.SetActive(standaloneUI == levelUI);
            mobileUI.SetActive(mobileUI == levelUI);

            levelUI.OnBlueprintPointerDown += OnBlueprintPointerDownCallback;
            levelUI.OnRaycastReceiverPointerDown += OnRaycastReceiverPointerDownCallback;
            levelUI.SetHeldItemIcon(null);
        }
        private void Update()
        {
            if (isGameStarted && !isPaused)
            {
                foreach (var entity in entities)
                {
                    entity.UpdateView(Time.deltaTime);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    CancelHeldItem();
                }
                var ui = GetLevelUI();
                ui.SetHeldItemPosition(levelCamera.ScreenToWorldPoint(Input.mousePosition));
                ui.SetEnergy(Mathf.FloorToInt(Mathf.Max(0, level.Energy - level.GetDelayedEnergy())).ToString());
                UpdateBlueprintRecharges();
                UpdateBlueprintDisabled();

            }
        }
        private void FixedUpdate()
        {
            if (isGameStarted && !isPaused)
            {
                level.Update();
                foreach (var entity in entities.ToArray())
                {
                    entity.UpdateLogic();
                }
            }
        }
        #endregion

        #region 事件回调

        #region 逻辑方
        private void OnEntitySpawnCallback(Entity entity)
        {
            var entityController = Instantiate(entityTemplate.gameObject, entity.Pos.LawnToTrans(), Quaternion.identity, entitiesRoot).GetComponent<EntityController>();
            var modelPrefab = main.ResourceManager.GetModel(entity.Definition.GetReference());
            entityController.Init(this, entity, modelPrefab);
            entities.Add(entityController);
        }
        private void OnEntityRemoveCallback(Entity entity)
        {
            var entityController = entities.FirstOrDefault(e => e.Entity == entity);
            if (entityController)
            {
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
        private void OnGridEnterCallback(int lane, int column, PointerEventData data)
        {
            var grid = gridLayout.GetGrid(lane, column);
            grid.SetColor(Color.green);
        }
        private void OnGridExitCallback(int lane, int column, PointerEventData data)
        {
            var grid = gridLayout.GetGrid(lane, column);
            grid.SetColor(Color.clear);
        }
        private void OnGridPointerDownCallback(int lane, int column, PointerEventData data)
        {
            switch (heldItemType)
            {
                case HeldTypes.BLUEPRINT:
                    var seed = level.GetSeedPackAt(heldItemID);
                    var seedDef = level.GetSeedDefinition(seed.SeedReference);
                    if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                    {
                        var x = level.GetEntityColumnX(column);
                        var z = level.GetEntityLaneZ(lane);
                        var y = level.GetGroundHeight(x, z);
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
        private void OnBlueprintPointerDownCallback(int index)
        {
            if (heldItemType == HeldTypes.BLUEPRINT)
            {
                CancelHeldItem();
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
        private void OnRaycastReceiverPointerDownCallback()
        {
            switch (heldItemType)
            {
                case HeldTypes.BLUEPRINT:
                    CancelHeldItem();
                    break;
            }
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
            LayerMask layerMask = Layers.GetMask(Layers.DEFAULT);
            switch (heldType)
            {
                case HeldTypes.BLUEPRINT:
                    icon = GetBlueprintIcon(id);
                    layerMask = Layers.GetMask(Layers.GRID, Layers.RAYCAST_RECEIVER);
                    break;
            }
            ui.SetHeldItemIcon(icon);
            ui.SetRaycasterMask(layerMask);
            raycaster.eventMask = layerMask;
        }
        private void CancelHeldItem()
        {
            if (heldItemType <= 0 || heldItemNoCancel)
                return;
            level.ResetHeldItem();
            level.PlaySound(SoundID.tap);
        }
        private void UpdateBlueprints()
        {
            var levelUI = GetLevelUI();
            var seeds = level.GetAllSeedPacks();
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
            var seeds = level.GetAllSeedPacks();
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
            var seeds = level.GetAllSeedPacks();
            var disabled = new bool[seeds.Length];
            for (int i = 0; i < disabled.Length; i++)
            {
                var seed = seeds[i];
                disabled[i] = IsHoldingBlueprint(i) || !CanPickBlueprint(seed);
            }
            levelUI.SetBlueprintDisabled(disabled);
        }
        private bool IsHoldingBlueprint(int i)
        {
            return heldItemType == HeldTypes.BLUEPRINT && heldItemID == i;
        }
        private bool CanPickBlueprint(SeedPack seed)
        {
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
                sprite = main.ResourceManager.GetModelIcon(seedDef.GetSeedEntityID());
            }
            return sprite;
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
        private void SelectBlueprint(int index)
        {
            level.SetHeldItem(HeldTypes.BLUEPRINT, index, 0);
        }
        #endregion

        #region 属性字段
        private bool isPaused = false;
        private List<EntityController> entities = new List<EntityController>();
        private Game level;
        private MainManager main;
        private bool isGameStarted;
        private int heldItemType;
        private int heldItemID;
        private int heldItemPriority;
        private bool heldItemNoCancel;

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
