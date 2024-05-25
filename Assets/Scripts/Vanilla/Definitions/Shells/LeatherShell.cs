using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Shells
{
    [Definition(ShellNames.leather)]
    public class LeatherShell : ShellDefinition
    {
        public LeatherShell()
        {
            SetProperty(ShellProps.HIT_SOUND, SoundID.leatherHit);
        }
    }
}
