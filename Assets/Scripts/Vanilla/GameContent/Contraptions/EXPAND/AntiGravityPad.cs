using System.Collections.Generic;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Artifacts;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using MVZ2.Vanilla.Audios;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.AntiGravityPad)]
    public class AntiGravityPad : ContraptionBehaviour
    {
        public AntiGravityPad(string nsp, string name) : base(nsp, name)
        {
            AddAura(new GravityAura());
            projectileDetector = new AntiGravityPadDetector(false, AFFECT_HEIGHT);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            var level = entity.Level;
            var minY = entity.Position.y + MIN_HEIGHT;
            detectBuffer.Clear();
            projectileDetector.DetectEntities(entity, detectBuffer);
            foreach (var projectile in detectBuffer)
            {
                Vector3 pos = projectile.Position;
                pos.y = Mathf.Max(pos.y + PULL_DOWN_SPEED, minY);
                projectile.Position = pos;
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelProperty("IsOn", !entity.IsAIFrozen());
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var pos = entity.Position + Vector3.up * 600;
            var level = entity.Level;
            if (level.AreaID == VanillaAreaID.castle && !Global.Game.IsUnlocked(VanillaUnlockID.brokenLantern))
            {
                if (!level.EntityExists(e => e.IsEntityOf(VanillaPickupID.artifactPickup) && ArtifactPickup.GetArtifactID(e) == VanillaArtifactID.brokenLantern))
                {
                    var lantern = level.Spawn(VanillaPickupID.artifactPickup, pos + Vector3.up * 100, entity);
                    ArtifactPickup.SetArtifactID(lantern, VanillaArtifactID.brokenLantern);
                }
            }
            var anvil = entity.SpawnWithParams(VanillaContraptionID.anvil, pos);



            foreach (var e in level.FindEntities(e => e.ExistsAndAlive() && e.GetFaction() != entity.GetFaction() && e.Type != EntityTypes.BOSS))
            {
                if (e.HasBuff<FlyBuff>())
                {
                    e.RemoveBuffs<FlyBuff>();
                }
            }
            foreach (var e in level.FindEntities(e => e.ExistsAndAlive() && e.GetFaction() != entity.GetFaction() && e.Type != EntityTypes.BOSS))
            {
                if (!e.HasBuff<AntiGravityPadGravityBuff>())
                {
                    e.AddBuff<AntiGravityPadGravityBuff>();
                }
            }
            Quake(entity);

            //我想要这段代码在30帧后执行
            foreach (var e in level.FindEntities(e => e.ExistsAndAlive() && e.GetFaction() != entity.GetFaction() && e.Type != EntityTypes.BOSS))
            {
                if (e.HasBuff<AntiGravityPadGravityBuff>())
                {
                    e.RemoveBuffs<AntiGravityPadGravityBuff>();
                }
            }

        }

        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            var level = entity.Level;
            foreach (var e in level.FindEntities(e => e.ExistsAndAlive() && e.GetFaction() != entity.GetFaction() && e.Type != EntityTypes.BOSS))
            {
                if (e.HasBuff<FlyBuff>())
                {
                    e.RemoveBuffs<FlyBuff>();
                }
            }
            foreach (var e in level.FindEntities(e => e.ExistsAndAlive() && e.GetFaction() != entity.GetFaction() && e.Type != EntityTypes.BOSS))
            {
                if (e.HasBuff<AntiGravityPadGravityBuff>())
                {
                    e.RemoveBuffs<AntiGravityPadGravityBuff>();
                }
            }
        }

        protected override void OnTrigger(Entity entity)
        {
            base.OnTrigger(entity);
            // 执行地震效果,震飞所有地面上的敌人  
            Quake(entity);
            // 设置为破损状态  
            //SetBroken(entity, true);
            // 重置修复计时器,开始60秒的修复倒计时  
            //var restoreTimer = GetRestoreTimer(entity);
            //restoreTimer.ResetTime(RESTORE_TIME);
        }

        private void Quake(Entity self)
        {
            // 清空检测缓冲区  
            detectBuffer.Clear();
            // 查找所有有效目标  
            self.Level.FindEntitiesNonAlloc(e => IsValidTarget(self, e), detectBuffer);

            // 遍历所有目标敌人  
            foreach (var target in detectBuffer)
            {
                // 只影响敌人类型的实体  
                if (target.Type != EntityTypes.ENEMY)
                    continue;

                // 获取击退倍率(某些敌人可能有击退抗性)  
                var knockbackMultiplier = target.GetStrongKnockbackMultiplier();

                // 设置敌人的速度  
                var vel = target.Velocity;
                vel.x = 4 * knockbackMultiplier;  // 水平速度:向后退4单位  
                vel.y = 15 * knockbackMultiplier; // 垂直速度:向上飞15单位  
                target.Velocity = vel;

                // 对中等质量及以下的敌人,随机切换到相邻行  
                // 这会让敌人在被震飞的同时换到上一行或下一行  
                if (target.GetMass() <= VanillaMass.MEDIUM)
                {
                    target.RandomChangeAdjacentLane(self.RNG);
                }

                // 处理骑乘关系:如果敌人有骑手  
                var passenger = target.GetRideablePassenger();
                if (passenger != null)
                {
                    // 眩晕骑手90帧(3秒)  
                    passenger.Stun(90);
                    // 强制骑手下马  
                    target.GetOffHorse();
                }

                var armorSlots = target.GetActiveArmorSlots();
                foreach (var slot in armorSlots)
                {
                    var armor = target.GetArmorAtSlot(slot);
                    if (armor != null)
                    {
                        armor.Health = 0.1f;
                    }
                }
            }


            self.PlaySound(VanillaSoundID.lightningAttack);
        }

        private bool IsValidTarget(Entity self, Entity target)
        {
            // 必须是敌对关系  
            if (!self.IsHostile(target))
                return false;
            // 必须存活  
            if (target.IsDead)
                return false;


            // 必须在陆地上(不能在水中)  
            if (!target.IsAboveLand())
                return false;
            if (!target.HasBuff<AntiGravityPadGravityBuff>())
                return false;
            return true;
        }

        public const float AFFECT_HEIGHT = 96;
        public const float MIN_HEIGHT = 5;
        public const float PULL_DOWN_SPEED = 6.666f;
        private Detector projectileDetector;
        private List<Entity> detectBuffer = new List<Entity>();

        public class GravityAura : AuraEffectDefinition
        {
            public GravityAura()
            {
                BuffID = VanillaBuffID.AntiGravityPadGravity;
                UpdateInterval = 7;
                enemyDetector = new AntiGravityPadDetector(true, AFFECT_HEIGHT);
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var source = auraEffect.Source;
                var entity = source.GetEntity();
                if (entity == null)
                    return;
                if (entity.IsAIFrozen())
                    return;
                detectBuffer.Clear();
                enemyDetector.DetectEntities(entity, detectBuffer);

                var armorSlots = entity.GetActiveArmorSlots();
                foreach (var slot in armorSlots)
                {
                    var armor = entity.GetArmorAtSlot(slot);
                    if (armor != null)
                    {
                        armor.Health = 0.1f;
                    }
                }

                results.AddRange(detectBuffer);
            }
            private Detector enemyDetector;
            private List<Entity> detectBuffer = new List<Entity>();
        }
    }
}
