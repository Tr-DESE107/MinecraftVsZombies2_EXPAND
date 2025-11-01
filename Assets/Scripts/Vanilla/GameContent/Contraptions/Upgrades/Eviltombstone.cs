using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using PVZEngine.Damages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using MVZ2.GameContent.Damages;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.Eviltombstone)]
    public class Eviltombstone : ContraptionBehaviour
    {
        public Eviltombstone(string nsp, string name) : base(nsp, name)
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
            
                var arc = entity.SpawnWithParams(VanillaEnemyID.NetherArcher, pos);

            arc.AddBuff<NecrotombstoneRisingBuff>();
            arc.UpdateModel();

            arc.PlaySound(VanillaSoundID.dirtRise);
            arc.PlaySound(VanillaSoundID.boneWallBuild);

            var van = entity.SpawnWithParams(VanillaEnemyID.NetherVanguard, pos);

            van.AddBuff<NecrotombstoneRisingBuff>();
            van.UpdateModel();

            van.PlaySound(VanillaSoundID.dirtRise);
            van.PlaySound(VanillaSoundID.boneWallBuild);

            var ber = entity.SpawnWithParams(VanillaEnemyID.berserker, pos);

            ber.AddBuff<NecrotombstoneRisingBuff>();
            ber.UpdateModel();

            ber.PlaySound(VanillaSoundID.dirtRise);
            ber.PlaySound(VanillaSoundID.boneWallBuild);

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

                    var randomID = GetRandomSkeletonID(entity.RNG);
                    var spawnParam = entity.GetSpawnParams();
                    

                    var pos = entity.Position;
                    pos.y = entity.GetGroundY() - 100;
                    var warrior = entity.SpawnWithParams(randomID, pos);
                    warrior.AddBuff<NecrotombstoneRisingBuff>();
                    warrior.UpdateModel();

                    warrior.PlaySound(VanillaSoundID.dirtRise);

                    productionTimer.ResetTime(SPAWN_INTERVAL);
                }
            }
        }

        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            var pos = entity.Position;
            pos.y = entity.GetGroundY() - 100;
            var warrior = entity.SpawnWithParams(VanillaEnemyID.mesmerizer, pos);
            warrior.AddBuff<NecrotombstoneRisingBuff>();
            warrior.UpdateModel();

            warrior.PlaySound(VanillaSoundID.dirtRise);

        }

        public NamespaceID GetRandomSkeletonID(RandomGenerator rng)
        {
            var index = rng.WeightedRandom(RandomSkeletonWeights);
            return RandomSkeleton[index];
        }

        private static NamespaceID[] RandomSkeleton = new NamespaceID[]
        {
            //�������
            VanillaEnemyID.NetherWarrior,
            VanillaEnemyID.berserker
        };

        private static int[] RandomSkeletonWeights = new int[]
        {
            10,
            1
        };



        private bool SkeletonOutOfLimit(Entity entity)
        {
            return entity.Level.GetEntityCount(VanillaEnemyID.skeletonWarrior) >= SKELETON_LIMIT;
        }
        public const int SKELETON_LIMIT = 30;
        public const int SPAWN_INTERVAL = 600;
        public const int RECHECK_INTERVAL = 60;
        public const int MAGE_COUNT = 3;
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_PRODUCTION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ProductionTimer");
    }
}
