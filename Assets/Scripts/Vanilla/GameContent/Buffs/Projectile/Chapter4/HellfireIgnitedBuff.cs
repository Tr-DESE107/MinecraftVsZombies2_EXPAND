using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Projectiles
{
    [BuffDefinition(VanillaBuffNames.hellfireIgnited)]
    public class HellfireIgnitedBuff : BuffDefinition
    {
        public HellfireIgnitedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.IS_LIGHT_SOURCE, true));
            AddModifier(new Vector3Modifier(VanillaEntityProps.LIGHT_RANGE, NumberOperator.Add, Vector3.one * 20));
            AddModifier(ColorModifier.Override(VanillaEntityProps.LIGHT_COLOR, PROP_LIGHT_COLOR));
            AddTrigger(VanillaLevelCallbacks.POST_PROJECTILE_HIT, PostProjectileHitCallback);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            Uncurse(buff);
        }
        private void PostProjectileHitCallback(VanillaLevelCallbacks.PostProjectileHitParams param, CallbackResult result)
        {
            var hit = param.hit;
            var damage = param.damage;
            var projectile = hit.Projectile;
            var buffs = projectile.GetBuffs(this);
            float additionalDamage = 0;
            bool cursed = false;
            foreach (var buff in buffs)
            {
                additionalDamage += GetDamage(buff);
                if (GetCursed(buff))
                {
                    cursed = true;
                }
            }
            if (additionalDamage <= 0)
                return;
            var target = hit.Other;
            var shield = hit.Shield;
            var armorSlot = shield != null ? shield.Slot : null;
            target.TakeDamage(additionalDamage, new DamageEffectList(VanillaDamageEffects.FIRE), projectile, armorSlot);

            bool blocksFire = damage.WillDamageBlockFire();

            if (!blocksFire)
            {
                var entity = projectile;
                var level = entity.Level;

                var damageEffects = new DamageEffectList(VanillaDamageEffects.FIRE, VanillaDamageEffects.MUTE);
                var center = entity.GetCenter();
                var radius = 40;
                var faction = entity.GetFaction();
                var splashAmount = additionalDamage * 2 / 3f;
                foreach (IEntityCollider entityCollider in level.OverlapSphere(center, radius, faction, EntityCollisionHelper.MASK_VULNERABLE, 0))
                {
                    if (entityCollider == hit.Collider)
                        continue;
                    entityCollider.TakeDamage(splashAmount, damageEffects, entity);
                }

                var effectID = cursed ? VanillaEffectID.cursedFireburn : VanillaEffectID.fireburn;
                entity.Spawn(effectID, entity.Position);
            }
        }
        public static void Curse(Buff buff)
        {
            SetDamage(buff, DAMAGE_ADDITION_CURSED);
            SetLightColor(buff, LIGHT_COLOR_CURSED);
            SetCursed(buff, true);
        }
        public static void Uncurse(Buff buff)
        {
            SetDamage(buff, DAMAGE_ADDITION);
            SetLightColor(buff, LIGHT_COLOR);
            SetCursed(buff, false);
        }
        public static void SetDamage(Buff buff, float value) => buff.SetProperty(PROP_DAMAGE, value);
        public static float GetDamage(Buff buff) => buff.GetProperty<float>(PROP_DAMAGE);
        public static void SetLightColor(Buff buff, Color value) => buff.SetProperty(PROP_LIGHT_COLOR, value);
        public static Color GetLightColor(Buff buff) => buff.GetProperty<Color>(PROP_LIGHT_COLOR);
        public static void SetCursed(Buff buff, bool value) => buff.SetProperty(PROP_CURSED, value);
        public static bool GetCursed(Buff buff) => buff.GetProperty<bool>(PROP_CURSED);
        public static readonly VanillaBuffPropertyMeta PROP_DAMAGE = new VanillaBuffPropertyMeta("damage");
        public static readonly VanillaBuffPropertyMeta PROP_LIGHT_COLOR = new VanillaBuffPropertyMeta("lightColor");
        public static readonly VanillaBuffPropertyMeta PROP_CURSED = new VanillaBuffPropertyMeta("cursed");
        public const float DAMAGE_ADDITION = 20;
        public const float DAMAGE_ADDITION_CURSED = 40;
        public static readonly Color LIGHT_COLOR = new Color(1, 0.5f, 0);
        public static readonly Color LIGHT_COLOR_CURSED = new Color(0, 1, 0);
    }
}
