using System.Collections.Generic;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// ResistanceBuff��ֻ�ṩ�˺�����Ч�����������κ��Ӿ����黯����
    /// </summary>
    [BuffDefinition(VanillaBuffNames.Entity.Resistance)]
    public class ResistanceBuff : BuffDefinition
    {
        public ResistanceBuff(string nsp, string name) : base(nsp, name)
        {
            // ע��ʵ������ǰ�Ļص�������ʵ�ּ��˻���
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
        }

        /// <summary>
        /// �˺�ǰ�ص����ж��Ƿ�ӵ�и�Buff��Ӧ�ü��˱���
        /// </summary>
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult callbackResult)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            if (entity == null)
                return;

            // �������WHACKЧ�����˺�
            //if (damageInfo.Effects.HasEffect(MVZ2.Vanilla.Properties.VanillaDamageEffects.WHACK))
            //    return;

            // ��ȡ��ʵ����������ResistanceBuffʵ��
            buffBuffer.Clear();
            entity.GetBuffs<ResistanceBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            // ����ӵ��Buff��ʵ�壬���ùؿ��ļ��˱��ʺ��������˺�����
            // �����������ResistanceBuff��Level.GetGhostTakenDamageMultiplier()������ֵͬ
            float multiplier = buffBuffer[0].Level.GetGhostTakenDamageMultiplier();
            damageInfo.Multiply(0.9f);
        }

        // �������б�����ÿ�η��䣬��������
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
