using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.iZombieHard)]
    public class IZombieHardBuff : BuffDefinition
    {
        public IZombieHardBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new IntModifier(VanillaLevelProps.FURNACE_DROP_REDSTONE_COUNT, NumberOperator.Add, -2));
        }
    }
}
