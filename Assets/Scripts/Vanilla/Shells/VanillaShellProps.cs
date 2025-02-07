using PVZEngine;
using PVZEngine.Damages;

namespace MVZ2.Vanilla.Shells
{
    [PropertyRegistryRegion]
    public static class VanillaShellProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta SLICE_CRITICAL = Get("sliceCritical");
        public static readonly PropertyMeta BLOCKS_FIRE = Get("blocksFire");
        public static readonly PropertyMeta HIT_SOUND = Get("hitSound");
        public static readonly PropertyMeta BLOCKS_SLICE = Get("blocks_slice");

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
