using System.Linq;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Shells;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Triggers;
using Tools;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEntityExt
    {
        #region 面朝方向
        public static int GetFacingX(this Entity entity)
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
            return GetHealthState(entity.Health, entity.GetMaxHealth(), stateCount);
        }
        public static int GetHealthState(float health, float maxHealth, int stateCount)
        {
            float stateHP = maxHealth / stateCount;
            return Mathf.CeilToInt(health / stateHP) - 1;
        }
        public static DamageOutput TakeDamageNoSource(this Entity entity, float amount, DamageEffectList effects, bool toBody = true, bool toShield = false)
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
            if (damageInfo.IsInterrupted)
                return false;
            damageInfo.Entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, damageInfo, c => c(damageInfo));
            return !damageInfo.IsInterrupted;
        }
        private static void PostTakeDamage(DamageOutput result)
        {
            Entity entity = result.Entity;
            if (entity == null)
                return;
            entity.Definition.PostTakeDamage(result);
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, c => c(result));
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

            var result = new BodyDamageResult()
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

            if (entity.Health <= 0)
            {
                entity.Die(info.Effects, info.Source, result);
            }

            return result;
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

        public static void PlaySound(this Entity entity, NamespaceID soundID, float pitch = 1, float volume = 1)
        {
            entity.Level.PlaySound(soundID, entity.Position, pitch, volume);
        }
        public static void PlayHitSound(this DamageOutput damage)
        {
            if (damage == null)
                return;
            var entity = damage.Entity;

            var armorResult = damage.ArmorResult;
            if (armorResult != null && !armorResult.Effects.HasEffect(VanillaDamageEffects.MUTE))
            {
                var shell = armorResult.ShellDefinition;
                NamespaceID hitSound = null;
                if (shell != null && shell.GetSpecialShellHitSound(armorResult, true) is NamespaceID specialSound && NamespaceID.IsValid(specialSound))
                {
                    hitSound = specialSound;
                }
                else if (shell != null && shell.GetHitSound() is NamespaceID shellSound && NamespaceID.IsValid(shellSound))
                {
                    hitSound = shellSound;
                }
                if (NamespaceID.IsValid(hitSound))
                {
                    entity.PlaySound(hitSound);
                }
            }
            var bodyResult = damage.BodyResult;
            if (bodyResult != null && !bodyResult.Effects.HasEffect(VanillaDamageEffects.MUTE))
            {
                var shell = bodyResult.ShellDefinition;
                NamespaceID hitSound = null;
                if (shell.GetSpecialShellHitSound(bodyResult, false) is NamespaceID specialSound && NamespaceID.IsValid(specialSound))
                {
                    hitSound = specialSound;
                }
                else if (entity.GetHitSound() is NamespaceID specificSound && NamespaceID.IsValid(specificSound))
                {
                    hitSound = specificSound;
                }
                else if (shell != null && shell.GetHitSound() is NamespaceID shellSound && NamespaceID.IsValid(shellSound))
                {
                    hitSound = shellSound;
                }
                if (NamespaceID.IsValid(hitSound))
                {
                    entity.PlaySound(hitSound);
                }
            }
        }
        public static NamespaceID GetSpecialShellHitSound(this ShellDefinition shell, DamageResult result, bool isArmor)
        {
            if (shell == null)
                return null;
            var damageEffects = result.Effects;
            if (!isArmor)
            {
                if (damageEffects.HasEffect(VanillaDamageEffects.WHACK))
                {
                    return VanillaSoundID.bonk;
                }
                else if (damageEffects.HasEffect(VanillaDamageEffects.FALL_DAMAGE))
                {
                    return result.Amount > 100 ? VanillaSoundID.fallBig : VanillaSoundID.fallSmall;
                }
            }

            if (damageEffects.HasEffect(VanillaDamageEffects.FIRE) && !shell.BlocksFire())
            {
                return VanillaSoundID.fire;
            }
            else if (damageEffects.HasEffect(VanillaDamageEffects.SLICE) && shell.IsSliceCritical())
            {
                return VanillaSoundID.slice;
            }
            else if (damageEffects.HasEffect(VanillaDamageEffects.TINY))
            {
                return VanillaSoundID.smallHit;
            }
            return null;
        }
        public static void EmitBlood(this Entity entity)
        {
            var blood = entity.Level.Spawn(VanillaEffectID.bloodParticles, entity.GetCenter(), entity);
            var bloodColor = entity.GetBloodColor();
            blood.SetTint(bloodColor);
        }
        #endregion

        #region 换行
        public static void RandomChangeAdjacentLane(this Entity entity, RandomGenerator rng)
        {
            var lane = entity.GetLane();
            int laneDir;
            if (lane <= 0)
            {
                laneDir = 1;
            }
            else if (lane >= entity.Level.GetMaxLaneCount() - 1)
            {
                laneDir = -1;
            }
            else
            {
                laneDir = rng.Next(2) * 2 - 1;
            }
            var targetLane = lane + laneDir;
            entity.StartChangingLane(targetLane);
        }
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
            return entity.Type == EntityTypes.ENEMY && !entity.IsDead && !entity.IsNotActiveEnemy() && entity.IsHostile(entity.Level.Option.LeftFaction);
        }
        public static bool CanEntityEnterHouse(this Entity entity)
        {
            return entity.Type == EntityTypes.ENEMY && !entity.IsDead && !entity.IsHarmless() && entity.IsHostile(entity.Level.Option.LeftFaction);
        }
        public static bool IsFriendlyEntity(this Entity entity)
        {
            return entity.Level.IsFriendlyFaction(entity.GetFaction());
        }
        public static bool IsHostileEntity(this Entity entity)
        {
            return entity.Level.IsHostileFaction(entity.GetFaction());
        }
        public static bool IsFriendlyFaction(this LevelEngine level, int faction)
        {
            return EngineEntityExt.IsFriendly(faction, level.Option.LeftFaction);
        }
        public static bool IsHostileFaction(this LevelEngine level, int faction)
        {
            return !IsFriendlyFaction(level, faction);
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

        private const float leaveGridHeight = 64;
        public static void UpdateTakenGrids(this Entity entity)
        {
            if (entity.GetRelativeY() > leaveGridHeight || entity.Removed)
            {
                entity.ClearTakenGrids();
            }
            else
            {
                var grid = entity.Level.GetGrid(entity.GetColumn(), entity.GetLane());
                foreach (var layer in entity.GetGridLayersToTake())
                {
                    entity.TakeGrid(grid, layer);
                }
            }
        }

        #region 治疗
        public static HealOutput HealEffects(this Entity entity, float amount, Entity source)
        {
            var result = entity.Heal(amount, source);
            if (result == null)
                return null;
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
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.PRE_ENTITY_HEAL, info, c => c(info));
            return !info.IsInterrupted;
        }
        private static void PostHeal(HealOutput result)
        {
            var entity = result.Entity;
            if (entity == null)
                return;
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_HEAL, c => c(result));
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

        #region 沉没
        public const int SPLASH_SIZE_UNIT = 110592; //48^3
        public static bool IsOnWater(this Entity entity)
        {
            var grid = entity.GetGrid();
            return grid != null && grid.IsWater();
        }
        public static bool IsInWater(this Entity entity)
        {
            return entity.IsOnWater() && entity.IsOnGround;
        }
        public static void PlaySplashEffect(this Entity entity)
        {
            var size = entity.GetScaledSize();
            var scale = Mathf.Clamp(size.x * size.y * size.z / SPLASH_SIZE_UNIT, 1, 5);
            entity.PlaySplashEffect(scale * Vector3.one);
        }
        public static void PlaySplashEffect(this Entity entity, Vector3 scale)
        {
            var level = entity.Level;
            var pos = entity.Position;
            pos.y = entity.GetGroundY();
            var splash = level.Spawn(VanillaEffectID.splashParticles, pos, entity);
            splash.SetTint(level.GetWaterColor());
            splash.SetDisplayScale(scale);
        }

        public static void PlaySplashSound(this Entity entity)
        {
            var level = entity.Level;
            var size = entity.GetScaledSize();
            var sound = VanillaSoundID.splash;
            if (entity.Type == EntityTypes.ENEMY)
            {
                sound = VanillaSoundID.water;
            }
            if (size.x * size.y * size.z / SPLASH_SIZE_UNIT > 1)
            {
                sound = VanillaSoundID.splashBig;
            }
            entity.PlaySound(sound);
        }
        #endregion

        #region 堆叠
        public static bool CanStackFrom(this Entity target, NamespaceID entityID)
        {
            var level = target.Level;
            var definition = level.Content.GetEntityDefinition(entityID);
            var result = new TriggerResultBoolean();
            foreach (var behaviour in definition.GetBehaviours<IStackEntity>())
            {
                behaviour.CanStackOnEntity(target, result);
            }
            return result.Result;
        }
        public static void StackFromEntity(this Entity target, NamespaceID entityID)
        {
            var level = target.Level;
            var definition = level.Content.GetEntityDefinition(entityID);
            foreach (var behaviour in definition.GetBehaviours<IStackEntity>())
            {
                behaviour.StackOnEntity(target);
            }
        }
        #endregion

        #region 阵营
        public static void Charm(this Entity entity, int faction)
        {
            var buff = entity.GetFirstBuff<CharmBuff>();
            if (buff == null)
            {
                buff = entity.AddBuff<CharmBuff>();
            }
            CharmBuff.SetPermanent(buff, faction);
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_CHARM, c => c(entity, buff));
        }
        public static void CharmWithSource(this Entity entity, Entity source)
        {
            var buff = entity.GetFirstBuff<CharmBuff>();
            if (buff == null)
            {
                buff = entity.AddBuff<CharmBuff>();
            }
            CharmBuff.SetSource(buff, source);
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_CHARM, c => c(entity, buff));
        }
        public static bool IsCharmed(this Entity entity)
        {
            return entity.HasBuff<CharmBuff>();
        }
        public static void SetFactionAndDirection(this Entity entity, int faction)
        {
            entity.SetFaction(faction);
            var faceRight = faction == entity.Level.Option.LeftFaction;
            var xScale = entity.FaceLeftAtDefault() == faceRight ? -1 : 1;
            entity.SetScale(new Vector3(xScale, 1, 1));
            entity.SetDisplayScale(new Vector3(xScale, 1, 1));
        }
        #endregion

    }
}
