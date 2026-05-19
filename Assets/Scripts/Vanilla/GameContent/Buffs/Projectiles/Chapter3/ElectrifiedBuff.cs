using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;

using MVZ2Logic.Models;

using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

using UnityEngine;

namespace MVZ2.GameContent.Buffs.Projectiles
{
    [BuffDefinition(VanillaBuffNames.Projectile.Electrified)]
    public class ElectrifiedBuff : BuffDefinition
    {
        public ElectrifiedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.IS_LIGHT_SOURCE, true));
            AddModifier(new Vector3Modifier(VanillaEntityProps.LIGHT_RANGE, NumberOperator.Add, PROP_LIGHT_RANGE_ADDITION));
            AddModifier(ColorModifier.Override(VanillaEntityProps.LIGHT_COLOR, LIGHT_COLOR));
            AddTrigger(VanillaLevelCallbacks.POST_PROJECTILE_HIT, PostProjectileHitCallback);
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.staticParticles, VanillaModelID.staticParticles);
        }
        private void PostProjectileHitCallback(VanillaLevelCallbacks.PostProjectileHitParams param, CallbackResult result)
        {
            var hit = param.hit;
            var damage = param.damage;
            var projectile = hit.Projectile;
            var buffs = projectile.GetBuffs(this);

            var buffCount = projectile.GetBuffCount<ElectrifiedBuff>();
            if (buffCount == 0)
                return;

            float additionalDamage = projectile.GetDamage() * 0.5f; // ∂ÓÕ‚50%…À∫¶    

            var target = hit.Other;
            var shield = hit.Shield;
            var armorSlot = shield != null ? shield.Slot : null;
            target.TakeDamage(additionalDamage, new DamageEffectList(VanillaDamageEffects.LIGHTNING), projectile, armorSlot);

            //// Ω¶…‰…À∫¶    
            //var damageEffects = new DamageEffectList(VanillaDamageEffects.LIGHTNING);
            //var center = entity.GetCenter();
            //var radius = 30;
            //var faction = entity.GetFaction();
            //var splashAmount = additionalDamage * 0.5f;
            //entity.SplashDamage(hit.Collider, center, radius, faction, splashAmount, damageEffects);

            //entity.Spawn(VanillaEffectID.electricArc, entity.Position);
        }
        public static readonly VanillaBuffPropertyMeta<Vector3> PROP_LIGHT_RANGE_ADDITION = new VanillaBuffPropertyMeta<Vector3>("light_range_addition");
        public static readonly Color LIGHT_COLOR = new Color(0.5f, 0.5f, 1);
    }
}