using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using MVZ2.GameContent.Effects;
using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using PVZEngine;
using MVZ2.Vanilla.Level;
using MVZ2.GameContent.Damages;
using PVZEngine.Damages;

namespace MVZ2.GameContent.Enemies
{
    /// <summary>
    /// SkeletonMage（骷髅法师）
    /// 继承自 StateEnemy，拥有攻击状态机（施法 → 发射 → 恢复）循环。
    /// 会在初始化时随机成为 火焰/冰霜/闪电 三个职业之一。
    /// </summary>
    [EntityBehaviourDefinition(VanillaEnemyNames.WintherMage)]
    public class WintherMage : StateEnemy
    {
        // 构造函数：设置目标检测器（忽略高层和低层敌人）
        public WintherMage(string nsp, string name) : base(nsp, name)
        {
            detector = new DispenserDetector()
            {
                ignoreHighEnemy = true, // 不检测高层
                ignoreLowEnemy = true   // 不检测低层
            };
        }

        /// <summary>
        /// 初始化逻辑：设置计时器、职业、动画
        /// </summary>
        public override void Init(Entity entity)
        {
            base.Init(entity);

            // 初始化时进入施法阶段，设置施法计时器
            SetStateTimer(entity, new FrameTimer(ATTACK_CAST_TIME));

            // 随机职业（火焰/冰霜/闪电）
            SetClass(entity, mageClasses.Random(entity.RNG));

            //// 同步动画参数：Class
            //entity.SetAnimationInt("Class", GetClass(entity));
        }

        /// <summary>
        /// 每帧更新逻辑
        /// </summary>
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            // 更新外观的受伤百分比
            entity.SetModelDamagePercent();

            // 确保动画与当前职业一致
            //entity.SetAnimationInt("Class", GetClass(entity));
        }

        /// <summary>
        /// 获取动作状态（走路/攻击/施法/恢复）
        /// </summary>
        protected override int GetActionState(Entity enemy)
        {
            var state = base.GetActionState(enemy);

            // 如果进入攻击状态，并且处于施法阶段 → 动画切换为 ENEMY_CAST
            if (state == VanillaEntityStates.ATTACK)
            {
                if (GetAttackState(enemy) == ATTACK_STATE_CAST)
                {
                    state = VanillaEntityStates.ENEMY_CAST;
                }
            }
            return state;
        }

        /// <summary>
        /// 行走状态逻辑
        /// </summary>
        protected override void UpdateStateWalk(Entity enemy)
        {
            base.UpdateStateWalk(enemy);

            // 行走时也更新目标（以便随时能进入攻击）
            UpdateTarget(enemy);

            // 计时器重置（施法计时）
            var timer = GetStateTimer(enemy);
            if (timer != null)
            {
                timer.ResetTime(ATTACK_CAST_TIME);
            }
        }

        /// <summary>
        /// 施法状态逻辑
        /// </summary>
        protected override void UpdateStateCast(Entity enemy)
        {
            base.UpdateStateCast(enemy);

            var timer = GetStateTimer(enemy);
            if (timer == null)
                return;

            // 按攻击速度运行施法计时器
            timer.Run(enemy.GetAttackSpeed());

            // 施法完成 → 进入发射阶段
            if (timer.Expired)
            {
                SetAttackState(enemy, ATTACK_STATE_FIRE);
                timer.ResetTime(ATTACK_FIRE_TIME);
            }
        }

        /// <summary>
        /// 攻击状态逻辑（发射/恢复）
        /// </summary>
        protected override void UpdateStateAttack(Entity enemy)
        {
            base.UpdateStateAttack(enemy);

            var timer = GetStateTimer(enemy);
            if (timer == null)
                return;

            // 攻击计时器运行
            timer.Run(enemy.GetAttackSpeed());

            if (timer.Expired)
            {
                var attackState = GetAttackState(enemy);

                if (attackState == ATTACK_STATE_FIRE)
                {
                    // 发射阶段完成 → 执行射击 → 进入恢复阶段
                    SetAttackState(enemy, ATTACK_STATE_RESTORE);
                    timer.ResetTime(ATTACK_RESTORE_TIME);
                    Shoot(enemy);
                }
                else if (attackState == ATTACK_STATE_RESTORE)
                {
                    // 恢复阶段完成 → 更新目标 → 重新进入施法阶段
                    UpdateTarget(enemy);
                    SetAttackState(enemy, ATTACK_STATE_CAST);
                    timer.ResetTime(ATTACK_CAST_TIME);
                }
            }
        }

