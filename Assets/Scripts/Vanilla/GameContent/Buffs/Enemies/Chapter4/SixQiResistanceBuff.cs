using System.Collections.Generic;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// SixQiResistanceBuff：
    /// 只提供伤害减免效果，且当怪物处于攻击状态时不减伤
    /// 不包含任何视觉幽灵化表现
    /// </summary>
    [BuffDefinition(VanillaBuffNames.SixQiResistanceBuff)]
    public class SixQiResistanceBuff : BuffDefinition
    {
        public SixQiResistanceBuff(string nsp, string name) : base(nsp, name)
        {
            // 注册实体受伤前的回调，用于实现减伤机制
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);

        }

        /// <summary>
        /// 伤害前回调：判断是否拥有该Buff，且怪物非攻击状态时应用减伤倍率
        /// </summary>
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult callbackResult)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            if (entity == null)
                return;



            // 获取该实体身上所有SixQiResistanceBuff实例
            buffBuffer.Clear();
            entity.GetBuffs<SixQiResistanceBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            // 应用固定减伤倍率，减伤99%
            damageInfo.Multiply(0.01f);
        }

        // 缓存用列表，避免每次分配，提升性能
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
