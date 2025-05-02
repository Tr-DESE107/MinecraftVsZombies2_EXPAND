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
            int healthState = 0;
            float maxHP = armor.GetMaxHealth();
            if (armor.Health > maxHP * 2 / 3f)
                healthState = 2;
            else if (armor.Health > maxHP / 3f)
                healthState = 1;
            else
                healthState = 0;
            armor.SetAnimationInt("HealthState", healthState);
        }
    }
}
