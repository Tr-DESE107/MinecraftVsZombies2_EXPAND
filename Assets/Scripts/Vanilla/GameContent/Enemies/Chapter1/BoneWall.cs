using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.boneWall)]
    public class BoneWall : StateEnemy
    {
        public BoneWall(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = entity.GetMaxTimeout();
            entity.PlaySound(VanillaSoundID.boneWallBuild);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            if (entity.Timeout >= 0)
            {
                entity.Timeout--;
                if (entity.Timeout <= 0)
                {
                    entity.Die(entity);
                    var randomID = GetRandomSkeletonID(entity.RNG);

                    var spawnParam = entity.GetSpawnParams();
                    spawnParam.SetProperty(EngineEntityProps.FACTION, entity.GetFaction());
                    entity.Spawn(randomID, entity.Position, spawnParam);
                }
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            entity.Level.Spawn(VanillaEffectID.boneParticles, entity.GetCenter(), entity);
            entity.Remove();
        }

        public NamespaceID GetRandomSkeletonID(RandomGenerator rng)
        {
            var index = rng.WeightedRandom(RandomSkeletonWeights);
            return RandomSkeleton[index];
        }

        private static NamespaceID[] RandomSkeleton = new NamespaceID[]
        {
            //怪物出怪
            VanillaEnemyID.SkeletonHead,
            VanillaEnemyID.MeleeSkeleton,
            VanillaEnemyID.skeleton,
            VanillaEnemyID.skeletonHorse,

        };

        private static int[] RandomSkeletonWeights = new int[]
        {
            10,
            5,
            5,
            2
        };
    }
}
