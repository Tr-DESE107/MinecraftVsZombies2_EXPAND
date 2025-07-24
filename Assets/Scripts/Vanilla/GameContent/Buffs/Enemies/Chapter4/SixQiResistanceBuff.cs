using System.Collections.Generic;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// SixQiResistanceBuff��
    /// ֻ�ṩ�˺�����Ч�����ҵ����ﴦ�ڹ���״̬ʱ������
    /// �������κ��Ӿ����黯����
    /// </summary>
    [BuffDefinition(VanillaBuffNames.SixQiResistanceBuff)]
    public class SixQiResistanceBuff : BuffDefinition
    {
        public SixQiResistanceBuff(string nsp, string name) : base(nsp, name)
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



            // ��ȡ��ʵ����������SixQiResistanceBuffʵ��
            buffBuffer.Clear();
            entity.GetBuffs<SixQiResistanceBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            // Ӧ�ù̶����˱��ʣ�����99%
            damageInfo.Multiply(0.01f);
        }

        // �������б�����ÿ�η��䣬��������
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
