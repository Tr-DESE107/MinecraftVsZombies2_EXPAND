﻿using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;


namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.obsidian)]
    public class Obsidian : ContraptionBehaviour
    {
        public Obsidian(string nsp, string name) : base(nsp, name) { }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            // 初始化血量记录，用于判断是否需要发射紫箭
            SetLastShootHealth(entity, entity.Health);
        }

        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);

            // 检测血量下降是否超过100，并发射紫箭
            CheckPurpleArrowShoot(contraption);

            // 生命状态动画控制逻辑
            var maxHP = contraption.GetMaxHealth();
            bool netherite = contraption.HasBuff<ObsidianArmorBuff>();
            if (netherite)
            {

                // 如果护甲血量掉到40%，移除Buff
                if (contraption.Health <= maxHP * 0.4f)
                {
                    var hp = contraption.Health;
                    contraption.RemoveBuffs<ObsidianArmorBuff>();
                    netherite = false;
                    contraption.Health = hp;
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
        private void CheckPurpleArrowShoot(Entity entity)
        {
            float lastHP = GetLastShootHealth(entity);
            float currHP = entity.Health;

            // 比较生命下降值是否超过100
            int arrowsToShoot = (int)((lastHP - currHP) / 200f);
            arrowsToShoot = Mathf.Min(arrowsToShoot, 2);
            if (arrowsToShoot > 0)
            {
                for (int i = 0; i < arrowsToShoot; i++)
                {
                    // 构造紫色箭的参数
                    var param = entity.GetShootParams();
                    param.projectileID = VanillaProjectileID.purpleArrow;


                    // 发射紫箭
                    var proj = entity.ShootProjectile(param);

                    // 设置飞行速度（右方向）
                    proj.Velocity = new UnityEngine.Vector2(2.5f, 0f);

                    // 播放音效
                    entity.Level.PlaySound(VanillaSoundID.bonk);
                }

                // 更新记录的生命值
                SetLastShootHealth(entity, currHP);
            }
        }


        public override bool CanEvoke(Entity entity)
        {
            // 有护甲就不能被大招强化
            if (entity.HasBuff<ObsidianArmorBuff>())
                return false;

            return base.CanEvoke(entity);
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);

            // 添加护甲buff
            contraption.AddBuff<ObsidianArmorBuff>();

            // 血量重置
            contraption.Health = contraption.GetMaxHealth();

            // 播放护甲音效
            contraption.Level.PlaySound(VanillaSoundID.armorUp);

            // 重置血量记录
            SetLastShootHealth(contraption, contraption.Health);
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
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_SHOOT_HEALTH =
            new VanillaEntityPropertyMeta<float>("LastShootHealth");

        private static float GetLastShootHealth(Entity e) =>
            e.GetBehaviourField<float>(PROP_LAST_SHOOT_HEALTH);

        private static void SetLastShootHealth(Entity e, float hp) =>
            e.SetBehaviourField(PROP_LAST_SHOOT_HEALTH, hp);
    }
}
