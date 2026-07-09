#nullable enable // 自动生成

using System.Collections.Generic;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Models;
using MVZ2Logic.Models;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// 六气抗性Buff
    /// 只提供伤害减免效果，并且当怪物处于攻击状态时触发
    /// 不包含任何视觉层变化
    /// </summary>
    [AutoBuffDefinition(VanillaBuffNames.Enemy.SixQiResistanceBuff)]
    public class SixQiResistanceBuff : BuffDefinition
    {
        public SixQiResistanceBuff(string nsp, string name) : base(nsp, name)
        {
            // 注册实体伤害前的回调来实现减伤逻辑
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.EightDiagram, VanillaModelID.EightDiagram);
        }

        /// <summary>
        /// 伤害前回调：判断是否拥有该Buff且处于攻击状态时应用减伤倍数
        /// </summary>
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult callbackResult)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            if (entity == null)
                return;

            // 获取实体身上的所有SixQiResistanceBuff实例
            buffBuffer.Clear();
            entity.GetBuffs<SixQiResistanceBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            // 如果伤害带有虚空(VOID)效果，则造成巨额伤害
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.VOID))
            {
                damageInfo.Multiply(65535f);
            }

            // 应用固定减伤倍率，即99%
            damageInfo.Multiply(0.01f);
        }

        // 缓冲区列表，避免每次分配，用于缓存
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
