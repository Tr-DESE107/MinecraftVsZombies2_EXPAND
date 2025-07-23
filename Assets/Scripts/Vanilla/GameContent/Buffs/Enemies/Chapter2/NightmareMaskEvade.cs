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
    /// NightmareMaskEvadeBuff：
    /// 只提供伤害减免效果，且当怪物处于攻击状态时不减伤
    /// 不包含任何视觉幽灵化表现
    /// </summary>
    [BuffDefinition(VanillaBuffNames.NightmareMaskEvade)]
    public class NightmareMaskEvadeBuff : BuffDefinition
    {
        public NightmareMaskEvadeBuff(string nsp, string name) : base(nsp, name)
        {
            // 注册实体受伤前的回调，用于实现减伤机制
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
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

            bool hostile = entity.IsHostile(0);
            // 敌人有1/5概率这次攻击不受到伤害
            if (entity.RNG.Next(6) == 0 && hostile)
            {
                damageInfo.Multiply(0f);
                entity.PlaySound(VanillaSoundID.buzzer);
                return;

            }


            // 获取该实体身上所有NightmareMaskEvadeBuff实例
            buffBuffer.Clear();
            entity.GetBuffs<NightmareMaskEvadeBuff>(buffBuffer);
            if (buffBuffer.Count == 0)
                return;

            

        }

        // 缓存用列表，避免每次分配，提升性能
        private List<Buff> buffBuffer = new List<Buff>();
    }
}
