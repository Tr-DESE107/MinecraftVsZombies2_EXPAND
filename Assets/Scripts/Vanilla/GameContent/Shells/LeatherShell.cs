using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Shells;
using PVZEngine.Damages;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.leather)]
    public class LeatherShell : ShellDefinition
    {
        public LeatherShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, VanillaSoundID.leatherHit);
        }
    }
}
