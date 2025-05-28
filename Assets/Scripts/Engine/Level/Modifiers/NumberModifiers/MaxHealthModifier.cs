using PVZEngine.Armors;
using PVZEngine.Entities;

namespace PVZEngine.Modifiers
{
    public class MaxHealthModifier : FloatModifier
    {
        public MaxHealthModifier(NumberOperator op, float valueConst, int priority = 0) : base(EngineEntityProps.MAX_HEALTH, op, valueConst, priority)
        {
        }

        public MaxHealthModifier(NumberOperator op, PropertyKey<float> buffPropertyName, int priority = 0) : base(EngineEntityProps.MAX_HEALTH, op, buffPropertyName, priority)
        {
        }
    }
    public class ArmorMaxHealthModifier : FloatModifier
    {
        public ArmorMaxHealthModifier(NumberOperator op, float valueConst, int priority = 0) : base(EngineArmorProps.MAX_HEALTH, op, valueConst, priority)
        {
        }

        public ArmorMaxHealthModifier(NumberOperator op, PropertyKey<float> buffPropertyName, int priority = 0) : base(EngineArmorProps.MAX_HEALTH, op, buffPropertyName, priority)
        {
        }
    }
}
