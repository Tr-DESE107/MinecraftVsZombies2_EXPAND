﻿using System.Linq;
using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.silvenser)]
    public class Silvenser : DispenserFamily
    {
        public Silvenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            var evocationTimer = new FrameTimer(EVOCATION_DURATION);
            SetEvocationTimer(entity, evocationTimer);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            ShootTick(entity);
            EvokedUpdate(entity);
        }

        // 核心修改：添加随机发射逻辑
        public override Entity Shoot(Entity entity)
        {
            if (entity.RNG.Next(6) == 0)
            {
                var param = entity.GetShootParams();
                // 将 "mvz2:purpleArrow" 拆分为命名空间和名称
                param.projectileID = VanillaProjectileID.RedKnife;
                param.damage *= 2;
                entity.TriggerAnimation("Shoot");
                return entity.ShootProjectile(param);
            }
            return base.Shoot(entity);
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var entities = entity.Level.FindEntities(e => e.IsVulnerableEntity() && !e.IsDead && !e.IsInvisible() && e.IsHostile(entity)).RandomTake(EVOCATION_MAX_TARGET_COUNT, entity.RNG);
            var positions = entities.Select(e => e.GetCenter()).ToArray();
            if (positions.Length > 0)
            {
                var evocationTimer = GetEvocationTimer(entity);
                evocationTimer.Reset();
                entity.SetEvoked(true);

                SetEvocationTargetPositions(entity, positions);
                entity.PlaySound(VanillaSoundID.spellCard);
            }
        }
        public static FrameTimer GetEvocationTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(ID, PROP_EVOCATION_TIMER);
        }
        public static void SetEvocationTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourField(ID, PROP_EVOCATION_TIMER, timer);
        }
        public static Vector3[] GetEvocationTargetPositions(Entity entity)
        {
            return entity.GetBehaviourField<Vector3[]>(ID, PROP_EVOCATION_TARGET_POSITIONS);
        }
        public static void SetEvocationTargetPositions(Entity entity, Vector3[] timer)
        {
            entity.SetBehaviourField(ID, PROP_EVOCATION_TARGET_POSITIONS, timer);
        }
        private void EvokedUpdate(Entity entity)
        {
            var evocationPositions = GetEvocationTargetPositions(entity);
            if (evocationPositions == null || evocationPositions.Length <= 0)
            {
                entity.SetEvoked(false);
                return;
            }
            int knivesPerEnemy = MAX_EVOCATION_KNIFE_COUNT / evocationPositions.Length;
            int layers = Mathf.CeilToInt(knivesPerEnemy / (float)EVOCATION_KNIVES_PER_LAYER);
            int knivesPerLayer = knivesPerEnemy / layers;

            float interval = (float)EVOCATION_DURATION / knivesPerLayer;
            var anglePerSpawn = 180f / knivesPerLayer;
            var anglePerFrame = anglePerSpawn / interval;

            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Run();
            foreach (var frame in evocationTimer.IteratePassedFrames(interval))
            {
                foreach (Vector3 target in evocationPositions)
                {
                    for (int layer = 0; layer < layers; layer++)
                    {
                        var layerRadius = EVOCATION_RADIUS + layer * 24;
                        for (int dir = 0; dir < 2; dir++)
                        {
                            float deg = frame * anglePerFrame;

                            if (dir == 1)
                            {
                                deg += 180;
                            }

                            var direction = Quaternion.Euler(0, deg, 0) * Vector3.right;
                            var posOffset = direction * layerRadius;
                            Vector3 knifePos = target + posOffset;

                            var param = entity.GetSpawnParams();
                            param.SetProperty(VanillaEntityProps.DAMAGE, entity.GetDamage() * EVOCATION_DAMAGE_MULTIPLIER);
                            var projectile = entity.Spawn(entity.GetProjectileID(), knifePos, param);
                            projectile.Velocity = direction * -10;

                            var buff = projectile.AddBuff<ProjectileWaitBuff>();
                            buff.SetProperty(ProjectileWaitBuff.PROP_TIMEOUT, 90);
                        }
                    }
                }
            }
            if (evocationTimer.Expired)
            {
                entity.SetEvoked(false);
            }
        }
        public const int EVOCATION_MAX_TARGET_COUNT = 10;
        public const int MAX_EVOCATION_KNIFE_COUNT = 45;
        public const int EVOCATION_DURATION = 30;
        public const int EVOCATION_KNIVES_PER_LAYER = 30;
        public const float EVOCATION_RADIUS = 100;
        public const float EVOCATION_DAMAGE_MULTIPLIER = 2;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_EVOCATION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("EvocationTimer");
        public static readonly VanillaEntityPropertyMeta<Vector3[]> PROP_EVOCATION_TARGET_POSITIONS = new VanillaEntityPropertyMeta<Vector3[]>("EvocationTargetPositions");

        public static readonly NamespaceID ID = VanillaContraptionID.silvenser;
    }
}
