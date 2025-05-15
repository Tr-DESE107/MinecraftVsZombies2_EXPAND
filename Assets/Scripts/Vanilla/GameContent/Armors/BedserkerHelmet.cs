using MVZ2.GameContent.Shells;
using PVZEngine.Armors;
using PVZEngine.Level;

namespace MVZ2.GameContent.Armors
{
    [ArmorDefinition(VanillaArmorNames.bedserkerHelmet)]
    public class BedserkerHelmet : ArmorDefinition
    {
        public BedserkerHelmet(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineArmorProps.SHELL, VanillaShellID.stone);
            SetProperty(EngineArmorProps.MAX_HEALTH, MAX_HEALTH);
        }
        public const float MAX_HEALTH = 400;
    }
}
