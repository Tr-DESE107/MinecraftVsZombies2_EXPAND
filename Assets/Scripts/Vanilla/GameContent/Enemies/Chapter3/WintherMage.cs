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
    /// SkeletonMage�����÷�ʦ��
    /// �̳��� StateEnemy��ӵ�й���״̬����ʩ�� �� ���� �� �ָ���ѭ����
    /// ���ڳ�ʼ��ʱ�����Ϊ ����/��˪/���� ����ְҵ֮һ��
    /// </summary>
    [EntityBehaviourDefinition(VanillaEnemyNames.WintherMage)]
    public class WintherMage : StateEnemy
    {
        // ���캯��������Ŀ�����������Ը߲�͵Ͳ���ˣ�
        public WintherMage(string nsp, string name) : base(nsp, name)
        {
            detector = new DispenserDetector()
            {
                ignoreHighEnemy = true, // �����߲�
                ignoreLowEnemy = true   // �����Ͳ�
            };
        }

        /// <summary>
        /// ��ʼ���߼������ü�ʱ����ְҵ������
        /// </summary>
        public override void Init(Entity entity)
        {
            base.Init(entity);

            // ��ʼ��ʱ����ʩ���׶Σ�����ʩ����ʱ��
            SetStateTimer(entity, new FrameTimer(ATTACK_CAST_TIME));

            // ���ְҵ������/��˪/���磩
            SetClass(entity, mageClasses.Random(entity.RNG));

            //// ͬ������������Class
            //entity.SetAnimationInt("Class", GetClass(entity));
        }

        /// <summary>
        /// ÿ֡�����߼�
        /// </summary>
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            // ������۵����˰ٷֱ�
            entity.SetModelDamagePercent();

            // ȷ�������뵱ǰְҵһ��
            //entity.SetAnimationInt("Class", GetClass(entity));
        }

        /// <summary>
        /// ��ȡ����״̬����·/����/ʩ��/�ָ���
        /// </summary>
        protected override int GetActionState(Entity enemy)
        {
            var state = base.GetActionState(enemy);

            // ������빥��״̬�����Ҵ���ʩ���׶� �� �����л�Ϊ ENEMY_CAST
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
        /// ����״̬�߼�
        /// </summary>
        protected override void UpdateStateWalk(Entity enemy)
        {
            base.UpdateStateWalk(enemy);

            // ����ʱҲ����Ŀ�꣨�Ա���ʱ�ܽ��빥����
            UpdateTarget(enemy);

            // ��ʱ�����ã�ʩ����ʱ��
            var timer = GetStateTimer(enemy);
            if (timer != null)
            {
                timer.ResetTime(ATTACK_CAST_TIME);
            }
        }

        /// <summary>
        /// ʩ��״̬�߼�
        /// </summary>
        protected override void UpdateStateCast(Entity enemy)
        {
            base.UpdateStateCast(enemy);

            var timer = GetStateTimer(enemy);
            if (timer == null)
                return;

            // �������ٶ�����ʩ����ʱ��
            timer.Run(enemy.GetAttackSpeed());

            // ʩ����� �� ���뷢��׶�
            if (timer.Expired)
            {
                SetAttackState(enemy, ATTACK_STATE_FIRE);
                timer.ResetTime(ATTACK_FIRE_TIME);
            }
        }

        /// <summary>
        /// ����״̬�߼�������/�ָ���
        /// </summary>
        protected override void UpdateStateAttack(Entity enemy)
        {
            base.UpdateStateAttack(enemy);

            var timer = GetStateTimer(enemy);
            if (timer == null)
                return;

            // ������ʱ������
            timer.Run(enemy.GetAttackSpeed());

            if (timer.Expired)
            {
                var attackState = GetAttackState(enemy);

                if (attackState == ATTACK_STATE_FIRE)
                {
                    // ����׶���� �� ִ����� �� ����ָ��׶�
                    SetAttackState(enemy, ATTACK_STATE_RESTORE);
                    timer.ResetTime(ATTACK_RESTORE_TIME);
                    Shoot(enemy);
                }
                else if (attackState == ATTACK_STATE_RESTORE)
                {
                    // �ָ��׶���� �� ����Ŀ�� �� ���½���ʩ���׶�
                    UpdateTarget(enemy);
                    SetAttackState(enemy, ATTACK_STATE_CAST);
                    timer.ResetTime(ATTACK_CAST_TIME);
                }
            }
        }

        /// <summary>
        /// ���¹���Ŀ��
        /// </summary>
        private void UpdateTarget(Entity enemy)
        {
            if (CanShoot(enemy))
            {
                // ���Ŀ����ڵ���Ч �� ���
                if (enemy.Target != null && !ValidateTarget(enemy, enemy.Target))
                {
                    enemy.Target = null;
                }

                // ���Ŀ��
                enemy.Target = FindTarget(enemy);
            }
            else
            {
                enemy.Target = null;
            }
        }

        /// <summary>
        /// ִ�����������ְҵ����������ʽ
        /// </summary>
        private void Shoot(Entity enemy)
        {
            if (shoot_times != 4)
            {
                var param = enemy.GetShootParams();
                param.damage = enemy.GetDamage() * 1.4f;          // �����˺�
                param.projectileID = VanillaProjectileID.witherSkull; // Ͷ�������ͷ­
                param.soundID = VanillaSoundID.fire;           // ��Ч������
                enemy.ShootProjectile(param);

                // �����������������
                shoot_times += 1;
            }
            // ÿ 4 ����� �� �ٻ�һ����ʯ
            if (shoot_times >= 4)
            {
                var target = FindTarget(enemy);
                if (target != null)
                {
                    var pos = target.GetCenter() + new Vector3(0, 1280, 0); // ��ʯ�Ӹ߿�����
                    var meteor = enemy.SpawnWithParams(VanillaEffectID.cursedMeteor, pos);
                    meteor.SetParent(enemy);

                    meteor.PlaySound(VanillaSoundID.bombFalling);
                }
                shoot_times = 0;
            }
        }

        // ==== ��Ϊ�ֶΣ������ Entity �ϣ� ====



        public static void SetStateTimer(Entity enemy, FrameTimer value) => enemy.SetBehaviourField(PROP_STATE_TIMER, value);
        public static FrameTimer GetStateTimer(Entity enemy) => enemy.GetBehaviourField<FrameTimer>(PROP_STATE_TIMER);

        public static void SetAttackState(Entity enemy, int value) => enemy.SetBehaviourField(PROP_ATTACK_STATE, value);
        public static int GetAttackState(Entity enemy) => enemy.GetBehaviourField<int>(PROP_ATTACK_STATE);

        public static void SetClass(Entity enemy, int value) => enemy.SetBehaviourField(PROP_CLASS, value);
        public static int GetClass(Entity enemy) => enemy.GetBehaviourField<int>(PROP_CLASS);

        // ==== Ŀ���� ====

        /// <summary>
        /// �ж��Ƿ���������δ�������������У�
        /// </summary>
        protected virtual bool CanShoot(Entity enemy)
        {
            return enemy.Position.x <= enemy.Level.GetEntityColumnX(enemy.Level.GetMaxColumnCount() - 1);
        }

        /// <summary>
        /// Ѱ��Ŀ�꣨ͨ���������
        /// </summary>
        protected virtual Entity FindTarget(Entity entity)
        {
            var collider = detector.Detect(entity);
            return collider?.Entity;
        }

        /// <summary>
        /// ��֤Ŀ���Ƿ�Ϸ�
        /// </summary>
        protected virtual bool ValidateTarget(Entity entity, Entity target)
        {
            return detector.ValidateTarget(entity, target);
        }

        // ==== ��Ա���� ====

        private Detector detector; // Ŀ������

        private int shoot_times = 0;

        // ==== �����������׶� ====

        public const int ATTACK_STATE_CAST = 0;    // ʩ���׶�
        public const int ATTACK_STATE_FIRE = 1;    // ����׶�
        public const int ATTACK_STATE_RESTORE = 2; // �ָ��׶�

        // ==== ��������ʱ ====

        public const int ATTACK_CAST_TIME = 5;     // ʩ��ʱ��
        public const int ATTACK_FIRE_TIME = 5;     // �������ʱ��
        public const int ATTACK_RESTORE_TIME = 60; // ��ȴʱ��

        // ==== ������ְҵ ====

        public const int CLASS_WINTHER = 0;      // ���淨ʦ
        //public const int CLASS_FROST = 1;     // ��˪��ʦ
        //public const int CLASS_LIGHTNING = 2; // ���編ʦ

        

        // ==== ʵ���ֶΣ��洢�� Entity �ڲ��� ====

        public static readonly VanillaEntityPropertyMeta<int> PROP_ATTACK_STATE = new VanillaEntityPropertyMeta<int>("attackState");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("attackTimer");
        public static readonly VanillaEntityPropertyMeta<int> PROP_CLASS = new VanillaEntityPropertyMeta<int>("class");

        // ְҵ���飬�������ְҵ
        public static int[] mageClasses = new int[]
        {
            //SkeletonMage.CLASS_FIRE,
            //SkeletonMage.CLASS_FROST,
            WintherMage.CLASS_WINTHER
        };
    }
}
