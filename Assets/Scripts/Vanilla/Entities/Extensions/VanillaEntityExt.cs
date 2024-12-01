using System.Linq;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Shells;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Networking.UnityWebRequest;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEntityExt
    {
        #region 面朝方向
        public static float GetFacingX(this Entity entity)
        {
            return entity.IsFacingLeft() ? -1 : 1;
        }
        public static Vector3 GetFacingDirection(this Entity entity)
        {
            return entity.IsFacingLeft() ? Vector3.left : Vector3.right;
        }
        #endregion

        #region 伤害和血量
        public static int GetHealthState(this Entity entity, int stateCount)
        {
            float maxHP = entity.GetMaxHealth();
            float stateHP = maxHP / stateCount;
            return Mathf.CeilToInt(entity.Health / stateHP) - 1;
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
            var triggers = damageInfo.Entity.Level.Triggers.GetTriggers(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE);
            foreach (var trigger in triggers)
            {
                trigger.Invoke(damageInfo);
                if (damageInfo.Canceled)
                {
                    return false;
                }
            }
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
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, bodyResult, armorResult);
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
            var shell = entity.Level.Content.GetShellDefinition(shellRef);
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
        #endregion

        #region 投射物
        public static Vector3 ModifyProjectileVelocity(this Entity entity, Vector3 velocity)
        {
            return velocity;
        }
        #endregion

        #region 光照
        public static bool IsIlluminated(this Entity entity)
        {
            if (entity == null)
                return false;
            return entity.Level.IsIlluminated(entity);
        }
        public static Entity GetIlluminationLightSource(this Entity entity)
        {
            if (entity == null)
                return null;
            var level = entity.Level;
            var entityID = level.GetIlluminationLightSourceID(entity);
            return level.FindEntityByID(entityID);
        }
        public static Entity[] GetIlluminationLightSources(this Entity entity)
        {
            if (entity == null)
                return null;
            var level = entity.Level;
            var entitiesID = level.GetIlluminationLightSources(entity);
            return entitiesID.Select(e => level.FindEntityByID(e)).ToArray();
        }
        #endregion

        #region 特效
        public static void UpdateShineRing(this Entity entity)
        {
            var lightSource = entity.IsLightSource();
            if (!lightSource)
                return;
            var shineRingID = entity.GetProperty<EntityID>("LightShineRing");
            var shineRing = shineRingID?.GetEntity(entity.Level);
            if (shineRing != null && shineRing.Exists())
                return;
            shineRing = entity.Level.FindFirstEntity(e => e.IsEntityOf(VanillaEffectID.shineRing) && e.Parent == entity);
            if (shineRing != null && shineRing.Exists())
                return;
            shineRing = entity.Level.Spawn(VanillaEffectID.shineRing, entity.Position, entity);
            shineRing.SetParent(entity);
            entity.SetProperty("LightShineRing", new EntityID(shineRing));
        }
        #endregion

        #region 音效

        public static void PlaySound(this Entity entity, NamespaceID soundID, float pitch = 1)
        {
            entity.Level.PlaySound(soundID, entity.Position, pitch);
        }
        public static void PlayHitSound(this Entity entity, DamageEffectList damageEffects, ShellDefinition shell)
        {
            if (entity == null || shell == null)
                return;
            var level = entity.Level;
            var blocksFire = shell.BlocksFire();
            if (damageEffects.HasEffect(VanillaDamageEffects.FIRE) && !blocksFire)
            {
                entity.PlaySound(VanillaSoundID.fire);
            }
            else if (damageEffects.HasEffect(VanillaDamageEffects.SLICE) && shell.GetProperty<bool>(VanillaShellProps.SLICE_CRITICAL))
            {
                entity.PlaySound(VanillaSoundID.slice);
            }
            else
            {
                var hitSound = entity.GetHitSound();
                if (NamespaceID.IsValid(hitSound))
                {
                    entity.PlaySound(hitSound);
                }
                else
                {
                    var shellHitSound = shell.GetProperty<NamespaceID>(VanillaShellProps.HIT_SOUND);
                    entity.PlaySound(shellHitSound);
                }
            }
        }

        #endregion

        #region 换行
        public static void StartChangingLane(this Entity entity, int target)
        {
            var changeLane = entity.Definition.GetBehaviour<IChangeLaneEntity>();
            if (changeLane == null)
                return;
            if (target < 0 || target >= entity.Level.GetMaxLaneCount())
                return;
            changeLane.SetChangingLane(entity, true);
            changeLane.SetChangeLaneTarget(entity, target);
            changeLane.SetChangeLaneSource(entity, entity.GetLane());
            changeLane.PostStartChangingLane(entity, target);
        }
        public static void StopChangingLane(this Entity entity)
        {
            var changeLane = entity.Definition.GetBehaviour<IChangeLaneEntity>();
            if (changeLane == null)
                return;
            if (!changeLane.IsChangingLane(entity))
                return;
            changeLane.SetChangingLane(entity, false);
            changeLane.SetChangeLaneTarget(entity, 0);
            changeLane.SetChangeLaneSource(entity, 0);
            changeLane.PostStopChangingLane(entity);
        }
        #endregion
        public static bool IsVulnerableEntity(this Entity entity)
        {
            return entity.Type == EntityTypes.PLANT || entity.Type == EntityTypes.ENEMY || entity.Type == EntityTypes.OBSTACLE || entity.Type == EntityTypes.BOSS;
        }
        public static bool IsAliveEnemy(this Entity entity)
        {
            return entity.Type == EntityTypes.ENEMY && !entity.IsDead && !entity.GetProperty<bool>(VanillaEnemyProps.HARMLESS) && entity.IsHostile(entity.Level.Option.LeftFaction);
        }
        public static EntitySeed GetEntitySeedDefinition(this Entity entity)
        {
            var game = Global.Game;
            var seedDef = game.GetSeedDefinition(entity.GetDefinitionID());
            if (seedDef is EntitySeed entitySeed)
                return entitySeed;
            return null;
        }
        public static void Stun(this Entity entity, int timeout)
        {
            if (entity == null)
                return;
            var buff = entity.AddBuff<StunBuff>();
            buff.SetProperty(StunBuff.PROP_TIMER, new FrameTimer(timeout));
        }

        #region 治疗
        public static HealResult HealEffects(this Entity entity, float amount, Entity source)
        {
            var result = entity.Heal(amount, source);
            if (result.RealAmount >= 0)
            {
                entity.AddTickHealing(result.RealAmount);
            }
            return result;
        }
        public static HealResult Heal(this Entity entity, float amount, Entity source)
        {
            return entity.Heal(amount, new EntityReferenceChain(source));
        }
        public static HealResult Heal(this Entity entity, float amount, EntityReferenceChain source)
        {
            return Heal(new HealInfo(amount, entity, source));
        }
        public static HealResult HealArmor(this Entity entity, float amount, Entity source)
        {
            return entity.HealArmor(amount, new EntityReferenceChain(source));
        }
        public static HealResult HealArmor(this Entity entity, float amount, EntityReferenceChain source)
        {
            var armor = entity.EquipedArmor;
            if (armor == null)
                return null;
            return Heal(new HealInfo(amount, entity, armor, source));
        }
        public static HealResult Heal(HealInfo info)
        {
            if (info.Entity.IsDead)
                return null;
            if (!PreHeal(info))
                return null;
            if (info.Amount <= 0)
                return null;
            HealResult result;
            if (info.ToArmor)
            {
                result = ArmorHeal(info);
            }
            else
            {
                result = BodyHeal(info);
            }
            PostHeal(result);
            return result;
        }
        private static bool PreHeal(HealInfo info)
        {
            var entity = info.Entity;
            if (entity == null)
                return false;
            var triggers = entity.Level.Triggers.GetTriggers(VanillaLevelCallbacks.PRE_ENTITY_HEAL);
            foreach (var trigger in triggers)
            {
                trigger.Invoke(info);
                if (info.Canceled)
                {
                    return false;
                }
            }
            return true;
        }
        private static void PostHeal(HealResult result)
        {
            var entity = result.Entity;
            if (entity == null)
                return;
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_HEAL, result);
        }
        private static HealResult ArmorHeal(HealInfo info)
        {
            var armor = info.Armor;
            // Apply Healing.
            float hpBefore = armor.Health;
            var maxHealth = armor.GetMaxHealth();
            if (armor.Health < maxHealth)
            {
                armor.Health = Mathf.Min(armor.Health + info.Amount, maxHealth);
            }

            return new HealResult()
            {
                OriginalAmount = info.OriginalAmount,
                Amount = info.Amount,
                RealAmount = armor.Health - hpBefore,
                Entity = armor.Owner,
                Armor = armor,
                ToArmor = true,
                Source = info.Source,
            };
        }
        private static HealResult BodyHeal(HealInfo info)
        {
            var entity = info.Entity;

            // Apply Healing.
            float hpBefore = entity.Health;
            var maxHealth = entity.GetMaxHealth();
            if (entity.Health < maxHealth)
            {
                entity.Health = Mathf.Min(entity.Health + info.Amount, maxHealth);
            }

            return new HealResult()
            {
                OriginalAmount = info.OriginalAmount,
                Amount = info.Amount,
                RealAmount = entity.Health - hpBefore,
                Entity = entity,
                Source = info.Source,
            };
        }
        #endregion
    }
}
