using MVZ2.Vanilla;
using PVZEngine.Damages;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.metal)]
    public class MetalShell : ShellDefinition
    {
        public MetalShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, SoundID.shieldHit);
        }
    }
}
