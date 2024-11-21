using MVZ2.Vanilla;
using PVZEngine.Armors;
using PVZEngine.Level;

namespace MVZ2.GameContent.Armors
{
    [Definition(VanillaArmorNames.leatherCap)]
    public class LeatherCap : ArmorDefinition
    {
        public LeatherCap(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineArmorProps.SHELL, VanillaShellID.leather);
            SetProperty(EngineArmorProps.MAX_HEALTH, MAX_HEALTH);
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
        public const float MAX_HEALTH = 370;
    }
}
