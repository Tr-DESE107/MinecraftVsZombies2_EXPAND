using MVZ2.GameContent.Shells;
using PVZEngine.Armors;
using PVZEngine.Level;

namespace MVZ2.GameContent.Armors
{
    [ArmorDefinition(VanillaArmorNames.berserkerHelmet)]
    public class BerserkerHelmet : ArmorDefinition
    {
        public BerserkerHelmet(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineArmorProps.SHELL, VanillaShellID.stone);
            SetProperty(EngineArmorProps.MAX_HEALTH, MAX_HEALTH);
        }
        public const float MAX_HEALTH = 100;
    }
}
