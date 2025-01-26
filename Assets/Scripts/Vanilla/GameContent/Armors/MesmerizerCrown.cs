using MVZ2.GameContent.Shells;
using MVZ2.Vanilla;
using PVZEngine.Armors;

namespace MVZ2.GameContent.Armors
{
    [Definition(VanillaArmorNames.mesmerizerCrown)]
    public class MesmerizerCrown : ArmorDefinition
    {
        public MesmerizerCrown(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineArmorProps.SHELL, VanillaShellID.metal);
            SetProperty(EngineArmorProps.MAX_HEALTH, MAX_HEALTH);
        }
        public const float MAX_HEALTH = 100;
    }
}
