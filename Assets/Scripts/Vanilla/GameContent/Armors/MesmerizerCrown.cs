using MVZ2.GameContent.Shells;
using PVZEngine.Armors;
using PVZEngine.Level;

namespace MVZ2.GameContent.Armors
{
    [ArmorDefinition(VanillaArmorNames.mesmerizerCrown)]
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
