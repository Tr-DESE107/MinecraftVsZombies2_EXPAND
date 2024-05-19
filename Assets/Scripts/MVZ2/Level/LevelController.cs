using System.Collections.Generic;
using MVZ2.GameContent;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class LevelController : MonoBehaviour
    {
        public EntityController Spawn(NamespaceID id, Vector3 position, Entity spawner)
        {
            var ent = level.Spawn(id, position, spawner);
            var entityController = Instantiate(entityTemplate.gameObject, position.LawnToTrans(), Quaternion.identity, entitiesRoot).GetComponent<EntityController>();
            entityController.Init(this, ent, modelPrefab);
            entities.Add(entityController);
            return entityController;
        }
        private void Awake()
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
            level.Init(0, AreaID.day, StageID.prologue, option);

            Spawn(ContraptionID.dispenser, new Vector3(400, 300), null);
        }
        private void Update()
        {
            if (!isPaused)
            {
                foreach (var entity in entities)
                {
                    entity.UpdateView(Time.deltaTime);
                }
            }
        }
        private void FixedUpdate()
        {
            if (!isPaused)
            {
                level.Update();
                foreach (var entity in entities)
                {
                    entity.UpdateLogic();
                }
            }
        }
        private bool isPaused = false;
        private List<EntityController> entities = new List<EntityController>();
        private Game level;
        [SerializeField]
        private EntityController entityTemplate;
        [SerializeField]
        private Model modelPrefab;
        [SerializeField]
        private Transform entitiesRoot;
    }
}
