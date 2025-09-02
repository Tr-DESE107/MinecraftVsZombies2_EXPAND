using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.WitherProtection)]
    public class WitherProtection : BuffDefinition
    {
        public WitherProtection(string nsp, string name) : base(nsp, name)
        {
            // 注册回调：在实体即将受到伤害前触发
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback, priority: -100);
        }

        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult result)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;

            // 从实体获取 WitherProtection buff实例  
            var buff = entity.GetFirstBuff<WitherProtection>();
            if (buff == null)
                return;

            // 如果伤害包含"WITHER"效果，则减少伤害  
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.WITHER))
            {
                var level = buff.GetProperty<float>(PROP_Protection_Level); // 明确指定类型参数  
                damageInfo.Multiply(1f - level); // 现在level是float类型  
            }
        }

        // 定义一个 Buff 属性元数据，用于存储防护等级
        public static readonly VanillaBuffPropertyMeta<float> PROP_Protection_Level =
            new VanillaBuffPropertyMeta<float>("Wither_Protection_Level");
    }
}
