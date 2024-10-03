using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Shells;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaEntityExt
    {
        public static int GetHealthState(this Entity entity, int stateCount)
        {
            float maxHP = entity.GetMaxHealth();
            float stateHP = maxHP / stateCount;
            return Mathf.CeilToInt(entity.Health / stateHP) - 1;
        }
        public static void PlayHitSound(this Entity entity, DamageEffectList damageEffects, ShellDefinition shell)
        {
            if (entity == null || shell == null)
                return;
            var level = entity.Level;
            var blocksFire = shell.GetProperty<bool>(VanillaShellProps.BLOCKS_FIRE);
            var hitSound = shell.GetProperty<NamespaceID>(VanillaShellProps.HIT_SOUND);
            if (damageEffects.HasEffect(VanillaDamageEffects.FIRE) && !blocksFire)
            {
                entity.PlaySound(SoundID.fire);
            }
            else if (damageEffects.HasEffect(VanillaDamageEffects.SLICE) && shell.GetProperty<bool>(VanillaShellProps.SLICE_CRITICAL))
            {
                entity.PlaySound(SoundID.slice);
            }
            else
            {
                entity.PlaySound(hitSound);
            }
        }
        public static Vector3 ModifyProjectileVelocity(this Entity entity, Vector3 velocity)
        {
            return velocity;
        }
        public static bool IsAliveEnemy(this Entity entity)
        {
            return entity.Type == EntityTypes.ENEMY && !entity.IsDead && !entity.GetProperty<bool>(BuiltinEnemyProps.HARMLESS) && entity.IsEnemy(entity.Level.Option.LeftFaction);
        }
        public static DamageResult TakeDamage(this Entity entity, float amount, DamageEffectList effects, EntityReferenceChain source)
        {
            return entity.TakeDamage(amount, effects, source, out _);
        }
        public static DamageResult TakeDamage(this Entity entity, float amount, DamageEffectList effects, EntityReferenceChain source, out DamageResult armorResult)
        {
            return TakeDamage(new DamageInfo(amount, effects, entity, source), out armorResult);
        }
        public static DamageResult TakeDamage(DamageInfo info)
        {
            return TakeDamage(info, out _);
        }
        public static DamageResult TakeDamage(DamageInfo info, out DamageResult armorResult)
        {
            armorResult = null;
            if (info.Entity.IsInvincible() || info.Entity.IsDead)
                return null;
            if (!PreTakeDamage(info))
                return null;
            if (info.Amount <= 0)
                return null;
            DamageResult bodyResult;
            if (Armor.Exists(info.Entity.EquipedArmor) && !info.Effects.HasEffect(VanillaDamageEffects.IGNORE_ARMOR))
            {
                bodyResult = ArmoredTakeDamage(info, out armorResult);
            }
            else
            {
                bodyResult = BodyTakeDamage(info);
            }
            PostTakeDamage(bodyResult, armorResult);
            return bodyResult;
        }
        private static bool PreTakeDamage(DamageInfo damageInfo)
        {
            return true;
        }
        private static void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            Entity entity = null;
            if (bodyResult != null)
            {
                entity = bodyResult.Entity;
            }
            else if (armorResult != null)
            {
                entity = armorResult.Entity;
            }
            if (entity == null)
                return;
            entity.Definition.PostTakeDamage(bodyResult, armorResult);
            LevelCallbacks.PostEntityTakeDamage.Run(bodyResult, armorResult);
        }
        private static DamageResult ArmoredTakeDamage(DamageInfo info, out DamageResult armorResult)
        {
            var entity = info.Entity;
            armorResult = Armor.TakeDamage(info);
            if (info.Effects.HasEffect(VanillaDamageEffects.DAMAGE_BOTH_ARMOR_AND_BODY))
            {
                return BodyTakeDamage(info);
            }
            else if (info.Effects.HasEffect(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN) && !Armor.Exists(entity.EquipedArmor))
            {
                float overkillDamage = armorResult != null ? info.Amount - armorResult.UsedDamage : info.Amount;
                if (overkillDamage > 0)
                {
                    var overkillInfo = new DamageInfo(overkillDamage, info.Effects, entity, info.Source);
                    return BodyTakeDamage(overkillInfo);
                }
            }
            return null;
        }
        private static DamageResult BodyTakeDamage(DamageInfo info)
        {
            var entity = info.Entity;
            var shellRef = entity.GetShellID();
            var shell = entity.Level.ContentProvider.GetShellDefinition(shellRef);
            if (shell != null)
            {
                shell.EvaluateDamage(info);
            }


            // Calculate used damage.
            float usedDamage = info.GetUsedDamage();

            // Apply Damage.
            float hpBefore = entity.Health;
            entity.Health -= info.Amount;
            if (entity.Health <= 0)
            {
                entity.Die(info);
            }

            return new DamageResult()
            {
                OriginalDamage = info.OriginalDamage,
                Amount = info.Amount,
                UsedDamage = info.GetUsedDamage(),
                Entity = entity,
                Effects = info.Effects,
                Source = info.Source,
                ShellDefinition = shell,
                Fatal = hpBefore > 0 && entity.Health <= 0
            };
        }

    }
}
