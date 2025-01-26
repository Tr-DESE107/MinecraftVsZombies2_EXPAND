using PVZEngine;
using PVZEngine.Damages;

namespace MVZ2.Vanilla.Shells
{
    public static class VanillaShellProps
    {
        public const string SLICE_CRITICAL = "sliceCritical";
        public const string BLOCKS_FIRE = "blocksFire";
        public const string HIT_SOUND = "hitSound";
        public const string BLOCKS_SLICE = "blocks_slice";

        public static bool IsSliceCritical(this ShellDefinition shell)
        {
            return shell.GetProperty<bool>(SLICE_CRITICAL);
        }
        public static bool BlocksFire(this ShellDefinition shell)
        {
            return shell.GetProperty<bool>(BLOCKS_FIRE);
        }
        public static bool BlocksSlice(this ShellDefinition shell)
        {
            return shell.GetProperty<bool>(BLOCKS_SLICE);
        }
        public static NamespaceID GetHitSound(this ShellDefinition shell)
        {
            return shell.GetProperty<NamespaceID>(HIT_SOUND);
        }
    }
}
