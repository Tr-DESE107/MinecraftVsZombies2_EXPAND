using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.parabot)]
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
            var buffs = other.GetBuffs<ParabotBuff>();
            Buff buff;
            if (buffs.Length > 0)
            {
                buff = buffs[0];
            }
            else
            {
                buff = other.AddBuff<ParabotBuff>();
            }
            buff.SetProperty(ParabotBuff.PROP_FACTION, projectile.GetFaction());
            buff.SetProperty(ParabotBuff.PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public const int MAX_TIMEOUT = 180;
    }
}
