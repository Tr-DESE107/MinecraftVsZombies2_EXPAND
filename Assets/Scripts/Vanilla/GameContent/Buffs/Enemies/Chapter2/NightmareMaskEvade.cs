#nullable enable // 自动生成

using System.Collections.Generic;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    /// <summary>
    /// 噩梦面具闪避Buff
    /// 只提供伤害减免效果，并且当怪物处于攻击状态时触发
    /// </summary>
    [AutoBuffDefinition(VanillaBuffNames.Enemy.NightmareMaskEvade)]
    public class NightmareMaskEvadeBuff : BuffDefinition
    {
        public NightmareMaskEvadeBuff(string nsp, string name) : base(nsp, name)
        {
            // 注册实体伤害前的回调来实现减伤逻辑
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
            // 效果：攻击速度降低
            AddModifier(new FloatModifier(VanillaEntityProps.ATTACK_SPEED, NumberOperator.Multiply, 0.8f));
        }

        /// <summary>
        /// 伤害前回调：判断是否拥有该Buff
        /// </summary>
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult callbackResult)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            if (entity == null)
                return;

            // 获取实体身上的所有NightmareMaskEvadeBuff实例
            buffBuffer.Clear();
            entity.GetBuffs<NightmareMaskEvadeBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            bool hostile = entity.IsHostile(0);
            // 有1/3几率免疫该次受到的伤害
            if (entity.RNG.Next(3) == 0 && hostile)
            {
                damageInfo.Multiply(0f);
                return;
            }
        }

        // 缓冲区列表，避免每次分配，用于缓存
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
