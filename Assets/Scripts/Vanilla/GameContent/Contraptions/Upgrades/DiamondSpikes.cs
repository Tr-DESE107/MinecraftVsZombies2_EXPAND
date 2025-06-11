﻿using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.diamondSpikes)]
    public class DiamondSpikes : SpikesBehaviour
    {
        public DiamondSpikes(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var param = entity.GetShootParams();
            var ySpeed = 10;
            var caltropCount = entity.Level.GetEntityCount(VanillaProjectileID.diamondCaltrop);
            var count = Mathf.Min(30, MAX_CALTROPS - caltropCount);
            for (int i = 0; i < count; i++)
            {
                var layer = i / 10;
                var index = i % 10;
                var angle = index * 36;
                var speed = 3 + layer * 2;
                var velocity2D = Vector2.right.RotateClockwise(angle) * speed;
                param.position = entity.Position + Vector3.up * 16;
                param.projectileID = VanillaProjectileID.diamondCaltrop;
                param.velocity = new Vector3(velocity2D.x, ySpeed, velocity2D.y);
                param.damage = entity.GetDamage() * 5;
                entity.ShootProjectile(param);
            }
            entity.PlaySound(VanillaSoundID.fling);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();
        }
        public const int MAX_CALTROPS = 100;
        public override NamespaceID SpikeParticleID => VanillaEffectID.diamondSpikeParticles;
        public override int AttackCooldown => 15;
        public override int EvocationAttackCooldown => 2;
    }
}
