using MVZ2.GameContent.Shells;
using MVZ2.Vanilla;
using PVZEngine.Armors;

namespace MVZ2.GameContent.Armors
{
    [Definition(VanillaArmorNames.berserkerHelmet)]
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
