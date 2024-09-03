using MVZ2.Vanilla;
using PVZEngine.Definitions;

namespace MVZ2.GameContent.Shells
{
    [Definition(ShellNames.metal)]
    public class MetalShell : ShellDefinition
    {
        public MetalShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(ShellProps.HIT_SOUND, SoundID.shieldHit);
        }
    }
}
