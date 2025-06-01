using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Corropoison)]  // ע����������� Corropoison �� Buff����
    public class CorropoisonBuff : BuffDefinition
    {
        public CorropoisonBuff(string nsp, string name) : base(nsp, name)
        {
            // ������ӻ�Ѫ��Ч������ð�̹�֮�ࣨ��ѡ��
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.CorropoisonParticles, VanillaModelID.CorropoisonParticles);
        }

        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var entity = buff.GetEntity();
            if (entity != null && !entity.IsDead && entity.Health > 1)
            {
                // �۳�һ��������
                float damage = buff.GetProperty<float>(PROP_DAMAGE_AMOUNT);
                entity.TakeDamage(damage, new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.NO_DAMAGE_BLINK, VanillaDamageEffects.IGNORE_ARMOR), entity);
            }

            // Buff����ʱ�߼�
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);

            if (timeout <= 0)
            {
                buff.Remove();
            }
        }

        // PropertyKeyע��
        public static readonly VanillaBuffPropertyMeta<float> PROP_DAMAGE_AMOUNT = new VanillaBuffPropertyMeta<float>("DamageAmount", 1f); // ÿ�λ�Ѫ��
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");       // ����ʱ��
    }
}
