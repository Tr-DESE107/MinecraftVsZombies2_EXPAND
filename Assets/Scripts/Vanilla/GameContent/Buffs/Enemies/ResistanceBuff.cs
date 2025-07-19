using System.Collections.Generic;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using System.Collections.Generic;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Stages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// ResistanceBuff：只提供伤害减免效果，不包含任何视觉幽灵化表现
    /// </summary>
    [BuffDefinition(VanillaBuffNames.Resistance)]
    public class ResistanceBuff : BuffDefinition
    {
        public ResistanceBuff(string nsp, string name) : base(nsp, name)
        {
            // 注册实体受伤前的回调，用于实现减伤机制
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
        }

        /// <summary>
        /// 伤害前回调：判断是否拥有该Buff，应用减伤倍率
        /// </summary>
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult callbackResult)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            if (entity == null)
                return;

            // 不处理带WHACK效果的伤害
            //if (damageInfo.Effects.HasEffect(MVZ2.Vanilla.Properties.VanillaDamageEffects.WHACK))
            //    return;

            // 获取该实体身上所有ResistanceBuff实例
            buffBuffer.Clear();
            entity.GetBuffs<ResistanceBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            // 对于拥有Buff的实体，调用关卡的减伤倍率函数进行伤害调整
            // 这里假设所有ResistanceBuff的Level.GetGhostTakenDamageMultiplier()返回相同值
            float multiplier = buffBuffer[0].Level.GetGhostTakenDamageMultiplier();
            damageInfo.Multiply(0.9f);
        }

        // 缓存用列表，避免每次分配，提升性能
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
