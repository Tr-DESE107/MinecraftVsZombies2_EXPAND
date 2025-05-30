using MVZ2.HeldItems;
using MVZ2Logic.HeldItems;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.HeldItems
{
    public static class VanillaHeldItemExt
    {
        public static SeedPack GetSeedPack(this HeldItemDefinition heldItemDef, LevelEngine level, IHeldItemData data)
        {
            var behaviours = heldItemDef.GetBehaviours();
            foreach (var behaviour in behaviours)
            {
                if (behaviour is not IBlueprintHeldItemBehaviour blueprintBehaviour)
                    continue;
                return blueprintBehaviour.GetSeedPack(level, data);
            }
            return null;
        }
    }
}
