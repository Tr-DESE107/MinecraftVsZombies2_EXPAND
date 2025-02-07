using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Shells;
using PVZEngine.Damages;
using PVZEngine.Level;

namespace MVZ2.GameContent.Shells
{
    [ShellDefinition(VanillaShellNames.leather)]
    public class LeatherShell : ShellDefinition
    {
        public LeatherShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, VanillaSoundID.leatherHit);
        }
    }
}
