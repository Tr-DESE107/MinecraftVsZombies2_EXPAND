using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.necromancer)]
    public class Necromancer : MeleeEnemy
    {
        public Necromancer(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStateTimer(entity, new FrameTimer(CAST_COOLDOWN));
        }
        protected override int GetActionState(Entity enemy)
        {
            if (IsCasting(enemy))
            {
                return VanillaEntityStates.NECROMANCER_CAST;
            }
            return base.GetActionState(enemy);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
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
            entity.SetBehaviourProperty(ID, "Casting", timer);
        }
        public static bool IsCasting(Entity entity)
        {
            return entity.GetBehaviourProperty<bool>(ID, "Casting");
        }
        public static void SetStateTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourProperty(ID, "StateTimer", timer);
        }
        public static FrameTimer GetStateTimer(Entity entity)
        {
            return entity.GetBehaviourProperty<FrameTimer>(ID, "StateTimer");
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
            return entity.Level.FindEntities(VanillaEnemyID.boneWall).Length < MAX_BONE_WALL_COUNT;
        }

        private void BuildBoneWalls(Entity entity)
        {
            var level = entity.Level;
            int startLine = -2;
            int endLine = 2;
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
                var z = entity.Position.z + level.GetGridHeight() * i * 0.5f;
                var y = level.GetGroundY(x, z);
                Vector3 wallPos = new Vector3(x, y, z);
                var boneWall = level.Spawn(VanillaEnemyID.boneWall, wallPos, entity);
                boneWall.SetFactionAndDirection(entity.GetFaction());
            }
        }
        #region 常量
        private const int CAST_COOLDOWN = 300;
        private const int CAST_TIME = 30;
        private const int BUILD_DETECT_TIME = 30;
        private const int MAX_BONE_WALL_COUNT = 15;
        public static NamespaceID ID => VanillaEnemyID.necromancer;
        #endregion 常量
    }
}
