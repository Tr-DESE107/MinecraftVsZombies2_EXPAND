using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Regeneration)]  // ע����������� Regeneration �� Buff����
    public class RegenerationBuff : BuffDefinition
    {
        public RegenerationBuff(string nsp, string name) : base(nsp, name)
        {
            // ������ӻ�Ѫ��Ч������ð�̹�֮�ࣨ��ѡ��
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.RegenerationParticles, VanillaModelID.RegenerationParticles);
        }

        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var entity = buff.GetEntity();
            if (entity != null && !entity.IsDead)
            {
                // �ָ�һ��������
                float heal = buff.GetProperty<float>(PROP_HEAL_AMOUNT);

                entity.Heal(heal, entity);
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
        public static readonly VanillaBuffPropertyMeta<float> PROP_HEAL_AMOUNT = new VanillaBuffPropertyMeta<float>("HealAmount", 1f); // ÿ�λ�Ѫ��
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");       // ����ʱ��
    }
}
