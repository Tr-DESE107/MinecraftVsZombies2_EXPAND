using MVZ2.Vanilla;
using PVZEngine.Damages;
using PVZEngine.Level;

namespace MVZ2.GameContent.Shells
{
    [Definition(VanillaShellNames.bone)]
    public class BoneShell : ShellDefinition
    {
        public BoneShell(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaShellProps.HIT_SOUND, SoundID.boneHit);
        }
    }
}
