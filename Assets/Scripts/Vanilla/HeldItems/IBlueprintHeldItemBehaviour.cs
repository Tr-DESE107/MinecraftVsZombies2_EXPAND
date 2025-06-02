﻿using MVZ2.HeldItems;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.HeldItems
{
    public interface IBlueprintHeldItemBehaviour
    {
        SeedPack GetSeedPack(LevelEngine level, IHeldItemData data);
    }
}
