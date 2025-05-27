using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.minigameEnemySpeed)]
    public class MinigameEnemySpeedBuff : BuffDefinition
    {
        public MinigameEnemySpeedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, PROP_SPEED_MULTIPLIER));
        }
        public static readonly VanillaBuffPropertyMeta<float> PROP_SPEED_MULTIPLIER = new VanillaBuffPropertyMeta<float>("SpeedMultiplier");
    }
}
