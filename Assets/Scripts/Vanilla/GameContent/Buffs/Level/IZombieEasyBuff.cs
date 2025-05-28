using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.iZombieEasy)]
    public class IZombieEasyBuff : BuffDefinition
    {
        public IZombieEasyBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new IntModifier(VanillaDifficultyProps.IZ_FURNACE_REDSTONE_COUNT, NumberOperator.Add, 2));
        }
    }
}
