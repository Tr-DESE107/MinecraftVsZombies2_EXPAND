using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.passengerEnterHouse)]
    public class PassengerEnterHouseBuff : BuffDefinition
    {
        public PassengerEnterHouseBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.UPDATE_AFTER_GAME_OVER, true));
        }
    }
}
