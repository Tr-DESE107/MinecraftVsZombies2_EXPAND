#nullable enable // 自动生成

using System.Collections.Generic;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using PVZEngine.Definitions;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// 抗性Buff：只提供伤害减免效果，不包含任何视觉层变化
    /// </summary>
    [AutoBuffDefinition(VanillaBuffNames.Entity.Resistance)]
    public class ResistanceBuff : BuffDefinition
    {
        public ResistanceBuff(string nsp, string name) : base(nsp, name)
        {
            // 注册实体伤害前的回调来实现减伤逻辑
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
        }

        /// <summary>
        /// 伤害前回调：判断是否拥有该Buff，应用减伤倍数
        /// </summary>
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult callbackResult)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            if (entity == null)
                return;

            // 忽略带有WHACK效果的伤害
            //if (damageInfo.Effects.HasEffect(MVZ2.Vanilla.Properties.VanillaDamageEffects.WHACK))
            //    return;

            // 获取实体身上的所有ResistanceBuff实例
            buffBuffer.Clear();
            entity.GetBuffs<ResistanceBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            // 对于拥有Buff的实体，应用关卡的幽灵承受伤害倍率和额外的伤害减免
            // 注意这里的ResistanceBuff的Level.GetGhostTakenDamageMultiplier()返回相同的值
            float multiplier = buffBuffer[0].Level.GetGhostTakenDamageMultiplier();
            damageInfo.Multiply(0.9f);
        }

        // 缓冲区列表，避免每次分配，用于缓存
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
