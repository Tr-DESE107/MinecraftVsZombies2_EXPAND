﻿using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.parabot)]
    public class ParabotBuff : BuffDefinition
    {
        public ParabotBuff(string nsp, string name) : base(nsp, name)
        {
            detector = new ParabotDetector(RANGE);
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.parabotInsected, VanillaModelID.parabotInsected);
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEntityDeathCallback);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_COOLDOWN, MAX_COOLDOWN);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateCooldown(buff);
            UpdateExplosion(buff);
            UpdateTimeout(buff);
        }
        private void UpdateCooldown(Buff buff)
        {
            var cooldown = buff.GetProperty<int>(PROP_COOLDOWN);
            cooldown--;
            if (cooldown <= 0)
            {
                ShootTick(buff);
                cooldown = MAX_COOLDOWN;
            }
            buff.SetProperty(PROP_COOLDOWN, cooldown);
        }
        private void ShootTick(Buff buff)
        {
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var level = buff.Level;
            Vector3 centerPos = entity.GetCenter();

            var param = new DetectionParams()
            {
                entity = entity,
                faction = GetFaction(buff)
            };
            var target = detector.DetectEntityWithTheLeast(param, e => GetTargetPriority(centerPos, e));
            if (target != null)
            {
                Vector3 otherCenter = target.GetCenter();
                var shootParams = new ShootParams()
                {
                    damage = 20,
                    faction = GetFaction(buff),
                    position = centerPos,
                    projectileID = VanillaProjectileID.parabot,
                    soundID = VanillaSoundID.bow,
                    velocity = (otherCenter - centerPos).normalized * 10
                };
                var projectile = entity.ShootProjectile(shootParams);
                projectile.Timeout = Mathf.CeilToInt(RANGE / projectile.Velocity.magnitude);
            }
        }
        private void UpdateExplosion(Buff buff)
        {
            var explodeTime = GetExplodeTime(buff);
            if (explodeTime <= 0)
                return;
            explodeTime--;
            buff.SetProperty(PROP_EXPLODE_TIME, explodeTime);

            var model = buff.GetInsertedModel(VanillaModelKeys.parabotInsected);
            if (model != null)
            {
                model.SetAnimationBool("Exploding", true);
            }
            if (explodeTime <= 0)
            {
                ParabotExplode(buff);
                buff.Remove();
            }
        }
        private void ParabotExplode(Buff buff)
        {
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var level = entity.Level;
            var range = 50;
            Vector3 centerPos = entity.GetCenter();
            entity.Explode(centerPos, range, GetFaction(buff), 500, new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.DAMAGE_BOTH_ARMOR_AND_BODY, VanillaDamageEffects.MUTE));
            var explosion = level.Spawn(VanillaEffectID.explosion, centerPos, entity);
            explosion.SetSize(Vector3.one * (range * 2));
            entity.PlaySound(VanillaSoundID.explosion);
            level.ShakeScreen(10, 0, 15);
        }
        private void UpdateTimeout(Buff buff)
        {
            if (GetExplodeTime(buff) > 0)
                return;
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            if (timeout <= 0)
            {
                buff.Remove();
                return;
            }
        }
        private int GetFaction(Buff buff)
        {
            return buff.GetProperty<int>(PROP_FACTION);
        }
        private int GetExplodeTime(Buff buff)
        {
            return buff.GetProperty<int>(PROP_EXPLODE_TIME);
        }
        private void PostEntityDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var info = param.deathInfo;
            var buffs = entity.GetBuffs<ParabotBuff>();
            foreach (var buff in buffs)
            {
                if (GetExplodeTime(buff) > 0)
                {
                    ParabotExplode(buff);
                }
            }
            entity.RemoveBuffs(buffs);
        }
        private float GetTargetPriority(Vector3 sourcePos, Entity target)
        {
            var priority = (sourcePos - target.GetCenter()).magnitude;
            if (target.HasBuff<ParabotBuff>())
                priority += 100000000;
            return priority;
        }
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public static readonly VanillaBuffPropertyMeta<int> PROP_COOLDOWN = new VanillaBuffPropertyMeta<int>("Cooldown");
        public static readonly VanillaBuffPropertyMeta<int> PROP_FACTION = new VanillaBuffPropertyMeta<int>("Faction");
        public static readonly VanillaBuffPropertyMeta<int> PROP_EXPLODE_TIME = new VanillaBuffPropertyMeta<int>("ExplodeTime");
        public const int MAX_COOLDOWN = 45;
        public const int MAX_EXPLODE_TIME = 24;
        public const float RANGE = 280;
        private ParabotDetector detector;
    }
}
