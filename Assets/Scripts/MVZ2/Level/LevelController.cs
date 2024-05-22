using System.Collections.Generic;
using System.Linq;
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
                TPS = 30
            };
            level = new Game(vanilla);
            level.OnEntitySpawn += OnEntitySpawnCallback;
            level.OnEntityRemove += OnEntityRemoveCallback;
            level.OnPlaySound += OnPlaySoundCallback;
            level.SetSeedPacks(new NamespaceID[] { ContraptionID.dispenser });
            level.Init(0, AreaID.day, StageID.prologue, option);

            UpdateBlueprints();

            isGameStarted = true;
        }
        private void Awake()
        {
            gridLayout.OnPointerEnter += OnGridEnterCallback;
            gridLayout.OnPointerExit += OnGridExitCallback;
            gridLayout.OnPointerClick += OnGridClickCallback;

            HideGridSprites();
            var levelUI = GetLevelUI();
            standaloneUI.SetActive(standaloneUI == levelUI);
            mobileUI.SetActive(mobileUI == levelUI);
        }
        private void Update()
        {
            if (isGameStarted && !isPaused)
            {
                foreach (var entity in entities)
                {
                    entity.UpdateView(Time.deltaTime);
                }
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
        private void OnPlaySoundCallback(NamespaceID soundID, Vector3 lawnPos)
        {
            main.SoundManager.Play(soundID, lawnPos.LawnToTrans());
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
        private void OnGridClickCallback(int lane, int column, PointerEventData data)
        {
            var x = level.GetEntityColumnX(column);
            var z = level.GetEntityLaneZ(lane);
            var y = level.GetGroundHeight(x, z);
            if (data.button == 0)
            {
                level.Spawn(ContraptionID.dispenser, new Vector3(x, y, z), null);
            }
            else
            {
                level.Spawn(EnemyID.zombie, new Vector3(x, y, z), null);
            }
        }
        #endregion

        private void UpdateBlueprints()
        {
            var levelUI = GetLevelUI();
            var seeds = level.GetAllSeedPacks();
            var viewDatas = new BlueprintViewData[seeds.Length];
            for (int i = 0; i < viewDatas.Length; i++)
            {
                var seed = seeds[i];
                var seedDef = level.GetSeedDefinition(seed.SeedReference);

                Sprite sprite = null;
                if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                {
                    sprite = main.ResourceManager.GetModelIcon(seedDef.GetSeedEntityID());
                }

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
        private bool isPaused = false;
        private List<EntityController> entities = new List<EntityController>();
        private Game level;
        private MainManager main;
        private bool isGameStarted;
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
    }
}
