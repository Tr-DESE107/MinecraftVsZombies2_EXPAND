using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
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

        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_HIT_COUNT, 0); // 初始化命中计数  
            UpdateLightRange(buff);
        }

        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateLightRange(buff);
        }

        private void UpdateLightRange(Buff buff)
        {
            var entity = buff.GetEntity();
            float scale = 1;
            if (entity != null)
            {
                var scaledSize = entity.GetScaledSize();
                var scaleVec = scaledSize / DEFAULT_SCALE;
                scale = Mathf.Max(scaleVec.x, scaleVec.y, scaleVec.z);
            }
            buff.SetProperty(PROP_LIGHT_RANGE_ADDITION, Vector3.one * 20 * scale);
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

            float additionalDamage = projectile.GetDamage() * 0.5f;

            var target = hit.Other;
            //var shield = hit.Shield;
            //var armorSlot = shield != null ? shield.Slot : null;
            //target.TakeDamage(additionalDamage, new DamageEffectList(VanillaDamageEffects.LIGHTNING), projectile, armorSlot);

            

            // 溅射伤害      
            var entity = projectile;
            var level = entity.Level;
            var damageEffects = new DamageEffectList(VanillaDamageEffects.LIGHTNING, VanillaDamageEffects.DAMAGE_BOTH_ARMOR_AND_BODY);
            var center = entity.GetCenter();
            var radius = 20;
            var faction = entity.GetFaction();
            var splashAmount = additionalDamage;
            entity.SplashDamage(hit.Collider, center, radius, faction, splashAmount, damageEffects);

            // 增加命中计数（只记录TakeDamage，不记录SplashDamage）  
            foreach (var buff in buffs)
            {
                int hitCount = buff.GetProperty<int>(PROP_HIT_COUNT);
                hitCount++;
                buff.SetProperty(PROP_HIT_COUNT, hitCount);

                // 达到3次命中后移除buff  
                if (hitCount >= MAX_HIT_COUNT)
                {
                    buff.Remove();
                }
            }

            //entity.Spawn(VanillaEffectID.electricArc, entity.Position);  
        }

        public static readonly VanillaBuffPropertyMeta<Vector3> PROP_LIGHT_RANGE_ADDITION = new VanillaBuffPropertyMeta<Vector3>("light_range_addition");
        public static readonly VanillaBuffPropertyMeta<int> PROP_HIT_COUNT = new VanillaBuffPropertyMeta<int>("hit_count");
        public static readonly Color LIGHT_COLOR = new Color(0.5f, 0.5f, 1);
        public const float DEFAULT_SCALE = 32;
        public const int MAX_HIT_COUNT = 3;
    }
}