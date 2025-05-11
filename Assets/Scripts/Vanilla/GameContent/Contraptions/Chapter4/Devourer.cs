using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Obstacles;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.devourer)]
    public class Devourer : ContraptionBehaviour
    {
        public Devourer(string nsp, string name) : base(nsp, name)
        {
            evokedDetector = new DevourerEvokedDetector();
        }
        public override void Init(Entity devourer)
        {
            base.Init(devourer);
            SetDevourTimer(devourer, new FrameTimer(DEVOUR_TIME));
            devourer.Level.AddLoopSoundEntity(VanillaSoundID.metalSaw, devourer.ID);
            devourer.Target = GetMillTarget(devourer);
            if (!devourer.Target.ExistsAndAlive())
            {
                devourer.Die(new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE), devourer);
                return;
            }
            UpdateDevourerPosition(devourer);
            devourer.SetAnimationBool("Mill", true);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.SetEvoked(true);
            entity.AddBuff<DevourerInvincibleBuff>();
            entity.PlaySound(VanillaSoundID.pacmanStart);
            entity.Level.RemoveLoopSoundEntity(VanillaSoundID.metalSaw, entity.ID);
            entity.Level.AddLoopSoundEntity(VanillaSoundID.pacmanGhost, entity.ID);
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;

            var devourer = collision.Entity;
            if (devourer.State != VanillaEntityStates.CONTRAPTION_SPECIAL)
                return;
            var other = collision.Other;
            if (!devourer.IsHostile(other))
                return;
            var level = devourer.Level;
            var output = other.TakeDamage(devourer.GetDamage() * EVOKED_DAMAGE_MULTIPLIER, new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.REMOVE_ON_DEATH), devourer);
            if (output != null)
            {
                if (output.HasAnyFatal())
                {
                    level.PlaySound(VanillaSoundID.pacmanKill);
                }
                if (!level.IsPlayingSound(VanillaSoundID.pacmanAttack))
                {
                    level.PlaySound(VanillaSoundID.pacmanAttack);
                }
            } 
        }
        #region 更新
        protected override void UpdateLogic(Entity devourer)
        {
            base.UpdateLogic(devourer);
            if (devourer.IsEvoked())
            {
                UpdateEvoked(devourer);
            }
            else
            {
                UpdateNotEvoked(devourer);
            }
        }
        private void UpdateEvoked(Entity devourer)
        {
            if (devourer.State != VanillaEntityStates.CONTRAPTION_SPECIAL)
            {
                UpdateEvokedDevour(devourer);
            }
            else
            {
                UpdateEvokedGhost(devourer);
            }
        }
        private void UpdateEvokedDevour(Entity devourer)
        {
            var devourTimer = GetDevourTimer(devourer);
            var target = devourer.Target;
            if (!target.ExistsAndAlive())
            {
                StartPacmanGhost(devourer);
            }
            else
            {
                devourTimer?.Run(27);
                if (devourTimer == null || devourTimer.Expired)
                {
                    FinishDevour(devourer, target);
                    StartPacmanGhost(devourer);
                }
            }
            UpdateDevourerPosition(devourer);
        }
        private void UpdateEvokedGhost(Entity devourer)
        {
            var level = devourer.Level;

            #region 确定移动方向
            var targetGridIndex = GetTargetGridIndex(devourer);
            var targetGridCol = level.GetGridColumnByIndex(targetGridIndex);
            var targetGridLane = level.GetGridLaneByIndex(targetGridIndex);
            var targetGridX = level.GetEntityColumnX(targetGridCol);
            var targetGridZ = level.GetEntityLaneZ(targetGridLane);
            var targetGridY = level.GetGroundY(targetGridX, targetGridZ);
            var targetGridPosition = new Vector3(targetGridX, targetGridY, targetGridZ);
            var targetGridDistance = targetGridPosition - devourer.Position;
            if (targetGridDistance.magnitude <= EVOKED_MOVE_SPEED)
            {
                // 更新目标。
                devourer.Position = targetGridPosition;

                FindPacmanGhostTarget(devourer);
            }
            #endregion

            devourer.Position += targetGridDistance.normalized * EVOKED_MOVE_SPEED;

            var timer = GetDevourTimer(devourer);
            timer?.Run();
            if (timer == null || timer.Expired)
            {
                devourer.Die(new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE), devourer);
            }
        }
        private void FindPacmanGhostTarget(Entity devourer)
        {
            var level = devourer.Level;
            var target = evokedDetector.DetectEntityWithTheLeast(devourer, e => Vector3.SqrMagnitude(e.Position - devourer.Position));
            devourer.Target = target;

            var lane = devourer.GetLane();
            var column = devourer.GetColumn();
            var x = devourer.Position.x;

            Vector2Int newTargetGridOffset = Vector2Int.zero;

            if (target.ExistsAndAlive())
            {
                var targetLane = target.GetLane();
                var targetX = target.Position.x;
                if (targetLane < lane)
                {
                    if (lane > 0)
                    {
                        newTargetGridOffset = new Vector2Int(0, -1);
                    }
                }
                else if (targetLane > lane)
                {
                    if (lane < level.GetMaxLaneCount())
                    {
                        newTargetGridOffset = new Vector2Int(0, 1);
                    }
                }
                else if (targetX > x)
                {
                    if (column < level.GetMaxColumnCount())
                    {
                        newTargetGridOffset = new Vector2Int(1, 0);
                    }
                }
                else
                {
                    if (column > 0)
                    {
                        newTargetGridOffset = new Vector2Int(-1, 0);
                    }
                }
            }
            else
            {
                var rng = devourer.RNG;
                List<Vector2Int> possibleDirections = new List<Vector2Int>();

                if (lane > 0)
                {
                    possibleDirections.Add(new Vector2Int(0, -1));
                }
                if (lane < level.GetMaxLaneCount())
                {
                    possibleDirections.Add(new Vector2Int(0, 1));
                }
                if (column < level.GetMaxColumnCount())
                {
                    possibleDirections.Add(new Vector2Int(1, 0));
                }
                if (column > 0)
                {
                    possibleDirections.Add(new Vector2Int(-1, 0));
                }
                newTargetGridOffset = possibleDirections.Random(rng);
            }
            var grid = level.GetGrid(column + newTargetGridOffset.x, lane + newTargetGridOffset.y) ?? devourer.GetGrid();
            SetTargetGridIndex(devourer, grid.GetIndex());
        }
        private void StartPacmanGhost(Entity devourer)
        {
            var devourTimer = GetDevourTimer(devourer);
            devourTimer?.ResetTime(EVOCATION_DURATION);
            devourer.State = VanillaEntityStates.CONTRAPTION_SPECIAL;
            FindPacmanGhostTarget(devourer);
            devourer.SetAnimationBool("Mill", false);
            devourer.CollisionMaskHostile |= EntityCollisionHelper.MASK_VULNERABLE;
        }
        private void UpdateNotEvoked(Entity devourer)
        {
            if (!devourer.Target.ExistsAndAlive())
            {
                devourer.Die(new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE), devourer);
                return;
            }
            UpdateDevourerPosition(devourer);

            var devourTimer = GetDevourTimer(devourer);
            devourTimer?.Run();
            if (devourTimer == null || devourTimer.Expired)
            {
                var target = devourer.Target;
                FinishDevour(devourer, target);
                devourer.Remove();
            }
        }
        private void FinishDevour(Entity devourer, Entity target)
        {
            if (!target.ExistsAndAlive())
                return;

            if (target.IsEntityOf(VanillaObstacleID.monsterSpawner))
            {
                target.Remove();
                devourer.Produce(VanillaPickupID.emerald);
            }
            else
            {
                target.Remove();
                var spawnParams = devourer.GetSpawnParams();
                var entityID = target.GetDefinitionID();
                var blueprintID = VanillaBlueprintID.FromEntity(entityID);
                spawnParams.SetProperty(BlueprintPickup.PROP_BLUEPRINT_ID, blueprintID);
                devourer.Produce(VanillaPickupID.blueprintPickup, spawnParams);
            }
        }
        private void UpdateDevourerPosition(Entity devourer)
        {
            if (devourer.State != VanillaEntityStates.CONTRAPTION_SPECIAL)
            {
                if (devourer.Target != null)
                {
                    var timer = GetDevourTimer(devourer);
                    var percent = timer?.GetTimeoutPercentage() ?? 0;
                    var pos = devourer.Target.Position;
                    pos.y += percent * DEVOUR_START_HEIGHT;
                    pos.z -= 0.01f;
                    devourer.Position = pos;
                }
            }
        }
        #endregion

        #region 目标
        public static Entity GetMillTarget(Entity devourer)
        {
            var grid = devourer.GetGrid();
            if (grid == null)
                return null;

            var gridEntities = grid.GetEntities();
            var spawner = gridEntities.FirstOrDefault(e => e.IsEntityOf(VanillaObstacleID.monsterSpawner));
            if (spawner.ExistsAndAlive())
                return spawner;

            var layers = grid.GetLayers();
            var orderedLayers = VanillaGridLayers.devourerLayers;
            foreach (var layer in orderedLayers)
            {
                var entity = grid.GetLayerEntity(layer);
                if (entity == devourer)
                    continue;
                if (!CanMill(entity))
                    continue;
                return entity;
            }
            return null;
        }
        public static bool CanMill(Entity entity)
        {
            if (!entity.ExistsAndAlive())
                return false;
            if (!entity.IsEntityOf(VanillaObstacleID.monsterSpawner) && entity.Type != EntityTypes.PLANT)
                return false;
            return true;
        }
        #endregion

        #region 属性
        public static FrameTimer GetDevourTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_DEVOUR_TIMER);
        public static void SetDevourTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_DEVOUR_TIMER, timer);
        public static int GetTargetGridIndex(Entity entity) => entity.GetBehaviourField<int>(PROP_TARGET_GRID_INDEX);
        public static void SetTargetGridIndex(Entity entity, int value) => entity.SetBehaviourField(PROP_TARGET_GRID_INDEX, value);
        #endregion
        public static readonly VanillaEntityPropertyMeta PROP_DEVOUR_TIMER = new VanillaEntityPropertyMeta("devourTimer");
        public static readonly VanillaEntityPropertyMeta PROP_TARGET_GRID_INDEX = new VanillaEntityPropertyMeta("targetGridIndex");
        public Detector evokedDetector;
        public const int DEVOUR_TIME = 135;
        public const int EVOCATION_DURATION = 450;
        public const float EVOKED_MOVE_SPEED = 4;
        public const float EVOKED_DAMAGE_MULTIPLIER = 0.05f;
        public const float DEVOUR_START_HEIGHT = 48f;
    }
}
