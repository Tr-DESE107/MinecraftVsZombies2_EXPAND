using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.KingofReverser)]
    public class KingofReverser : MeleeEnemy
    {
        public KingofReverser(string nsp, string name) : base(nsp, name)
        {
            // 光球检测器，参考 AngryReverser
            detector = new DispenserDetector()
            {
                ignoreBoss = true,
                ignoreHighEnemy = false,
                ignoreLowEnemy = false,
                colliderFilter = (p, c) => ColliderFilter(p.entity, c)
            };
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);

            // 初始化两个独立的计时器
            SetReverserTimer(entity, new FrameTimer(REVERSER_COOLDOWN)); // 光球
            SetSummonTimer(entity, new FrameTimer(CAST_COOLDOWN));      // 骷髅召唤
        }

        protected override int GetActionState(Entity enemy)
        {
            var state = base.GetActionState(enemy);

            // 如果正在释放光球，切换到 CAST 动画
            if (state == STATE_WALK && IsCasting(enemy))
            {
                return STATE_CAST;
            }

            // 如果正在召唤骷髅，切换到 NECROMANCER_CAST 动画
            if (state == STATE_WALK && IsSummoning(enemy))
            {
                return STATE_CAST;
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

            // ---------- 光球逻辑 ----------
            if (entity.State == STATE_WALK && !IsCasting(entity))
            {
                var orbTimer = GetReverserTimer(entity);
                orbTimer.Run(entity.GetAttackSpeed());
                if (orbTimer.Expired)
                {
                    var target = detector.DetectEntityWithTheLeast(entity, t => Mathf.Abs(entity.Position.x - t.Position.x));
                    if (target == null)
                    {
                        orbTimer.Frame = CONTROL_DETECT_TIME;
                    }
                    else
                    {
                        var param = entity.GetShootParams();
                        param.damage = 0;
                        var orb = entity.ShootProjectile(param);
                        orb.Target = target;
                        orb.SetParent(entity);
                        SetOrb(entity, new EntityID(orb));
                        SetCasting(entity, true);
                    }
                }
            }
            else if (entity.State == STATE_CAST)
            {
                var orbID = GetOrb(entity);
                var orb = orbID?.GetEntity(entity.Level);
                if (orb == null || !orb.Exists() || orb.IsDead)
                {
                    EndCasting(entity);
                }
            }

            // ---------- 召唤逻辑 ----------
            if (entity.State == STATE_WALK && !IsSummoning(entity))
            {
                var summonTimer = GetSummonTimer(entity);
                summonTimer.Run(entity.GetAttackSpeed());
                if (summonTimer.Expired)
                {
                    if (!CheckBuildable(entity))
                    {
                        summonTimer.ResetTime(BUILD_DETECT_TIME);
                    }
                    else
                    {
                        StartSummon(entity);
                        BuildBoneWalls(entity);
                    }
                }
            }
            else if (entity.State == STATE_CAST)
            {
                var summonTimer = GetSummonTimer(entity);
                summonTimer.Run(entity.GetAttackSpeed());
                if (summonTimer.Expired)
                {
                    EndSummon(entity);
                }
            }
        }

        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);

            // 清理光球
            if (entity.State == STATE_CAST)
            {
                EndCasting(entity);
            }

            // 清理召唤
            if (entity.State == STATE_CAST)
            {
                EndSummon(entity);
            }
        }

        #region 光球相关
        private void EndCasting(Entity entity)
        {
            var orbTimer = GetReverserTimer(entity);
            orbTimer.ResetTime(REVERSER_COOLDOWN);
            SetCasting(entity, false);
        }

        private bool ColliderFilter(Entity self, IEntityCollider collider)
        {
            if (!collider.IsForMain())
                return false;
            var target = collider.Entity;
            if (!CompellingOrb.CanControl(self, target))
                return false;
            if (target.IsFloor())
                return false;
            return true;
        }
        #endregion

        #region 召唤相关
        private void StartSummon(Entity entity)
        {
            SetSummoning(entity, true);
            entity.PlaySound(VanillaSoundID.reviveCast);
            var summonTimer = GetSummonTimer(entity);
            summonTimer.ResetTime(CAST_TIME);
        }

        private void EndSummon(Entity entity)
        {
            SetSummoning(entity, false);
            var summonTimer = GetSummonTimer(entity);
            summonTimer.ResetTime(CAST_COOLDOWN);
        }

        private bool CheckBuildable(Entity entity)
        {
            return entity.Level.FindEntities(VanillaEnemyID.MeleeSkeleton).Length < MAX_BONE_WALL_COUNT;
        }

        private void BuildBoneWalls(Entity entity)
        {
            var level = entity.Level;
            //int startLine = -1;
            //int endLine = 1;
            //var lane = entity.GetLane();
            //if (lane == 0)
            //{
            //    endLine = 0;
            //}
            //if (lane == level.GetMaxLaneCount() - 1)
            //{
            //    startLine = 0;
            //}

            //for (int i = startLine; i <= endLine; i++)
            //{
            var x = entity.Position.x + level.GetGridWidth() * 1.5f * entity.GetFacingX();
            var z = entity.Position.z;
            var y = level.GetGroundY(x, z);
            Vector3 wallPos = new Vector3(x, y, z);

            var randomID = GetRandomSkeletonID(entity.RNG);

            entity.SpawnWithParams(randomID, wallPos);
            //}
        }
        #endregion


        public NamespaceID GetRandomSkeletonID(RandomGenerator rng)
        {
            var index = rng.WeightedRandom(RandomSkeletonWeights);
            return RandomSkeleton[index];
        }

        private static NamespaceID[] RandomSkeleton = new NamespaceID[]
        {
            //怪物出怪
            VanillaEnemyID.WitherSkeleton,
            VanillaEnemyID.NetherWarrior,
            VanillaEnemyID.NetherArcher,
            VanillaEnemyID.BerserkerHead,

        };

        private static int[] RandomSkeletonWeights = new int[]
        {
            10,
            2,
            2,
            5
        };


        #region 属性存取器
        public static void SetCasting(Entity entity, bool timer) => entity.SetBehaviourField(ID, PROP_CASTING, timer);
        public static bool IsCasting(Entity entity) => entity.GetBehaviourField<bool>(ID, PROP_CASTING);
        public static void SetOrb(Entity entity, EntityID value) => entity.SetBehaviourField(ID, PROP_ORB, value);
        public static EntityID GetOrb(Entity entity) => entity.GetBehaviourField<EntityID>(ID, PROP_ORB);
        public static void SetReverserTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, PROP_REVERSER_TIMER, timer);
        public static FrameTimer GetReverserTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_REVERSER_TIMER);

        public static void SetSummoning(Entity entity, bool timer) => entity.SetBehaviourField(ID, PROP_SUMMONING, timer);
        public static bool IsSummoning(Entity entity) => entity.GetBehaviourField<bool>(ID, PROP_SUMMONING);
        public static void SetSummonTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, PROP_SUMMON_TIMER, timer);
        public static FrameTimer GetSummonTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_SUMMON_TIMER);
        #endregion

        #region 常量
        // 光球
        private Detector detector;
        private const int REVERSER_COOLDOWN = 300;
        private const int CONTROL_DETECT_TIME = 30;

        // 召唤
        private const int CAST_COOLDOWN = 300;
        private const int CAST_TIME = 30;
        private const int BUILD_DETECT_TIME = 30;
        private const int MAX_BONE_WALL_COUNT = 15;

        // 状态
        //VanillaEnemyStates.MELEE_ATTACK
        public const int STATE_WALK = VanillaEnemyStates.WALK;
        public const int STATE_ATTACK = VanillaEnemyStates.MELEE_ATTACK;
        public const int STATE_CAST = VanillaEnemyStates.CAST;


        // ID
        public static readonly NamespaceID ID = VanillaEnemyID.KingofReverser;

        // 属性字段
        public static readonly VanillaEntityPropertyMeta<bool> PROP_CASTING = new VanillaEntityPropertyMeta<bool>("Casting");
        public static readonly VanillaEntityPropertyMeta<EntityID> PROP_ORB = new VanillaEntityPropertyMeta<EntityID>("Orb");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_REVERSER_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ReverserTimer");

        public static readonly VanillaEntityPropertyMeta<bool> PROP_SUMMONING = new VanillaEntityPropertyMeta<bool>("Summoning");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_SUMMON_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("SummonTimer");
        #endregion
    }
}
