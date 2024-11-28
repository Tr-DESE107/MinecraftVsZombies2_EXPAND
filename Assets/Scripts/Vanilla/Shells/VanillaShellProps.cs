using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Shells
{
    public static class VanillaShellProps
    {
        public const string SLICE_CRITICAL = "sliceCritical";
        public const string SLICE_BLOOD = "sliceBlood";
        public const string BLOCKS_FIRE = "blocksFire";
        public const string HIT_SOUND = "hitSound";

        public static bool BlocksFire(this ShellDefinition shell)
        {
            return shell.GetProperty<bool>(BLOCKS_FIRE);
        }
    }
}
