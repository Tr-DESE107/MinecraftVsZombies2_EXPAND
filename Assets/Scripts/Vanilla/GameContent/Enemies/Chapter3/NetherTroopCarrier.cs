#nullable enable // 自动生成

using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Entities;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Collisions;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [AutoEntityBehaviourDefinition(VanillaEnemyNames.NetherTroopCarrier)]
    public class NetherTroopCarrier : AIEntityBehaviour, IDestroyBySpikesEntityBehaviour, IDeathEffectsBehaviour
    {
        public NetherTroopCarrier(string nsp, string name) : base(nsp, name)
        {
        }

        #region 回调
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetPunctureTimer(entity, new FrameTimer(PUNCTURE_TIME));
            if (!entity.IsPreviewEnemy())
            {
                entity.PlaySound(VanillaSoundID.trainWhistle);
                entity.Level.AddLoopSoundEntity(VanillaSoundID.trainTravel, entity.ID);
            }
            SetLastTriggerHealth(entity, entity.Health);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            bool broken = entity.Health <= BROKEN_THRESOLD;
            if (!entity.IsDead && broken)
            {
                entity.Health -= 2;
                if (entity.Health <= 0)
                {
                    entity.Die();
                }
            }
            // 设置血量状态。
            bool punctured = IsPunctured(entity);
            var hp = entity.Health;
            if (punctured)
            {
                var timer = GetPunctureTimer(entity);
                if (timer != null)
                {
                    // 如果被刺穿，进入死亡倒计时
                    if (timer.RunToExpired())
                    {
                        entity.Die();
                    }
                    else
                    {
                        // 根据剩余时间计算血量百分比，用于显示伤害效果
                        hp *= timer.Frame / (float)timer.MaxFrame;
                    }
                }
                // 播放音效
                entity.PlaySound(VanillaSoundID.shieldHit);
            }
            entity.SetModelDamagePercent(hp, entity.GetMaxHealth());
            entity.SetAnimationBool("Shaking", broken || punctured);

            // 检查是否触发血量损失事件
            CheckHealthLossTrigger(entity);
        }

        /// <summary>
        /// 每失去一定血量达到阈值时，触发一次事件
        /// </summary>
        private void CheckHealthLossTrigger(Entity entity)
        {
            float lastHP = GetLastTriggerHealth(entity);
            float currHP = entity.Health;

            // 每失去 1000 点血量，触发一次事件
            int triggerCount = (int)((lastHP - currHP) / 1000f);
            // 取最小值，最多触发3次
            triggerCount = Mathf.Min(triggerCount, 3);
            if (triggerCount > 0)
            {
                for (int i = 0; i < triggerCount; i++)
                {
                    var randomID = GetRandomSkeletonID(entity.RNG);
                    var spawnParam = entity.GetSpawnParams();
                    spawnParam.SetProperty(EngineEntityProps.FACTION, entity.GetFaction());
                    entity.Spawn(randomID, entity.Position, spawnParam);
                }

                // 重新记录当前血量
                SetLastTriggerHealth(entity, currHP);
            }
        }

        public NamespaceID GetRandomSkeletonID(RandomGenerator rng)
        {
            var index = rng.WeightedRandom(RandomSkeletonWeights);
            return RandomSkeleton[index];
        }

        // 骷髅列表
        private static NamespaceID[] RandomSkeleton = new NamespaceID[]
        {
            VanillaEnemyID.RaiderSkull,
            VanillaEnemyID.NetherArcher,
            VanillaEnemyID.berserker,
            VanillaEnemyID.mesmerizer,
            VanillaEnemyID.NetherWarrior,
            VanillaEnemyID.dullahan,
        };

        // 骷髅权重列表
        private static int[] RandomSkeletonWeights = new int[]
        {
            4,
            8,
            8,
            6,
            10,
            4,
        };

        // 存储上次触发时的血量值字段
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_TRIGGER_HEALTH = new VanillaEntityPropertyMeta<float>("LastTriggerHealth");

        private static float GetLastTriggerHealth(Entity entity) =>
            entity.GetBehaviourField<float>(ID, PROP_LAST_TRIGGER_HEALTH);

        private static void SetLastTriggerHealth(Entity entity, float hp) =>
            entity.SetBehaviourField(ID, PROP_LAST_TRIGGER_HEALTH, hp);


        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;
            if (!collision.Collider.IsForMain())
                return;
            var other = collision.Other;
            if (!other.IsVulnerableEntity())
                return;
            var chariot = collision.Entity;
            if (IsPunctured(chariot) || !chariot.IsHostile(other))
                return;

            Crush(chariot, collision.OtherCollider);
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (!entity.WillRemoveOnDeath(info))
            {
                Explosion.Spawn(entity, entity.GetCenter(), entity.GetScaledSize());
                Explode(entity, 120, 1200);
            }
            entity.Remove();
        }
        public static DamageOutput[] Explode(Entity entity, float range, float damage)
        {
            var damageEffects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.EXPLOSION);
            var damageOutputs = entity.Level.Explode(entity.Position, range, entity.GetFaction(), damage, damageEffects, entity);
            foreach (var output in damageOutputs)
            {
                var result = output.BodyResult;
                if (result != null && result.Fatal)
                {
                    var target = output.Entity;
                    var distance = (target.Position - entity.Position).magnitude;
                    var speed = 25 * Mathf.Lerp(1f, 0.5f, distance / range);
                    target.Velocity = target.Velocity + Vector3.up * speed;
                }
            }
            Explosion.Spawn(entity, entity.GetCenter(), range);
            entity.PlaySound(VanillaSoundID.explosion);


            return damageOutputs;
        }
        public void DeathEffects(Entity entity, DeathInfo info)
        {
            // 阿努比斯偏移量
            var anubisandOffset = ANUBISAND_OFFSET;
            anubisandOffset.x *= entity.GetFacingX();
            // 生成阿努比斯头骨
            var Anubiskull = entity.SpawnWithParams(VanillaEnemyID.Anubiskull, entity.Position + anubisandOffset);
            // 生成地狱先锋
            var NetherVanguard = entity.SpawnWithParams(VanillaEnemyID.NetherVanguard, entity.Position + anubisandOffset);
            // 生成愤怒逆转者
            var AngryReverser = entity.SpawnWithParams(VanillaEnemyID.AngryReverser, entity.Position + anubisandOffset);
            entity.Remove();
        }
        #endregion

        public bool CanBeDestroyedBySpikes(Entity entity, Entity source)
        {
            return !IsPunctured(entity);
        }
        public void DestroyBySpikes(Entity entity, Entity source)
        {
            Puncture(entity);
        }
        public static void Puncture(Entity entity)
        {
            if (IsPunctured(entity))
                return;
            SetPunctured(entity, true);
            var timer = GetPunctureTimer(entity);
            timer?.Reset();
        }
        public static void Crush(Entity chariot, IEntityCollider otherCollider)
        {
            var other = otherCollider.Entity;
            float damage = other.GetTakenCrushDamage();
            var vehicleInteraction = other.GetVehicleInteraction();
            switch (vehicleInteraction)
            {
                case VehicleInteraction.BLOCK:
                    damage = chariot.GetDamage() * 0.1f;
                    break;
                case VehicleInteraction.IGNORE:
                    return;
            }

            if (!other.IsDead)
            {
                if (vehicleInteraction == VehicleInteraction.BLOCK || other.IsInvincible())
                {
                    var vel = chariot.Velocity;
                    if (vel.x * chariot.GetFacingX() > 0)
                    {
                        vel.x = 0;
                    }
                    chariot.Velocity = vel;
                }
                DamageEffectList damageEffects = new DamageEffectList(VanillaDamageEffects.GRIND, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN);
                otherCollider.TakeDamage(damage, damageEffects, chariot)?.Let(o =>
                {
                    if (o.BodyResult != null && o.BodyResult.Fatal)
                    {
                        if (other.Type == EntityTypes.PLANT || other.Type == EntityTypes.OBSTACLE)
                        {
                            other.PlaySound(VanillaSoundID.smash);
                        }
                        else if (other.Type == EntityTypes.ENEMY)
                        {
                            other.PlaySound(VanillaSoundID.grind);
                        }
                    }
                });
            }
        }

        #region 字段
        public static bool IsPunctured(Entity entity) => entity.GetBehaviourField<bool>(ID, FIELD_PUNCTURED);
        public static void SetPunctured(Entity entity, bool value) => entity.SetBehaviourField(ID, FIELD_PUNCTURED, value);

        public static FrameTimer? GetPunctureTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, FIELD_PUNCTURE_TIMER);
        public static void SetPunctureTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(ID, FIELD_PUNCTURE_TIMER, value);
        #endregion

        public const float BROKEN_THRESOLD = 200;
        public static readonly Vector3 ANUBISAND_OFFSET = new Vector3(-48, 32, 0);
        public const int PUNCTURE_TIME = 40;
        private static readonly VanillaEntityPropertyMeta<bool> FIELD_PUNCTURED = new VanillaEntityPropertyMeta<bool>("Punctured");
        private static readonly VanillaEntityPropertyMeta<FrameTimer> FIELD_PUNCTURE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("PunctureTimer");
        private static readonly NamespaceID ID = VanillaEnemyID.NetherTroopCarrier;
    }
}
