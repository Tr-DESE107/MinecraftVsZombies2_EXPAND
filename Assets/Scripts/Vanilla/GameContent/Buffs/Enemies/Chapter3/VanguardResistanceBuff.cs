using System.Collections.Generic;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using MVZ2.Vanilla.Enemies;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// VanguardResistanceBuff��
    /// ֻ�ṩ�˺�����Ч�����ҵ����ﴦ�ڹ���״̬ʱ������
    /// �������κ��Ӿ����黯����
    /// </summary>
    [BuffDefinition(VanillaBuffNames.Enemy.VanguardResistance)]
    public class VanguardResistanceBuff : BuffDefinition
    {
        public VanguardResistanceBuff(string nsp, string name) : base(nsp, name)
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


            // ������˴��ڹ���״̬���򲻼��ˣ�ֱ�ӷ���
            if (entity.State == STATE_MELEE_ATTACK)
            {

                return;

            }


            // ��ȡ��ʵ����������VanguardResistanceBuffʵ��
            buffBuffer.Clear();
            entity.GetBuffs<VanguardResistanceBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            // Ӧ�ù̶����˱��ʣ�����0.1������90%��
            damageInfo.Multiply(0.1f);
        }

        // �������б�����ÿ�η��䣬��������
        private List<Buff> buffBuffer = new List<Buff>();
        public const int STATE_MELEE_ATTACK = VanillaEnemyStates.MELEE_ATTACK;
    }
}
