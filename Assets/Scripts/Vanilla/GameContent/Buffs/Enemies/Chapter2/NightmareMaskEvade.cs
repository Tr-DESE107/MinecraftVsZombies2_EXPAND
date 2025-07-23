using System.Collections.Generic;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Level;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Level;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// NightmareMaskEvadeBuff��
    /// ֻ�ṩ�˺�����Ч�����ҵ����ﴦ�ڹ���״̬ʱ������
    /// �������κ��Ӿ����黯����
    /// </summary>
    [BuffDefinition(VanillaBuffNames.NightmareMaskEvade)]
    public class NightmareMaskEvadeBuff : BuffDefinition
    {
        public NightmareMaskEvadeBuff(string nsp, string name) : base(nsp, name)
        {
            // ע��ʵ������ǰ�Ļص�������ʵ�ּ��˻���
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
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

            bool hostile = entity.IsHostile(0);
            // ������1/5������ι������ܵ��˺�
            if (entity.RNG.Next(6) == 0 && hostile)
            {
                damageInfo.Multiply(0f);
                entity.PlaySound(VanillaSoundID.buzzer);
                return;

            }


            // ��ȡ��ʵ����������NightmareMaskEvadeBuffʵ��
            buffBuffer.Clear();
            entity.GetBuffs<NightmareMaskEvadeBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            

        }

        // �������б�����ÿ�η��䣬��������
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
