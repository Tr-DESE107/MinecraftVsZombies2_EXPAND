﻿using System.Collections.Generic;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.seijaCursedDoll)]
    public class SeijaCursedDoll : EnemyBehaviour
    {
        public SeijaCursedDoll(string nsp, string name) : base(nsp, name)
        {
            absorbDetector = new SphereDetector(ABSORB_RADIUS)
            {
                mask = EntityCollisionHelper.MASK_PROJECTILE,
            };
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            if (!entity.Parent.ExistsAndAlive())
            {
                entity.Die(new DamageEffectList(VanillaDamageEffects.NO_NEUTRALIZE), entity);
                return;
            }

            // Orbit.
            var angle = GetOrbitAngle(entity);
            angle += ORBIT_ANGLE_SPEED;
            SetOrbitAngle(entity, angle);

            var orbitOffset = Vector2.right.RotateClockwise(angle) * ORBIT_DISTANCE;
            var targetPosition = entity.Parent.Position + new Vector3(orbitOffset.x, 0, orbitOffset.y);
            targetPosition.y = Mathf.Max(targetPosition.y, entity.Level.GetGroundY(targetPosition));
            entity.Position = targetPosition;

            // Absorb.
            detectBuffer.Clear();
            absorbDetector.DetectEntities(entity, detectBuffer);
            foreach (var target in detectBuffer)
            {
                var vel = target.Velocity;
                var speed = Mathf.Min(vel.magnitude + ABSORB_SPEED, ABSORB_MAX_SPEED);
                vel += (entity.Position - target.Position).normalized * ABSORB_SPEED;
                vel = vel.normalized * speed;
                target.Velocity = vel;
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.SIZE, entity.GetSize());
            var smoke = entity.Spawn(VanillaEffectID.smoke, entity.GetCenter(), param);
            entity.Remove();
        }
        public static float GetOrbitAngle(Entity entity) => entity.GetBehaviourField<float>(PROP_ORBIT_ANGLE);
        public static void SetOrbitAngle(Entity entity, float value) => entity.SetBehaviourField(PROP_ORBIT_ANGLE, value);

        private static readonly VanillaEntityPropertyMeta<float> PROP_ORBIT_ANGLE = new VanillaEntityPropertyMeta<float>("OrbitAngle");
        private List<Entity> detectBuffer = new List<Entity>();
        private Detector absorbDetector;
        public const float ORBIT_DISTANCE = 120;
        public const float ORBIT_ANGLE_SPEED = 2;
        public const float ABSORB_RADIUS = 120;
        public const float ABSORB_SPEED = 5;
        public const float ABSORB_MAX_SPEED = 10;
    }
}
