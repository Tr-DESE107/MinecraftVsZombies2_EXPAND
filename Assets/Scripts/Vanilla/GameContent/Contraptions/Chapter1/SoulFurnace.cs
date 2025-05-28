﻿using System.Collections.Generic;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.soulFurnace)]
    public class SoulFurnace : DispenserFamily
    {
        public SoulFurnace(string nsp, string name) : base(nsp, name)
        {
            evocationDetector = new SoulFurnaceEvocationDetector();
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            var fuel = 0;
            if (entity.Level.IsIZombie())
            {
                fuel = I_ZOMBIE_FUEL;
            }
            SetFuel(entity, fuel);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            var fuel = GetFuel(entity);
            if (fuel > 0)
            {
                if (!entity.IsEvoked())
                {
                    ShootTick(entity);
                    // 大招退出 → 清除计时器
                    if (evokeTimerDict.ContainsKey(entity))
                    {
                        evokeTimerDict.Remove(entity);
                    }
                }
                else
                {
                    EvokedUpdate(entity);

                    // 正在大招 → 更新计时器
                    if (!evokeTimerDict.ContainsKey(entity))
                        evokeTimerDict[entity] = 0;

                    evokeTimerDict[entity] += Time.deltaTime;

                    // 超时处理
                    if (evokeTimerDict[entity] >= MAX_EVOKE_DURATION)
                    {

                        evokeTimerDict.Remove(entity);
                        Explode(entity, 120, 600);
                        entity.Level.ShakeScreen(10, 0, 15);
                        entity.Remove();
                    }
                }

            }

            UpdateSacrifice(entity);
            var percentage = fuel / (float)MAX_FUEL;
            float light = 0;
            if (fuel > 0)
            {
                light = Mathf.Lerp(0.5f, 1, percentage);
            }
            var bar = GetDisplayFuel(entity);
            bar = bar * 0.5f + (percentage) * 0.5f;
            SetDisplayFuel(entity, bar);

            entity.SetAnimationFloat("Light", light);
            entity.SetAnimationFloat("Bar", bar);

            entity.SetIsFire(fuel > 0);
            entity.SetLightSource(fuel > 0);
            entity.SetLightRange(Vector3.one * (240 * light));
        }
        public override Entity Shoot(Entity entity)
        {
            var projectile = base.Shoot(entity);
            SpendFuel(entity, 1);
            return projectile;
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var fuel = GetFuel(entity);
            fuel = Mathf.Max(REFUEL_THRESOLD, fuel);
            SetFuel(entity, fuel);
            entity.SetEvoked(true);


        }

        public int GetFuel(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_FUEL);
        public void SetFuel(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_FUEL, Mathf.Clamp(value, 0, MAX_FUEL));
        public void AddFuel(Entity entity, int value)
        {
            SetFuel(entity, GetFuel(entity) + value);
        }
        public void SpendFuel(Entity entity, int value)
        {
            var fuelBefore = GetFuel(entity);
            AddFuel(entity, -value);
            var fuelAfter = GetFuel(entity);
            if (fuelAfter <= 0 && fuelBefore > 0)
            {
                entity.PlaySound(VanillaSoundID.fizz);
            }
        }
        public float GetDisplayFuel(Entity entity) => entity.GetBehaviourField<float>(ID, PROP_DISPLAY_FUEL);
        public void SetDisplayFuel(Entity entity, float value) => entity.SetBehaviourField(ID, PROP_DISPLAY_FUEL, value);
        public bool CanSacrifice(Entity entity, Entity soulFurnace)
        {
            bool canSacrifice = entity.Type == EntityTypes.PLANT && !entity.IsDead;
            var result = new CallbackResult(canSacrifice);
            entity.Level.Triggers.RunCallbackWithResultFiltered(VanillaLevelCallbacks.CAN_CONTRAPTION_SACRIFICE, new VanillaLevelCallbacks.ContraptionSacrificeValueParams(entity, soulFurnace), result, entity.GetDefinitionID());
            return result.GetValue<bool>();
        }
        public static int GetSacrificeFuel(Entity entity, Entity soulFurnace)
        {
            int fuel = 5;

            var game = Global.Game;
            int cost = entity.GetCost();
            var rechargeID = entity.GetRechargeID();

            float fuelMultiplier = 1;
            var rechargeDef = game.GetRechargeDefinition(rechargeID);
            if (rechargeDef != null)
            {
                fuelMultiplier = rechargeDef.GetQuality();
            }

            fuel = Mathf.CeilToInt((fuel + cost / 6f) * fuelMultiplier);
            var result = new CallbackResult(fuel);
            entity.Level.Triggers.RunCallbackWithResult(VanillaLevelCallbacks.GET_CONTRAPTION_SACRIFICE_FUEL, new VanillaLevelCallbacks.ContraptionSacrificeValueParams(entity, soulFurnace), result);
            return result.GetValue<int>();
        }
        public void Sacrifice(Entity entity, Entity soulFurnace, int fuel)
        {
            var result = new CallbackResult(true);
            Global.Game.RunCallbackWithResultFiltered(VanillaLevelCallbacks.PRE_CONTRAPTION_SACRIFICE, new VanillaLevelCallbacks.ContraptionSacrificeParams(entity, soulFurnace, fuel), result, entity.GetDefinitionID());
            if (!result.GetValue<bool>())
                return;

            var effects = new DamageEffectList(VanillaDamageEffects.SACRIFICE, VanillaDamageEffects.SELF_DAMAGE);
            entity.Die(effects, soulFurnace);
            AddFuel(soulFurnace, fuel);
            entity.Level.Spawn(VanillaEffectID.soulfireBurn, entity.GetCenter(), soulFurnace);
            entity.PlaySound(VanillaSoundID.refuel);

            Global.Game.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_SACRIFICE, new VanillaLevelCallbacks.ContraptionSacrificeParams(entity, soulFurnace, fuel), entity.GetDefinitionID());
        }
        private void UpdateSacrifice(Entity furnace)
        {
            if (furnace.IsDead)
                return;
            var column = furnace.GetColumn();
            var lane = furnace.GetLane();
            //var targetGrid = furnace.Level.GetGrid(column + 1 * furnace.GetFacingX(), lane);
            var targetGrid = furnace.Level.GetGrid(column, lane);
            if (targetGrid == null)
                return;
            var layers = targetGrid.GetLayers();
            var orderedLayers = VanillaGridLayers.sacrificeLayers;
            foreach (var layer in orderedLayers)
            {
                var ent = targetGrid.GetLayerEntity(layer);
                if (ent == null || ent == furnace || !CanSacrifice(ent, furnace))
                    continue;
                var fuel = GetSacrificeFuel(ent, furnace);
                Sacrifice(ent, furnace, fuel);
                break;
            }
        }
        private void EvokedUpdate(Entity entity)
        {
            detectBuffer.Clear();
            evocationDetector.DetectMultiple(entity, detectBuffer);
            var shootPoint = entity.GetShootPoint();
            Vector3 shootDir;
            if (detectBuffer.Count <= 0)
            {
                shootDir = entity.GetFacingDirection();
            }
            else
            {
                var targetCollider = detectBuffer.Random(entity.RNG);
                var target = targetCollider?.Entity;
                shootDir = (target.Position - shootPoint).normalized;
                shootDir.y = 0;
            }
            var shotspeed = entity.GetShotVelocity().magnitude * 2;
            var velocity = shotspeed * shootDir;
            var projectile = entity.ShootProjectile(entity.GetProjectileID(), velocity);

            SoulfireBall.SetBlast(projectile, true);
            entity.PlaySound(VanillaSoundID.darkSkiesCast);

            SpendFuel(entity, 1);

            if (GetFuel(entity) <= 0)
            {
                entity.SetEvoked(false);
            }
        }

        public static DamageOutput[] Explode(Entity entity, float range, float damage)
        {
            var damageEffects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.EXPLOSION);
            var damageOutputs = entity.Level.Explode(entity.Position, range, VanillaFactions.NEUTRAL, damage, damageEffects, entity);
            foreach (var output in damageOutputs)
            {
                var result = output?.BodyResult;
                if (result != null && result.Fatal)
                {
                    var target = output.Entity;
                    var distance = (target.Position - entity.Position).magnitude;
                    var speed = 25 * Mathf.Lerp(1f, 0.5f, distance / range);
                    target.Velocity = target.Velocity + Vector3.up * speed;
                }
            }
            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));
            entity.PlaySound(VanillaSoundID.explosion);
            entity.Level.ShakeScreen(10, 0, 15);


            return damageOutputs;
        }

        private static readonly NamespaceID ID = VanillaContraptionID.soulFurnace;
        public static readonly VanillaEntityPropertyMeta<int> PROP_FUEL = new VanillaEntityPropertyMeta<int>("Fuel");
        public static readonly VanillaEntityPropertyMeta<float> PROP_DISPLAY_FUEL = new VanillaEntityPropertyMeta<float>("DisplayFuel");
        public const int MAX_FUEL = 60;
        public const int REFUEL_THRESOLD = 10;
        public const int I_ZOMBIE_FUEL = REFUEL_THRESOLD + 5;
        private Detector evocationDetector;
        private List<IEntityCollider> detectBuffer = new List<IEntityCollider>();

        private static readonly float MAX_EVOKE_DURATION = 1.75f;
        private Dictionary<Entity, float> evokeTimerDict = new();

    }
}
