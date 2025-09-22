using System.Collections.Generic;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// NightmareMaskEvadeBuff��
    /// ֻ�ṩ�˺�����Ч�����ҵ����ﴦ�ڹ���״̬ʱ������
    /// </summary>
    [BuffDefinition(VanillaBuffNames.Enemy.NightmareMaskEvade)]
    public class NightmareMaskEvadeBuff : BuffDefinition
    {
        public NightmareMaskEvadeBuff(string nsp, string name) : base(nsp, name)
        {
            // ע��ʵ������ǰ�Ļص�������ʵ�ּ��˻���
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
            //���棺���˹����ٶȽ���
            AddModifier(new FloatModifier(VanillaEntityProps.ATTACK_SPEED, NumberOperator.Multiply, 0.8f));

        }

        /// <summary>
        /// �˺�ǰ�ص����ж��Ƿ�ӵ�и�Buff
        /// </summary>
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult callbackResult)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            if (entity == null)
                return;






            // ��ȡ��ʵ����������NightmareMaskEvadeBuffʵ��
            buffBuffer.Clear();
            entity.GetBuffs<NightmareMaskEvadeBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            bool hostile = entity.IsHostile(0);
            // ������1/3������ι������ܵ��˺�
            if (entity.RNG.Next(3) == 0 && hostile)
            {
                damageInfo.Multiply(0f);

                return;

            }

        }

        // �������б�����ÿ�η��䣬��������
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
