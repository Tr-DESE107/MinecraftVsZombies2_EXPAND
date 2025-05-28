using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.GlowingObsidian)]
    public class GlowingObsidian : ContraptionBehaviour
    {
        public GlowingObsidian(string nsp, string name) : base(nsp, name) { }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            // ��ʼ��Ѫ����¼�������ж��Ƿ���Ҫ�����ϼ�
            SetLastShootHealth(entity, entity.Health);
        }

        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);

            // ���Ѫ���½��Ƿ񳬹�100���������ϼ�
            CheckPurpleArrowShoot(contraption);

            // ����״̬���������߼�
            var state = 0;
            var maxHP = contraption.GetMaxHealth();

            if (contraption.HasBuff<ObsidianArmorBuff>())
            {
                state = GetArmoredHealthState(contraption, maxHP);

                // �������Ѫ������40%���Ƴ�Buff
                if (contraption.Health <= maxHP * 0.4f)
                {
                    contraption.RemoveBuffs<ObsidianArmorBuff>();
                }
            }
            else
            {
                state = GetHealthState(contraption, maxHP);
            }

            contraption.SetAnimationInt("HealthState", state);
        }

        /// <summary>
        /// ÿ��Ѫ�����ͳ���200��������ɫ��ʸ����������Ч��
        /// </summary>
        private void CheckPurpleArrowShoot(Entity entity)
        {
            float lastHP = GetLastShootHealth(entity);
            float currHP = entity.Health;

            // �Ƚ������½�ֵ�Ƿ񳬹�100
            int arrowsToShoot = (int)((lastHP - currHP) / 200f);
            if (arrowsToShoot > 0)
            {
                for (int i = 0; i < arrowsToShoot; i++)
                {
                    // ������ɫ���Ĳ���
                    var param = entity.GetShootParams();
                    param.projectileID = VanillaProjectileID.purpleArrow;


                    // �����ϼ�
                    var proj = entity.ShootProjectile(param);

                    // ���÷����ٶȣ��ҷ���
                    proj.Velocity = new UnityEngine.Vector2(2.5f, 0f);

                    // ������Ч
                    //entity.Level.PlaySound(VanillaSoundID.mvz2:bonk);
                }

                // ���¼�¼������ֵ
                SetLastShootHealth(entity, currHP);
            }
        }


        public override bool CanEvoke(Entity entity)
        {
            // �л��׾Ͳ��ܱ�����ǿ��
            if (entity.HasBuff<ObsidianArmorBuff>())
                return false;

            return base.CanEvoke(entity);
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);

            // ��ӻ���buff
            contraption.AddBuff<ObsidianArmorBuff>();

            // Ѫ������
            contraption.Health = contraption.GetMaxHealth();

            // ���Ż�����Ч
            contraption.Level.PlaySound(VanillaSoundID.armorUp);

            // ����Ѫ����¼
            SetLastShootHealth(contraption, contraption.Health);
        }

        /// <summary>
        /// ��ȡ����״̬�µĶ���״ֵ̬
        /// </summary>
        private int GetArmoredHealthState(Entity contraption, float maxHP)
        {
            if (contraption.Health <= 0.4f * maxHP)
            {
                return GetHealthState(contraption, maxHP * 0.4f);
            }
            else if (contraption.Health <= 0.6f * maxHP)
            {
                return 3;
            }
            else if (contraption.Health <= 0.8f * maxHP)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        /// <summary>
        /// ��ȡ����״̬�µĶ���״ֵ̬
        /// </summary>
        private int GetHealthState(Entity contraption, float maxHP)
        {
            if (contraption.Health <= maxHP / 3)
            {
                return 0;
            }
            else if (contraption.Health <= maxHP * 2 / 3)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        //�����ֶμ�¼����ֵ�������ж��Ƿ񴥷�������
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_SHOOT_HEALTH =
            new VanillaEntityPropertyMeta<float>("LastShootHealth");

        private static float GetLastShootHealth(Entity e) =>
            e.GetBehaviourField<float>(PROP_LAST_SHOOT_HEALTH);

        private static void SetLastShootHealth(Entity e, float hp) =>
            e.SetBehaviourField(PROP_LAST_SHOOT_HEALTH, hp);
    }
}
