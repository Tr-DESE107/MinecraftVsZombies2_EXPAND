using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.necrotombstone)]
    public class Necrotombstone : ContraptionBehaviour
    {
        public Necrotombstone(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetProductionTimer(entity, new FrameTimer(SPAWN_INTERVAL));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            ProductionUpdate(entity);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var pos = entity.Position;
            pos.y = entity.GetGroundY() - 100;
            var mageClass = SkeletonMage.mageClasses.Random(entity.RNG);
            for (int i = 0; i < MAGE_COUNT; i++)
            {
                var mage = entity.SpawnWithParams(VanillaEnemyID.skeletonMage, pos);
                SkeletonMage.SetClass(mage, mageClass);
                mage.AddBuff<NecrotombstoneRisingBuff>();
                mage.UpdateModel();

                mage.PlaySound(VanillaSoundID.dirtRise);
                mage.PlaySound(VanillaSoundID.boneWallBuild);
            }
        }
        public static FrameTimer GetProductionTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_PRODUCTION_TIMER);
        public static void SetProductionTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_PRODUCTION_TIMER, timer);
        private void ProductionUpdate(Entity entity)
        {
            var productionTimer = GetProductionTimer(entity);
            if (!entity.Level.IsCleared)
            {
                productionTimer.Run(entity.GetProduceSpeed());
            }
            if (productionTimer.Expired)
            {
                if (SkeletonOutOfLimit(entity))
                {
                    productionTimer.Frame = RECHECK_INTERVAL;
                }
                else
                {
                    var pos = entity.Position;
                    pos.y = entity.GetGroundY() - 100;
                    var warrior = entity.SpawnWithParams(VanillaEnemyID.skeletonWarrior, pos);
                    warrior.AddBuff<NecrotombstoneRisingBuff>();
                    warrior.UpdateModel();

                    warrior.PlaySound(VanillaSoundID.dirtRise);

                    productionTimer.ResetTime(SPAWN_INTERVAL);
                }
            }
        }
        private bool SkeletonOutOfLimit(Entity entity)
        {
            return entity.Level.GetEntityCount(VanillaEnemyID.skeletonWarrior) >= SKELETON_LIMIT;
        }
        public const int SKELETON_LIMIT = 30;
        public const int SPAWN_INTERVAL = 450;
        public const int RECHECK_INTERVAL = 60;
        public const int MAGE_COUNT = 3;
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_PRODUCTION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ProductionTimer");
    }
}
