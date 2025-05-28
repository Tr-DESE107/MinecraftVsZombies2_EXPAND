﻿using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.parabot)]
    public class Parabot : ProjectileBehaviour
    {
        public Parabot(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
            base.PostHitEntity(hitResult, damage);
            var projectile = hitResult.Projectile;
            if (hitResult.Shield != null)
                return;
            var other = hitResult.Other;
            var buff = other.GetFirstBuff<ParabotBuff>();
            if (buff == null)
            {
                buff = other.AddBuff<ParabotBuff>();
            }
            buff.SetProperty(ParabotBuff.PROP_FACTION, projectile.GetFaction());
            buff.SetProperty(ParabotBuff.PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public const int MAX_TIMEOUT = 180;
    }
}
