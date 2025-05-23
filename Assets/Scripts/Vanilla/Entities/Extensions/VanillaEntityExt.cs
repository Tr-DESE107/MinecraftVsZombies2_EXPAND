using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Carts;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.Shells;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
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
        public static void DamageBlink(this Entity entity)
        {
            if (entity != null && !entity.HasBuff<DamageColorBuff>())
                entity.AddBuff<DamageColorBuff>();
        }
        public static int GetHealthState(this Entity entity, int stateCount)
        {
            return GetHealthState(entity.Health, entity.GetMaxHealth(), stateCount);
        }
        public static int GetHealthState(float health, float maxHealth, int stateCount)
        {
            float stateHP = maxHealth / stateCount;
            return Mathf.CeilToInt(health / stateHP) - 1;
        }
        public static DamageOutput TakeDamageNoSource(this Entity entity, float amount, DamageEffectList effects, NamespaceID armorSlot = null)
        {
            return entity.TakeDamage(amount, effects, new EntityReferenceChain(null), armorSlot);
        }
        public static DamageOutput TakeDamage(this Entity entity, float amount, DamageEffectList effects, Entity source, NamespaceID armorSlot = null)
        {
            return entity.TakeDamage(amount, effects, new EntityReferenceChain(source), armorSlot);
        }
        public static DamageOutput TakeDamage(this Entity entity, float amount, DamageEffectList effects, EntityReferenceChain source, NamespaceID armorSlot = null)
        {
            return TakeDamage(new DamageInput(amount, effects, entity, source, armorSlot));
        }
        public static DamageOutput TakeDamage(DamageInput input)
        {
            var result = new DamageOutput()
            {
                Entity = input.Entity
            };
            if (input == null)
                return result;
            if (input.Entity.IsInvincible() || input.Entity.IsDead)
                return result;
            if (!PreTakeDamage(input))
                return result;
            if (input.Amount <= 0)
                return result;
            if (!NamespaceID.IsValid(input.ShieldTarget))
            {
                if (Armor.Exists(input.Entity.GetMainArmor()) && !input.Effects.HasEffect(VanillaDamageEffects.IGNORE_ARMOR))
                {
                    ArmoredTakeDamage(input, result);
                }
                else
                {
                    result.BodyResult = BodyTakeDamage(input);
                }
            }
            else
            {
                var armor = input.Entity.GetArmorAtSlot(input.ShieldTarget);
                if (Armor.Exists(armor))
                {
                    result.ShieldResult = armor.TakeDamage(input);
                    result.ShieldTarget = input.ShieldTarget;
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
            var result = new CallbackResult(true);
            entity.Definition.PreTakeDamage(damageInfo, result);
            if (!result.IsBreakRequested)
            {
                var param = new VanillaLevelCallbacks.PreTakeDamageParams()
                {
                    input = damageInfo
                };
                damageInfo.Entity.Level.Triggers.RunCallbackWithResultFiltered(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, param, result, entity.Type);
            }
            return result.GetValue<bool>();
        }
        private static void PostTakeDamage(DamageOutput output)
        {
            Entity entity = output.Entity;
            if (entity == null)
                return;
            entity.Definition.PostTakeDamage(output);
            var param = new VanillaLevelCallbacks.PostTakeDamageParams()
            {
                output = output
            };
            entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, param, entity.Type);
        }
        private static void ArmoredTakeDamage(DamageInput info, DamageOutput result)
        {
            var entity = info.Entity;
            var armor = entity.GetMainArmor();
            var armorResult = armor.TakeDamage(info);
            result.ArmorResult = armorResult;
            if (info.Effects.HasEffect(VanillaDamageEffects.DAMAGE_BOTH_ARMOR_AND_BODY))
            {
                result.BodyResult = BodyTakeDamage(info);
            }
            else if (info.Effects.HasEffect(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN) && !Armor.Exists(entity.GetMainArmor()))
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
            var shineRingID = entity.GetProperty<EntityID>(PROP_SHINE_RING);
            var shineRing = shineRingID?.GetEntity(entity.Level);
            if (shineRing != null && shineRing.Exists())
                return;
            shineRing = entity.Level.FindFirstEntity(e => e.IsEntityOf(VanillaEffectID.shineRing) && e.Parent == entity);
            if (shineRing != null && shineRing.Exists())
                return;
            shineRing = entity.Level.Spawn(VanillaEffectID.shineRing, entity.Position, entity);
            shineRing.SetParent(entity);
            entity.SetProperty(PROP_SHINE_RING, new EntityID(shineRing));
        }
        #endregion

        #region 音效

        public static void PlayCrySound(this Entity entity, NamespaceID soundID, float pitchMultiplier = 1, float volume = 1)
        {
            var pitch = entity.GetCryPitch() * pitchMultiplier;
            entity.PlaySound(soundID, pitch, volume);
        }
        public static void PlaySound(this Entity entity, NamespaceID soundID, float pitch = 1, float volume = 1)
        {
            entity.Level.PlaySound(soundID, entity.Position, pitch, volume);
        }
        public static void PlayHitSound(this DamageOutput damage)
        {
            if (damage == null)
                return;
            var entity = damage.Entity;

            var shieldResult = damage.ShieldResult;
            PlayArmorHitSound(entity, shieldResult);

            var armorResult = damage.ArmorResult;
            PlayArmorHitSound(entity, armorResult);

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
        public static void PlayArmorHitSound(this Entity entity, ArmorDamageResult result)
        {
            if (result != null && !result.Effects.HasEffect(VanillaDamageEffects.MUTE))
            {
                var armor = result.Armor;
                var shell = result.ShellDefinition;
                NamespaceID hitSound = null;
                if (shell != null && shell.GetSpecialShellHitSound(result, true) is NamespaceID specialSound && NamespaceID.IsValid(specialSound))
                {
                    hitSound = specialSound;
                }
                else if (armor.GetHitSound() is NamespaceID specificSound && NamespaceID.IsValid(specificSound))
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
            else if (damageEffects.HasEffect(VanillaDamageEffects.LIGHTNING))
            {
                return VanillaSoundID.zap;
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
        public static bool IsChangingLane(this Entity entity)
        {
            return entity.HasBuff<ChangeLaneBuff>();
        }
        public static void StartChangingLane(this Entity entity, int target)
        {
            var level = entity.Level;
            target = Math.Clamp(target, 0, level.GetMaxLaneCount() - 1);
            var source = entity.GetLane();
            var buff = entity.GetFirstBuff<ChangeLaneBuff>();
            if (buff == null)
            {
                buff = entity.AddBuff<ChangeLaneBuff>();
            }
            ChangeLaneBuff.Start(buff, target, source);
        }
        public static void StopChangingLane(this Entity entity)
        {
            var buff = entity.GetFirstBuff<ChangeLaneBuff>();
            if (buff == null)
            {
                return;
            }
            ChangeLaneBuff.Stop(buff);
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
            var buff = entity.GetFirstBuff<StunBuff>();
            if (buff == null)
            {
                buff = entity.AddBuff<StunBuff>();
            }
            StunBuff.SetStunTime(buff, timeout);
        }

        #region 阻挡火焰
        public static bool WillDamageBlockFire(this DamageOutput damage)
        {
            if (damage == null)
                return false;
            foreach (var result in damage.GetAllResults())
            {
                if (result == null)
                    continue;
                var shell = result.ShellDefinition;
                if (shell == null)
                    continue;
                if (shell.BlocksFire())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region 网格
        public static void UpdateTakenGrids(this Entity entity)
        {
            entityGridBuffer.Clear();
            entity.GetTakenGrids(entityGridBuffer);
            foreach (var grid in entityGridBuffer)
            {
                entityGridLayerBuffer.Clear();
                entity.GetTakingGridLayersNonAlloc(grid, entityGridLayerBuffer);
                foreach (var layer in entityGridLayerBuffer)
                {
                    if (CanTakeGrid(entity, grid, layer))
                        continue;
                    entity.ReleaseGrid(grid, layer);
                }
            }
            if (entity.ExistsAndAlive())
            {
                var gridBelow = entity.GetGrid();
                foreach (var layer in entity.GetGridLayersToTake())
                {
                    if (!CanTakeGrid(entity, gridBelow, layer))
                        continue;
                    entity.TakeGrid(gridBelow, layer);
                }
            }
        }
        public static bool CanTakeGrid(this Entity entity, LawnGrid grid, NamespaceID layer)
        {
            if (grid == null)
                return false;
            if (!entity.ExistsAndAlive())
                return false;
            if (entity.GetRelativeY() > leaveGridHeight)
                return false;
            if (entity.GetGrid() != grid)
                return false;
            var layersToTake = entity.GetGridLayersToTake();
            if (!layersToTake.Contains(layer))
                return false;
            return true;
        }
        private const float leaveGridHeight = 64;
        private static List<LawnGrid> entityGridBuffer = new List<LawnGrid>();
        private static List<NamespaceID> entityGridLayerBuffer = new List<NamespaceID>();
        #endregion


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
            var result = new CallbackResult(true);
            var param = new VanillaLevelCallbacks.PreHealParams()
            {
                input = info
            };
            entity.Level.Triggers.RunCallbackWithResult(VanillaLevelCallbacks.PRE_ENTITY_HEAL, param, result);
            return result.GetValue<bool>();
        }
        private static void PostHeal(HealOutput output)
        {
            var entity = output.Entity;
            if (entity == null)
                return;
            var param = new VanillaLevelCallbacks.PostHealParams()
            {
                output = output
            };
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_HEAL, param);
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

        #region 阵营
        public static void Charm(this Entity entity, int faction)
        {
            var buff = entity.GetFirstBuff<CharmBuff>();
            if (buff == null)
            {
                buff = entity.AddBuff<CharmBuff>();
            }
            CharmBuff.SetPermanent(buff, faction);
            buff.Update();
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_CHARM, new VanillaLevelCallbacks.PostEntityCharmParams(entity, buff));
        }
        public static void CharmWithSource(this Entity entity, Entity source)
        {
            var buff = entity.GetFirstBuff<CharmBuff>();
            if (buff == null)
            {
                buff = entity.AddBuff<CharmBuff>();
            }
            CharmBuff.SetSource(buff, source);
            buff.Update();
            entity.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENTITY_CHARM, new VanillaLevelCallbacks.PostEntityCharmParams(entity, buff));
        }
        public static void RemoveCharm(this Entity entity)
        {
            entity.RemoveBuffs<CharmBuff>();
        }
        public static bool IsCharmed(this Entity entity)
        {
            return entity.HasBuff<CharmBuff>();
        }
        #endregion

        #region 护甲
        public static Armor GetMainArmor(this Entity entity)
        {
            return entity.GetArmorAtSlot(VanillaArmorSlots.main);
        }
        public static void EquipMainArmor(this Entity entity, NamespaceID id)
        {
            entity.EquipArmorTo(VanillaArmorSlots.main, id);
        }
        #endregion

        #region 寻路
        public static bool MoveOrthogonally(this Entity entity, int targetGridIndex, float speed)
        {
            var level = entity.Level;
            var targetGridPosition = level.GetEntityGridPositionByIndex(targetGridIndex);
            var targetGridDistance = targetGridPosition - entity.Position;
            if (targetGridDistance.magnitude <= speed)
            {
                // 更新目标。
                entity.Position = targetGridPosition;
                return true;
            }
            else
            {
                entity.Position += targetGridDistance.normalized * speed;
            }
            return false;
        }
        public static LawnGrid GetChaseTargetGrid(this Entity entity, Entity target, Func<Entity, Vector2Int, bool> gridValidator)
        {
            var level = entity.Level;
            var lane = entity.GetLane();
            var column = entity.GetColumn();
            var x = entity.Position.x;

            Vector2Int currentGrid = new Vector2Int(column, lane);
            Vector2Int newTargetGridOffset = Vector2Int.zero;
            var possibleDirections = adjacentGridOffsets.Where(o => gridValidator(entity, currentGrid + o));
            if (possibleDirections.Count() <= 0)
                return entity.GetGrid();

            if (target.ExistsAndAlive() && possibleDirections.Count() > 0)
            {
                var targetLane = target.GetLane();
                float currentDistanceY = currentGrid.y - targetLane;
                float currentDistanceX = entity.Position.x - target.Position.x;
                var orderedDirections = possibleDirections
                    .OrderBy(o => Mathf.Abs(currentDistanceY + o.y) - Mathf.Abs(currentDistanceY))
                    .ThenBy(o => Mathf.Abs(currentDistanceX + o.x * level.GetGridWidth()) - Mathf.Abs(currentDistanceX));
                newTargetGridOffset = orderedDirections.FirstOrDefault();
            }
            else
            {
                var rng = entity.RNG;
                newTargetGridOffset = possibleDirections.Random(rng);
            }
            return level.GetGrid(currentGrid + newTargetGridOffset) ?? entity.GetGrid();
        }
        public static LawnGrid GetChaseTargetGrid(this Entity entity, Entity target)
        {
            return GetChaseTargetGrid(entity, target, (e, p) => e.Level.ValidateGridOutOfBounds(p));
        }
        public static LawnGrid GetEvadeTargetGrid(this Entity entity, Entity target, Func<Entity, Vector2Int, bool> gridValidator)
        {
            var level = entity.Level;
            var lane = entity.GetLane();
            var column = entity.GetColumn();
            var x = entity.Position.x;

            Vector2Int currentGrid = new Vector2Int(column, lane);
            Vector2Int newTargetGridOffset = Vector2Int.zero;
            var possibleDirections = adjacentGridOffsets.Where(o => gridValidator(entity, currentGrid + o));
            if (possibleDirections.Count() <= 0)
                return entity.GetGrid();

            if (target.ExistsAndAlive() && possibleDirections.Count() > 0)
            {
                var targetLane = target.GetLane();
                float currentDistanceY = currentGrid.y - targetLane;
                float currentDistanceX = entity.Position.x - target.Position.x;
                var orderedDirections = possibleDirections
                    .OrderByDescending(o => Mathf.Abs(currentDistanceY + o.y) - Mathf.Abs(currentDistanceY))
                    .ThenByDescending(o => Mathf.Abs(currentDistanceX + o.x * level.GetGridWidth()) - Mathf.Abs(currentDistanceX));
                newTargetGridOffset = orderedDirections.FirstOrDefault();
            }
            else
            {
                var rng = entity.RNG;
                newTargetGridOffset = possibleDirections.Random(rng);
            }
            return level.GetGrid(currentGrid + newTargetGridOffset) ?? entity.GetGrid();
        }
        public static LawnGrid GetEvadeTargetGrid(this Entity entity, Entity target)
        {
            return GetEvadeTargetGrid(entity, target, (e, p) => e.Level.ValidateGridOutOfBounds(p));
        }
        public static readonly Vector2Int[] adjacentGridOffsets = new Vector2Int[]
        {
            Vector2Int.down,
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.left
        };
        #endregion
        public static SpawnParams GetSpawnParams(this Entity entity)
        {
            var param = new SpawnParams();
            param.SetProperty(EngineEntityProps.FACTION, entity.GetFaction());
            return param;
        }
        public static Entity SpawnWithParams(this Entity entity, NamespaceID id, Vector3 pos)
        {
            var param = entity.GetSpawnParams();
            return entity.Spawn(id, pos, param);
        }

        private const string PROP_REGION = "entities";
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta PROP_SHINE_RING = new VanillaEntityPropertyMeta("LightShineRing");
    }
}
