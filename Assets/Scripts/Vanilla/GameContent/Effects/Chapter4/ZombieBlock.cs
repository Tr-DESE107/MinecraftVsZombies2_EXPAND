﻿using System.Collections.Generic;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.zombieBlock)]
    public class ZombieBlock : EffectBehaviour
    {

        #region 公有方法
        public ZombieBlock(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetMoveCooldownTimer(entity, new FrameTimer(0));
            entity.CollisionMaskHostile |= EntityCollisionHelper.MASK_VULNERABLE;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var parent = entity.Parent;
            if (parent == null || !parent.Exists())
            {
                entity.Remove();
                return;
            }


            var mode = GetMode(entity);
            switch (mode)
            {
                case MODE_FLY:
                case MODE_TRANSFORM:
                    {
                        var cooldownTimer = GetMoveCooldownTimer(entity);
                        cooldownTimer.Run();
                        if (!cooldownTimer.Expired)
                        {
                            var startPosition = GetStartPosition(entity);
                            entity.Position = entity.Position * 0.7f + startPosition * 0.3f;
                        }
                        else
                        {
                            var targetPosition = GetTargetPosition(entity);
                            if (!IsReached(entity))
                            {
                                var vel = (targetPosition - entity.Position).normalized * MOVE_SPEED;
                                entity.Velocity = vel;
                            }
                            else
                            {
                                entity.Velocity = Vector3.zero;
                                entity.Position = entity.Position * 0.7f + targetPosition * 0.3f;
                            }
                        }
                    }
                    break;
                case MODE_JUMP:
                    {
                        var cooldownTimer = GetMoveCooldownTimer(entity);
                        cooldownTimer.Run();
                        if (!cooldownTimer.Expired)
                        {
                            var startPosition = GetStartPosition(entity);
                            entity.Position = entity.Position * 0.7f + startPosition * 0.3f;
                        }
                        else if (entity.IsOnGround)
                        {
                            var targetPosition = GetTargetPosition(entity);
                            if (IsReached(entity))
                            {
                                entity.Velocity = Vector3.zero;
                            }
                            else
                            {
                                var jumpDistance = GetJumpDistance(entity);

                                var gravity = entity.GetGravity();
                                var distance = targetPosition - entity.Position;

                                jumpDistance *= Mathf.Sign(distance.x);
                                if (distance.x > 0 && jumpDistance > distance.x)
                                {
                                    jumpDistance = distance.x;
                                }
                                else if (distance.x < 0 && jumpDistance < distance.x)
                                {
                                    jumpDistance = distance.x;
                                }
                                var nextX = entity.Position.x + jumpDistance;
                                var nextColumn = entity.Level.GetColumn(nextX);
                                nextX = entity.Level.GetEntityColumnX(nextColumn);
                                var nextZ = entity.Position.z;
                                var nextY = entity.Level.GetGroundY(nextX, nextZ);
                                var nextPosition = new Vector3(nextX, nextY, nextZ);

                                var maxY = nextY + 80;

                                entity.Velocity = VanillaProjectileExt.GetLobVelocity(entity.Position, nextPosition, maxY, gravity);
                            }

                            jumpBuffer.Clear();
                            jumpDetector.DetectMultiple(entity, jumpBuffer);
                            foreach (var collider in jumpBuffer)
                            {
                                var other = collider.Entity;
                                if (entity.IsHostile(other))
                                {
                                    collider.TakeDamage(entity.GetDamage(), new DamageEffectList(), entity);
                                }
                            }
                        }
                    }
                    break;
                case MODE_SNAKE_FOOD:
                    {
                        var startPosition = GetStartPosition(entity);
                        entity.Position = entity.Position * 0.7f + startPosition * 0.3f;
                    }
                    break;
            }
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (state != EntityCollisionHelper.STATE_ENTER)
                return;
            var block = collision.Entity;
            var other = collision.Other;
            var mode = GetMode(block);
            switch (mode)
            {
                case MODE_FLY:
                    if (block.IsHostile(other) && other.CanDeactive())
                    {
                        other.Stun(STUN_DURATION);
                        other.PlaySound(VanillaSoundID.punch);
                    }
                    break;
            }
        }
        public static bool IsReached(Entity entity)
        {
            var targetPos = GetTargetPosition(entity);
            return (targetPos - entity.Position).sqrMagnitude <= MOVE_SPEED * MOVE_SPEED;
        }
        public static void SetMoveCooldown(Entity entity, int time)
        {
            var moveTimer = GetMoveCooldownTimer(entity);
            if (moveTimer == null)
                return;
            moveTimer.ResetTime(time);
        }
        public static int GetMode(Entity entity) => entity.GetBehaviourField<int>(PROP_MODE);
        public static void SetMode(Entity entity, int value) => entity.SetBehaviourField(PROP_MODE, value);
        public static FrameTimer GetMoveCooldownTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_MOVE_COOLDOWN_TIMER);
        public static void SetMoveCooldownTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(PROP_MOVE_COOLDOWN_TIMER, value);
        public static float GetJumpDistance(Entity entity) => entity.GetBehaviourField<float>(PROP_JUMP_DISTANCE);
        public static void SetJumpDistance(Entity entity, float value) => entity.SetBehaviourField(PROP_JUMP_DISTANCE, value);
        public static Vector3 GetStartPosition(Entity entity) => entity.GetBehaviourField<Vector3>(PROP_START_POSITION);
        public static void SetStartPosition(Entity entity, Vector3 value) => entity.SetBehaviourField(PROP_START_POSITION, value);
        public static void SetStartGrid(Entity entity, int column, int lane) => SetStartPosition(entity, entity.Level.GetEntityGridPosition(column, lane));
        public static Vector3 GetTargetPosition(Entity entity) => entity.GetBehaviourField<Vector3>(PROP_TARGET_POSITION);
        public static void SetTargetPosition(Entity entity, Vector3 value) => entity.SetBehaviourField(PROP_TARGET_POSITION, value);
        public static void SetTargetGrid(Entity entity, int column, int lane) => entity.SetBehaviourField(PROP_TARGET_POSITION, entity.Level.GetEntityGridPosition(column, lane));

        public const float MOVE_SPEED = 20;
        public const int STUN_DURATION = 90;

        public const int MODE_FLY = 0;
        public const int MODE_JUMP = 1;
        public const int MODE_TRANSFORM = 2;
        public const int MODE_SNAKE_FOOD = 3;

        private Detector jumpDetector = new CollisionDetector();
        private List<IEntityCollider> jumpBuffer = new List<IEntityCollider>();

        private static readonly VanillaEntityPropertyMeta<int> PROP_MODE = new VanillaEntityPropertyMeta<int>("Mode");
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_MOVE_COOLDOWN_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("MoveCooldownTimer");
        private static readonly VanillaEntityPropertyMeta<float> PROP_JUMP_DISTANCE = new VanillaEntityPropertyMeta<float>("JumpDistance");
        private static readonly VanillaEntityPropertyMeta<Vector3> PROP_START_POSITION = new VanillaEntityPropertyMeta<Vector3>("StartPosition");
        private static readonly VanillaEntityPropertyMeta<Vector3> PROP_TARGET_POSITION = new VanillaEntityPropertyMeta<Vector3>("TargetPosition");
    }
}