using MVZ2.Vanilla;
using MVZ2Logic.Audios;
using PVZEngine.Damages;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.leather)]
    public class LeatherShell : ShellDefinition
    {
        public LeatherShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, SoundID.leatherHit);
        }
    }
}
