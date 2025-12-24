using MVZ2.GameContent.Armors;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using MVZ2.GameContent.Damages;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.RandomKingSkeleton)]
    public class RandomKingSkeleton : MeleeEnemy
    {
        public RandomKingSkeleton(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStateTimer(entity, new FrameTimer(CAST_COOLDOWN));
            
        }
        protected override int GetActionState(Entity enemy)
        {
            var state = base.GetActionState(enemy);
            if (state == VanillaEntityStates.WALK && IsCasting(enemy))
            {
                return VanillaEntityStates.NECROMANCER_CAST;
            }
            return state;
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.IsDead)
                return;
            if (entity.State == VanillaEntityStates.ATTACK)
                return;
            var stateTimer = GetStateTimer(entity);
            if (entity.State == VanillaEntityStates.NECROMANCER_CAST)
            {
                stateTimer.Run(entity.GetAttackSpeed());
                if (stateTimer.Expired)
                {
                    EndCasting(entity);
                }
            }
            else
            {
                stateTimer.Run(entity.GetAttackSpeed());
                if (stateTimer.Expired)
                {
                    if (!CheckBuildable(entity))
                    {
                        stateTimer.ResetTime(BUILD_DETECT_TIME);
                    }
                    else
                    {
                        StartCasting(entity);
                        BuildBoneWalls(entity);
                    }
                }
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (entity.State == VanillaEntityStates.NECROMANCER_CAST)
            {
                EndCasting(entity);
            }
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;

            // 生成随机僵尸  
            SpawnRandomEnemy(entity);
            entity.Remove();
        }
        public static void SetCasting(Entity entity, bool timer)
        {
            entity.SetBehaviourField(ID, PROP_CASTING, timer);
        }
        public static bool IsCasting(Entity entity)
        {
            return entity.GetBehaviourField<bool>(ID, PROP_CASTING);
        }
        public static void SetStateTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourField(ID, PROP_STATE_TIMER, timer);
        }
        public static FrameTimer GetStateTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(ID, PROP_STATE_TIMER);
        }

        private void StartCasting(Entity entity)
        {
            SetCasting(entity, true);
            entity.PlaySound(VanillaSoundID.reviveCast);
            var stateTimer = GetStateTimer(entity);
            stateTimer.ResetTime(CAST_TIME);
        }

        private void EndCasting(Entity entity)
        {
            SetCasting(entity, false);
            var stateTimer = GetStateTimer(entity);
            stateTimer.ResetTime(CAST_COOLDOWN);
        }

        private bool CheckBuildable(Entity entity)
        {
            return entity.Level.FindEntities(VanillaEnemyID.RandomSkeleton).Length < MAX_BONE_WALL_COUNT;
        }

        private void BuildBoneWalls(Entity entity)
        {
            var level = entity.Level;
            int startLine = -1;
            int endLine = 1;
            var lane = entity.GetLane();
            if (lane == 0)
            {
                endLine = 0;
            }
            if (lane == level.GetMaxLaneCount() - 1)
            {
                startLine = 0;
            }

            for (int i = startLine; i <= endLine; i++)
            {
                var x = entity.Position.x + level.GetGridWidth() * 1.5f * entity.GetFacingX();
                var z = entity.Position.z + level.GetGridHeight() * i * 1f;
                var y = level.GetGroundY(x, z);
                Vector3 wallPos = new Vector3(x, y, z);
                entity.SpawnWithParams(VanillaEnemyID.RandomSkeleton, wallPos);
            }
        }

        /// <summary>  
        /// 在原位置生成随机僵尸  
        /// </summary>  
        private void SpawnRandomEnemy(Entity entity)
        {
            var level = entity.Level;
            var rng = entity.RNG;

            // 使用白名单和权重生成  
            NamespaceID enemyID = GetRandomEnemyID(rng);

            // 在原位置生成僵尸  
            var spawnParam = entity.GetSpawnParams();
            spawnParam.SetProperty(EngineEntityProps.FACTION, entity.GetFaction());
            entity.Spawn(enemyID, entity.Position, spawnParam);
        }

        /// <summary>  
        /// 根据权重随机选择一个僵尸ID  
        /// </summary>  
        private NamespaceID GetRandomEnemyID(RandomGenerator rng)
        {
            var index = rng.WeightedRandom(SpawnWeights);
            return SpawnWhitelist[index];
        }



        // 定义可生成的僵尸白名单  
        private static NamespaceID[] SpawnWhitelist = new NamespaceID[]
        {
            VanillaEnemyID.necromancer,
            VanillaEnemyID.KingSkeleton,
            VanillaEnemyID.EvilMage,
            VanillaEnemyID.KingofReverser,
            VanillaEnemyID.NetherWarrior,
            VanillaEnemyID.NetherArcher,
            VanillaEnemyID.NetherVanguard,
            VanillaEnemyID.AngryReverser,
            VanillaEnemyID.RaiderSkull,
            VanillaEnemyID.WitherSkeletonHorse,
            VanillaEnemyID.WintherMage,

        };

        // 定义每个僵尸的生成权重（数值越大，生成概率越高）  
        private static int[] SpawnWeights = new int[]
        {
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,

        };

        #region 常量
        private const int CAST_COOLDOWN = 300;
        private const int CAST_TIME = 30;
        private const int BUILD_DETECT_TIME = 30;
        private const int MAX_BONE_WALL_COUNT = 15;
        public static readonly NamespaceID ID = VanillaEnemyID.RandomKingSkeleton;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("StateTimer");
        public static readonly VanillaEntityPropertyMeta<bool> PROP_CASTING = new VanillaEntityPropertyMeta<bool>("Casting");
        #endregion 常量
    }
}
