﻿using System.Collections.Generic;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.blackhole)]
    public class Blackhole : EffectBehaviour
    {

        #region 公有方法
        public Blackhole(string nsp, string name) : base(nsp, name)
        {
            absorbDetector = new BlackholeDetector()
            {
                mask = EntityCollisionHelper.MASK_VULNERABLE | EntityCollisionHelper.MASK_PROJECTILE,
                canDetectInvisible = true,
                factionTarget = FactionTarget.Any
            };
        }
        #endregion
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Level.AddLoopSoundEntity(VanillaSoundID.gravitationSurge, entity.ID);
        }

        public override void Update(Entity entity)
        {
            base.Update(entity);

            entity.SetDisplayScale(Vector3.one * (entity.GetRange() / 80));

            bool active = entity.Timeout > 5;
            entity.SetAnimationBool("Started", active);
            if (!active)
                return;

            // Absorb.
            detectBuffer.Clear();
            absorbDetector.DetectMultiple(entity, detectBuffer);
            foreach (var collider in detectBuffer)
            {
                var target = collider.Entity;
                bool hostile = target.IsHostile(entity);
                if (collider.IsMainCollider())
                {
                    if (target.Type == EntityTypes.BOSS)
                    {
                        if (hostile)
                        {
                            if (target.IsEntityOf(VanillaBossID.theGiantSnakeTail) || (target.IsEntityOf(VanillaBossID.theGiant) && TheGiant.CanAttractByBlackhole(target)))
                            {
                                snakeBuffer.Clear();
                                TheGiantSnakeTail.GetFullSnake(target, snakeBuffer);
                                foreach (var segment in snakeBuffer)
                                {
                                    segment.Velocity = Vector3.zero;
                                    var newCenter = segment.GetCenter() * 0.9f + entity.GetCenter() * 0.1f;
                                    segment.SetCenter(newCenter);
                                }
                            }
                        }
                    }
                    else if (target.Type == EntityTypes.ENEMY)
                    {
                        if (hostile)
                        {
                            target.Velocity = Vector3.zero;
                            var newCenter = target.GetCenter() * 0.7f + entity.GetCenter() * 0.3f;
                            target.SetCenter(newCenter);
                            target.StopChangingLane();
                        }
                    }
                    else if (target.Type == EntityTypes.PROJECTILE)
                    {
                        var newCenter = target.GetCenter() * 0.7f + entity.GetCenter() * 0.3f;
                        target.SetCenter(newCenter);
                    }
                }
                if (hostile && target.IsVulnerableEntity())
                {
                    collider.TakeDamage(entity.GetDamage(), new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN), entity);
                }
            }
        }
        private List<IEntityCollider> detectBuffer = new List<IEntityCollider>();
        private List<Entity> snakeBuffer = new List<Entity>();
        private Detector absorbDetector;
    }
}