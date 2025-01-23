using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Shells;
using PVZEngine.Damages;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.diamond)]
    public class DiamondShell : ShellDefinition
    {
        public DiamondShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, VanillaSoundID.crystal);
            SetProperty(VanillaShellProps.BLOCKS_SLICE, true);
        }
    }
}
