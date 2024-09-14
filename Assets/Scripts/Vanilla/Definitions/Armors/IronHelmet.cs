using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Armors
{
    [Definition(ArmorNames.ironHelmet)]
    public class IronHelmet : ArmorDefinition
    {
        public IronHelmet(string nsp, string name) : base(nsp, name)
        {
            SetProperty(ArmorProperties.SHELL, ShellID.metal);
            SetProperty(ArmorProperties.MAX_HEALTH, MAX_HEALTH);
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
            armor.Owner.SetAnimationInt("HealthState", healthState, EntityAnimationTarget.Armor);
        }
        public const float MAX_HEALTH = 1110;
    }
}