        /// <summary>
        /// 更新攻击目标
        /// </summary>
        private void UpdateTarget(Entity enemy)
        {
            if (CanShoot(enemy))
            {
                // 如果目标存在但无效 → 清空
                if (enemy.Target != null && !ValidateTarget(enemy, enemy.Target))
                {
                    enemy.Target = null;
                }

                // 检测目标
                enemy.Target = FindTarget(enemy);
            }
            else
            {
                enemy.Target = null;
            }
        }

        /// <summary>
        /// 执行射击，根据职业决定攻击方式
        /// </summary>
        private void Shoot(Entity enemy)
        {
            if (shoot_times != 4)
            {
                var param = enemy.GetShootParams();
                param.damage = enemy.GetDamage() * 1.4f;          // 攻击伤害
                param.projectileID = VanillaProjectileID.witherSkull; // 投射物：凋零头颅
                param.soundID = VanillaSoundID.fire;           // 音效：火焰
                enemy.ShootProjectile(param);

                // 计数：连续射击次数
                shoot_times += 1;
            }
            // 每 4 次射击 → 召唤一次陨石
            if (shoot_times >= 4)
            {
                var target = FindTarget(enemy);
                if (target != null)
                {
                    var pos = target.GetCenter() + new Vector3(0, 1280, 0); // 陨石从高空砸下
                    var meteor = enemy.SpawnWithParams(VanillaEffectID.cursedMeteor, pos);
                    meteor.SetParent(enemy);

                    meteor.PlaySound(VanillaSoundID.bombFalling);
                }
                shoot_times = 0;
            }
        }

        // ==== 行为字段（存放于 Entity 上） ====



        public static void SetStateTimer(Entity enemy, FrameTimer value) => enemy.SetBehaviourField(PROP_STATE_TIMER, value);
        public static FrameTimer GetStateTimer(Entity enemy) => enemy.GetBehaviourField<FrameTimer>(PROP_STATE_TIMER);

        public static void SetAttackState(Entity enemy, int value) => enemy.SetBehaviourField(PROP_ATTACK_STATE, value);
        public static int GetAttackState(Entity enemy) => enemy.GetBehaviourField<int>(PROP_ATTACK_STATE);

        public static void SetClass(Entity enemy, int value) => enemy.SetBehaviourField(PROP_CLASS, value);
        public static int GetClass(Entity enemy) => enemy.GetBehaviourField<int>(PROP_CLASS);

        // ==== 目标检测 ====

        /// <summary>
        /// 判断是否可以射击（未超过场地最右列）
        /// </summary>
        protected virtual bool CanShoot(Entity enemy)
        {
            return enemy.Position.x <= enemy.Level.GetEntityColumnX(enemy.Level.GetMaxColumnCount() - 1);
        }

        /// <summary>
        /// 寻找目标（通过检测器）
        /// </summary>
        protected virtual Entity FindTarget(Entity entity)
        {
            var collider = detector.Detect(entity);
            return collider?.Entity;
        }

        /// <summary>
        /// 验证目标是否合法
        /// </summary>
        protected virtual bool ValidateTarget(Entity entity, Entity target)
        {
            return detector.ValidateTarget(entity, target);
        }

        // ==== 成员变量 ====

        private Detector detector; // 目标检测器

        private int shoot_times = 0;

        // ==== 常量：攻击阶段 ====

        public const int ATTACK_STATE_CAST = 0;    // 施法阶段
        public const int ATTACK_STATE_FIRE = 1;    // 发射阶段
        public const int ATTACK_STATE_RESTORE = 2; // 恢复阶段

        // ==== 常量：计时 ====

        public const int ATTACK_CAST_TIME = 5;     // 施法时间
        public const int ATTACK_FIRE_TIME = 5;     // 发射持续时间
        public const int ATTACK_RESTORE_TIME = 60; // 冷却时间

        // ==== 常量：职业 ====

        public const int CLASS_WINTHER = 0;      // 火焰法师
        //public const int CLASS_FROST = 1;     // 冰霜法师
        //public const int CLASS_LIGHTNING = 2; // 闪电法师

        

        // ==== 实体字段（存储在 Entity 内部） ====

        public static readonly VanillaEntityPropertyMeta<int> PROP_ATTACK_STATE = new VanillaEntityPropertyMeta<int>("attackState");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("attackTimer");
        public static readonly VanillaEntityPropertyMeta<int> PROP_CLASS = new VanillaEntityPropertyMeta<int>("class");

        // 职业数组，用于随机职业
        public static int[] mageClasses = new int[]
        {
            //SkeletonMage.CLASS_FIRE,
            //SkeletonMage.CLASS_FROST,
            WintherMage.CLASS_WINTHER
        };
    }
}
