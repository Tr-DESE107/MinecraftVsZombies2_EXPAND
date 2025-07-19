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
            //SetLastShootHealth(entity, entity.Health);
        }

        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);

            //// ���Ѫ���½��Ƿ񳬹�100���������ϼ�
            //CheckPurpleArrowShoot(contraption);

            // ����״̬���������߼�
            var maxHP = contraption.GetMaxHealth();
            bool netherite = contraption.HasBuff<ObsidianArmorBuff>();
            if (netherite)
            {

                // �������Ѫ������40%���Ƴ�Buff
                if (contraption.Health <= maxHP * 0.4f)
                {
                    var hp = contraption.Health;
                    contraption.RemoveBuffs<ObsidianArmorBuff>();
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
        /// ÿ��Ѫ�����ͳ���200��������ɫ��ʸ����������Ч��
        /// </summary>
        //private void CheckPurpleArrowShoot(Entity entity)
        //{
        //    float lastHP = GetLastShootHealth(entity);
        //    float currHP = entity.Health;

        //    // �Ƚ������½�ֵ�Ƿ񳬹�100
        //    int arrowsToShoot = (int)((lastHP - currHP) / 200f);
        //    arrowsToShoot = Mathf.Min(arrowsToShoot, 2);
        //    if (arrowsToShoot > 0)
        //    {
        //        for (int i = 0; i < arrowsToShoot; i++)
        //        {
        //            // ������ɫ���Ĳ���
        //            var param = entity.GetShootParams();
        //            param.projectileID = VanillaProjectileID.purpleArrow;


        //            // �����ϼ�
        //            var proj = entity.ShootProjectile(param);

        //            // ���÷����ٶȣ��ҷ���
        //            proj.Velocity = new UnityEngine.Vector2(2.5f, 0f);

        //            // ������Ч
        //            entity.Level.PlaySound(VanillaSoundID.bonk);
        //        }

        //        // ���¼�¼������ֵ
        //        SetLastShootHealth(entity, currHP);
        //    }
        //}


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

            //// ����Ѫ����¼
            //SetLastShootHealth(contraption, contraption.Health);
        }
        /// <summary>
        /// ��ȡ����״̬�µĶ���״ֵ̬
        /// </summary>
        private float GetArmoredDamagePercent(Entity contraption, float maxHP)
        {
            var percent = contraption.Health / maxHP;
            var armorPercent = (percent - 0.4f) / 0.6f;
            return 1 - armorPercent;
        }

        // �����ֶμ�¼����ֵ�������ж��Ƿ񴥷�������
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
