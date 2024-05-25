using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Shells
{
    [Definition(ShellNames.metal)]
    public class MetalShell : ShellDefinition
    {
        public MetalShell()
        {
            SetProperty(ShellProps.HIT_SOUND, SoundID.shieldHit);
        }
    }
}
