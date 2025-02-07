using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.littleZombieLevel)]
    public class LittleZombieLevelBuff : BuffDefinition
    {
        public LittleZombieLevelBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaLevelProps.SPAWN_POINTS_MUTLIPLIER, NumberOperator.Multiply, 4));
            AddModifier(new FloatModifier(VanillaLevelProps.SPAWN_POINTS_ADDITION, NumberOperator.Add, 3));
        }
    }
}
