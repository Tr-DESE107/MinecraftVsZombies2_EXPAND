using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using MVZ2.GameContent.Buffs.Enemies;


namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.Barrier)]
    public class Barrier : ContraptionBehaviour
    {
        public Barrier(string nsp, string name) : base(nsp, name) { }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            // 初始化血量记录，用于判断是否需要发射紫箭
            //SetLastShootHealth(entity, entity.Health);
            if (!entity.HasBuff<SixQiResistanceBuff>())
            {
                entity.AddBuff<SixQiResistanceBuff>();
            }
        }

        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);

            //// 检测血量下降是否超过100，并发射紫箭
            //CheckPurpleArrowShoot(contraption);

            // 生命状态动画控制逻辑
            var maxHP = contraption.GetMaxHealth();
            bool netherite = contraption.HasBuff<GlowingObsidianArmorBuff>();
            if (netherite)
            {

                // 如果护甲血量掉到40%，移除Buff
                if (contraption.Health <= maxHP * 0.4f)
                {
                    var hp = contraption.Health;
                    contraption.RemoveBuffs<GlowingObsidianArmorBuff>();
                    netherite = false;
                    contraption.Health = hp;

                    Explode(contraption, 120, 1800);
                    contraption.Level.ShakeScreen(10, 0, 15);
                }
            }

            if (netherite)
            {
                var percent = GetArmoredDamagePercent(contraption, maxHP);
                contraption.SetModelDamagePercent(percent);
            }
            else
            {
                contraption.SetModelDamagePercent();
            }
            contraption.SetModelProperty("Netherite", netherite);
        }

        /// <summary>
        /// 每当血量降低超过200，发射紫色箭矢，并播放音效。
        /// </summary>
        //private void CheckPurpleArrowShoot(Entity entity)
        //{
        //    float lastHP = GetLastShootHealth(entity);
        //    float currHP = entity.Health;

        //    // 比较生命下降值是否超过100
        //    int arrowsToShoot = (int)((lastHP - currHP) / 200f);
        //    arrowsToShoot = Mathf.Min(arrowsToShoot, 2);
        //    if (arrowsToShoot > 0)
        //    {
        //        for (int i = 0; i < arrowsToShoot; i++)
        //        {
        //            // 构造紫色箭的参数
        //            var param = entity.GetShootParams();
        //            param.projectileID = VanillaProjectileID.purpleArrow;


        //            // 发射紫箭
        //            var proj = entity.ShootProjectile(param);

        //            // 设置飞行速度（右方向）
        //            proj.Velocity = new UnityEngine.Vector2(2.5f, 0f);

        //            // 播放音效
        //            entity.Level.PlaySound(VanillaSoundID.bonk);
        //        }

        //        // 更新记录的生命值
        //        SetLastShootHealth(entity, currHP);
        //    }
        //}


        public override bool CanEvoke(Entity entity)
        {
            // 有护甲就不能被大招强化
            if (entity.HasBuff<GlowingObsidianArmorBuff>())
                return false;

            return base.CanEvoke(entity);
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);

            // 添加护甲buff
            //contraption.AddBuff<GlowingObsidianArmorBuff>();

            // 血量重置
            contraption.Health = contraption.GetMaxHealth();

            // 播放护甲音效
            contraption.Level.PlaySound(VanillaSoundID.armorUp);

            //// 重置血量记录
            //SetLastShootHealth(contraption, contraption.Health);
        }
        /// <summary>
        /// 获取护甲状态下的动画状态值
        /// </summary>
        private float GetArmoredDamagePercent(Entity contraption, float maxHP)
        {
            var percent = contraption.Health / maxHP;
            var armorPercent = (percent - 0.4f) / 0.6f;
            return 1 - armorPercent;
        }

        // 新增字段记录生命值（用于判断是否触发攻击）
        //private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_SHOOT_HEALTH =
        //    new VanillaEntityPropertyMeta<float>("LastShootHealth");

        //private static float GetLastShootHealth(Entity e) =>
        //    e.GetBehaviourField<float>(PROP_LAST_SHOOT_HEALTH);

        //private static void SetLastShootHealth(Entity e, float hp) =>
        //    e.SetBehaviourField(PROP_LAST_SHOOT_HEALTH, hp);

        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);

            Explode(entity, 120, 1000);
            entity.Level.ShakeScreen(10, 0, 15);

        }

        public static DamageOutput[] Explode(Entity entity, float range, float damage)
        {
            var damageEffects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.EXPLOSION);
            var damageOutputs = entity.Level.Explode(entity.Position, range, entity.GetFaction(), damage, damageEffects, entity);
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
    }
}
