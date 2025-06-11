using MVZ2.Vanilla.Entities;
using PVZEngine.Armors;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Armors
{
    [ArmorBehaviourDefinition(VanillaArmorBehaviourNames.damageState3)]
    public class ArmorDamageState3 : ArmorBehaviourDefinition
    {
        public ArmorDamageState3(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostUpdate(Armor armor)
        {
            base.PostUpdate(armor);
            armor.SetModelDamagePercent();
        }
    }
}
