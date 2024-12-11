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
using UnityEditor;
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
        public static DamageOutput TakeDamage(this Entity entity, float amount, DamageEffectList effects, bool toBody = true, bool toShield = false)
        {
            return entity.TakeDamage(amount, effects, new EntityReferenceChain(null), toBody, toShield);
        }
        public static DamageOutput TakeDamage(this Entity entity, float amount, DamageEffectList effects, Entity source, bool toBody = true, bool toShield = false)
        {
            return entity.TakeDamage(amount, effects, new EntityReferenceChain(source), toBody, toShield);
        }
        public static DamageOutput TakeDamage(this Entity entity, float amount, DamageEffectList effects, EntityReferenceChain source, bool toBody = true, bool toShield = false)
        {
            return TakeDamage(new DamageInput(amount, effects, entity, source, toBody, toShield));
        }
        public static DamageOutput TakeDamage(DamageInput input)
        {
            if (input == null)
                return null;
            if (input.Entity.IsInvincible() || input.Entity.IsDead)
                return null;
            if (!PreTakeDamage(input))
                return null;
            if (input.Amount <= 0)
                return null;
            var result = new DamageOutput()
            {
                Entity = input.Entity
            };
            if (input.ToShield)
            {
                var shield = input.Entity.GetShield();
                if (Armor.Exists(shield))
                {
                    result.ShieldResult = Armor.TakeDamage(input);
                }
            }
            if (input.ToBody)
            {
                if (Armor.Exists(input.Entity.EquipedArmor) && !input.Effects.HasEffect(VanillaDamageEffects.IGNORE_ARMOR))
                {
                    ArmoredTakeDamage(input, result);
                }
                else
                {
                    result.BodyResult = BodyTakeDamage(input);
                }
            }

            PostTakeDamage(result);
            return result;
        }
        private static bool PreTakeDamage(DamageInput damageInfo)
        {
            Entity entity = damageInfo.Entity;
            if (entity == null)
                return false;
            entity.Definition.PreTakeDamage(damageInfo);
            if (damageInfo.Canceled)
            {
                return false;
            }
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
        private static void PostTakeDamage(DamageOutput result)
        {
            Entity entity = result.Entity;
            if (entity == null)
                return;
            entity.Definition.PostTakeDamage(result);
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, result);
        }
        private static void ArmoredTakeDamage(DamageInput info, DamageOutput result)
        {
            var entity = info.Entity;
            var armorResult = Armor.TakeDamage(info);
            result.ArmorResult = armorResult;
            if (info.Effects.HasEffect(VanillaDamageEffects.DAMAGE_BOTH_ARMOR_AND_BODY))
            {
                result.BodyResult = BodyTakeDamage(info);
            }
            else if (info.Effects.HasEffect(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN) && !Armor.Exists(entity.EquipedArmor))
            {
                var armorSpendAmount = armorResult?.SpendAmount ?? 0;
                float overkillDamage = info.Amount - armorSpendAmount;
                if (overkillDamage > 0)
                {
                    var overkillInfo = new DamageInput(overkillDamage, info.Effects, entity, info.Source);
                    result.BodyResult = BodyTakeDamage(overkillInfo);
                }
            }
        }
        private static BodyDamageResult BodyTakeDamage(DamageInput info)
        {
            var entity = info.Entity;
            var shellRef = entity.GetShellID();
            var shell = entity.Level.Content.GetShellDefinition(shellRef);
            if (shell != null)
            {
                shell.EvaluateDamage(info);
            }

            // Apply Damage.
            float hpBefore = entity.Health;
            entity.Health -= info.Amount;
            if (entity.Health <= 0)
            {
                entity.Die(info);
            }

            return new BodyDamageResult()
            {
                OriginalAmount = info.OriginalAmount,
                Amount = info.Amount,
                SpendAmount = Mathf.Min(hpBefore, info.OriginalAmount),
                Entity = entity,
                Effects = info.Effects,
                Source = info.Source,
                ShellDefinition = shell,
                Fatal = hpBefore > 0 && entity.Health <= 0,
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
        public static void PlayHitSound(this DamageOutput damage)
        {
            if (damage == null)
                return;
            var entity = damage.Entity;

            var armorResult = damage.ArmorResult;
            if (armorResult != null && !armorResult.Effects.HasEffect(VanillaDamageEffects.MUTE))
            {
                if (armorResult.Effects.HasEffect(VanillaDamageEffects.WHACK))
                {
                    entity.PlaySound(VanillaSoundID.bonk);
                }
                PlayHitSound(entity, armorResult.Effects, armorResult.ShellDefinition);
            }
            var bodyResult = damage.BodyResult;
            if (bodyResult != null && !bodyResult.Effects.HasEffect(VanillaDamageEffects.MUTE))
            {
                if (bodyResult.Effects.HasEffect(VanillaDamageEffects.WHACK))
                {
                    entity.PlaySound(VanillaSoundID.bonk);
                }
                else
                {
                    PlayHitSound(entity, bodyResult.Effects, bodyResult.ShellDefinition);
                }
            }
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
            else if (damageEffects.HasEffect(VanillaDamageEffects.SLICE) && shell.IsSliceCritical())
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
        public static void EmitBlood(this Entity entity)
        {
            var bloodColor = entity.GetBloodColor();
            var blood = entity.Level.Spawn(VanillaEffectID.bloodParticles, entity.GetCenter(), entity);
            blood.SetTint(bloodColor);
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
        public static bool IsFriendlyEnemy(this Entity entity)
        {
            return entity.IsFriendly(entity.Level.Option.LeftFaction);
        }
        public static bool IsHostileEnemy(this Entity entity)
        {
            return !entity.IsFriendlyEnemy();
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
        public static HealOutput HealEffects(this Entity entity, float amount, Entity source)
        {
            var result = entity.Heal(amount, source);
            if (result.RealAmount >= 0)
            {
                entity.AddTickHealing(result.RealAmount);
            }
            return result;
        }
        public static HealOutput Heal(this Entity entity, float amount, Entity source)
        {
            return entity.Heal(amount, new EntityReferenceChain(source));
        }
        public static HealOutput Heal(this Entity entity, float amount, EntityReferenceChain source)
        {
            return Heal(new HealInput(amount, entity, source));
        }
        public static HealOutput HealArmor(this Entity entity, float amount, Entity source)
        {
            return entity.HealArmor(amount, new EntityReferenceChain(source));
        }
        public static HealOutput HealArmor(this Entity entity, float amount, EntityReferenceChain source)
        {
            var armor = entity.EquipedArmor;
            if (armor == null)
                return null;
            return Heal(new HealInput(amount, entity, armor, source));
        }
        public static HealOutput Heal(HealInput info)
        {
            if (info.Entity.IsDead)
                return null;
            if (!PreHeal(info))
                return null;
            if (info.Amount <= 0)
                return null;
            HealOutput result;
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
        private static bool PreHeal(HealInput info)
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
        private static void PostHeal(HealOutput result)
        {
            var entity = result.Entity;
            if (entity == null)
                return;
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_HEAL, result);
        }
        private static HealOutput ArmorHeal(HealInput info)
        {
            var armor = info.Armor;
            // Apply Healing.
            float hpBefore = armor.Health;
            var maxHealth = armor.GetMaxHealth();
            if (armor.Health < maxHealth)
            {
                armor.Health = Mathf.Min(armor.Health + info.Amount, maxHealth);
            }

            return new HealOutput()
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
        private static HealOutput BodyHeal(HealInput info)
        {
            var entity = info.Entity;

            // Apply Healing.
            float hpBefore = entity.Health;
            var maxHealth = entity.GetMaxHealth();
            if (entity.Health < maxHealth)
            {
                entity.Health = Mathf.Min(entity.Health + info.Amount, maxHealth);
            }

            return new HealOutput()
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
