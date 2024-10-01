using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Shells
{
    [Definition(ShellNames.bone)]
    public class BoneShell : ShellDefinition
    {
        public BoneShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(ShellProps.HIT_SOUND, SoundID.boneHit);
        }
    }
}
