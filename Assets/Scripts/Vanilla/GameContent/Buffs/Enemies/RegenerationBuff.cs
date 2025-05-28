using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Damages;
using PVZEngine.Level;
using PVZEngine.Modifiers;
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
                float heal = 1f;
                try { heal = buff.GetProperty<float>(REGEN_HEAL_AMOUNT); } catch { }

                entity.Health = Mathf.Min(entity.Health + heal, entity.GetMaxHealth());
            }

            // Buff����ʱ�߼�
            var timeout = buff.GetProperty<int>(REGEN_TIMEOUT);
            timeout--;
            buff.SetProperty(REGEN_TIMEOUT, timeout);

            if (timeout <= 0)
            {
                buff.Remove();
            }
        }

        // PropertyKeyע��
        public static readonly VanillaBuffPropertyMeta<float> REGEN_HEAL_AMOUNT = new VanillaBuffPropertyMeta<float>("RegenHealAmount"); // ÿ�λ�Ѫ��
        public static readonly VanillaBuffPropertyMeta<int> REGEN_TIMEOUT = new VanillaBuffPropertyMeta<int>("RegenTimeout");       // ����ʱ��
    }
}
