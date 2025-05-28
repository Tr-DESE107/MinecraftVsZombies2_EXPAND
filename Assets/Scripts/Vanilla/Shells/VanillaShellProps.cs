﻿using PVZEngine;
using PVZEngine.Damages;

namespace MVZ2.Vanilla.Shells
{
    [PropertyRegistryRegion(PropertyRegions.shell)]
    public static class VanillaShellProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<bool> SLICE_CRITICAL = Get<bool>("sliceCritical");
        public static readonly PropertyMeta<bool> BLOCKS_FIRE = Get<bool>("blocksFire");
        public static readonly PropertyMeta<NamespaceID> HIT_SOUND = Get<NamespaceID>("hitSound");
        public static readonly PropertyMeta<bool> BLOCKS_SLICE = Get<bool>("blocks_slice");

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
