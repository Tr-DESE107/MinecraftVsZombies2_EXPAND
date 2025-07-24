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

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.KingSkeleton)]
    public class KingSkeleton : MeleeEnemy
    {
        public KingSkeleton(string nsp, string name) : base(nsp, name)
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
            return entity.Level.FindEntities(VanillaEnemyID.MeleeSkeleton).Length < MAX_BONE_WALL_COUNT;
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
                entity.SpawnWithParams(VanillaEnemyID.MeleeSkeleton, wallPos);
            }
        }
        #region 常量
        private const int CAST_COOLDOWN = 300;
        private const int CAST_TIME = 30;
        private const int BUILD_DETECT_TIME = 30;
        private const int MAX_BONE_WALL_COUNT = 15;
        public static readonly NamespaceID ID = VanillaEnemyID.necromancer;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("StateTimer");
        public static readonly VanillaEntityPropertyMeta<bool> PROP_CASTING = new VanillaEntityPropertyMeta<bool>("Casting");
        #endregion 常量
    }
}
