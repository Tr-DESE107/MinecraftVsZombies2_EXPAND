using System.Collections.Generic;
using MVZ2.GameContent;
using PVZEngine;
using UnityEngine;

namespace MVZ2
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
            level.Init(0, AreaID.day, StageID.prologue, option);

            level.Spawn(ContraptionID.dispenser, new Vector3(300, 0, 300), null);
            level.Spawn(EnemyID.zombie, new Vector3(500, 0, 300), null);
            isGameStarted = true;
        }
        private void OnEntitySpawnCallback(Entity entity)
        {
            var entityController = Instantiate(entityTemplate.gameObject, entity.Pos.LawnToTrans(), Quaternion.identity, entitiesRoot).GetComponent<EntityController>();
            var modelPrefab = main.ResourceManager.GetModel(entity.Definition.GetReference());
            entityController.Init(this, entity, modelPrefab);
            entities.Add(entityController);
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
        private bool isPaused = false;
        private List<EntityController> entities = new List<EntityController>();
        private Game level;
        private MainManager main;
        private bool isGameStarted;
        [SerializeField]
        private EntityController entityTemplate;
        [SerializeField]
        private Transform entitiesRoot;
    }
}
