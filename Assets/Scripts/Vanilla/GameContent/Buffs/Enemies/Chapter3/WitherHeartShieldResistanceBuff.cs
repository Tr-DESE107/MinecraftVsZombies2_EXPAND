using System.Collections.Generic;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// WitherHeartShieldResistanceBuff��
    /// ֻ�ṩ�˺�����Ч�����ҵ����ﴦ�ڹ���״̬ʱ������
    /// �������κ��Ӿ����黯����
    /// </summary>
    [BuffDefinition(VanillaBuffNames.WitherHeartShieldResistanceBuff)]
    public class WitherHeartShieldResistanceBuff : BuffDefinition
    {
        public WitherHeartShieldResistanceBuff(string nsp, string name) : base(nsp, name)
        {
            // ע��ʵ������ǰ�Ļص�������ʵ�ּ��˻���
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);

        }

        /// <summary>
        /// �˺�ǰ�ص����ж��Ƿ�ӵ�и�Buff���ҹ���ǹ���״̬ʱӦ�ü��˱���
        /// </summary>
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult callbackResult)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            if (entity == null)
                return;

            //������ҷ�����̶�����75%�ܵ����˺�
            if (!(entity.IsHostile(0)))
            {
                damageInfo.Multiply(1.75f);
                return;
            }

            // ������˴��ڹ���״̬��������25%�ܵ����˺�
            if (entity.State == VanillaEntityStates.ATTACK)
            {
                damageInfo.Multiply(1.25f);
                return;

            }


            // ��ȡ��ʵ����������WitherHeartShieldResistanceBuffʵ��
            buffBuffer.Clear();
            entity.GetBuffs<WitherHeartShieldResistanceBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            // Ӧ�ù̶����˱��ʣ�����0.25������75%��
            damageInfo.Multiply(0.25f);
        }

        // �������б�����ÿ�η��䣬��������
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
