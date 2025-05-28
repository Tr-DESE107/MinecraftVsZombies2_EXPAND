﻿using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.randomEnemySpeed)]
    public class RandomEnemySpeedBuff : BuffDefinition
    {
        public RandomEnemySpeedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, PROP_SPEED));
        }
        public static void SetSpeed(Buff buff, float value)
        {
            buff.SetProperty(PROP_SPEED, value);
        }
        public static readonly VanillaBuffPropertyMeta<float> PROP_SPEED = new VanillaBuffPropertyMeta<float>("Speed");
    }
}
