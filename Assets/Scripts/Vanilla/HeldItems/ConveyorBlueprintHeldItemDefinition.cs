using System;
using MVZ2.GameContent.HeldItems;
using MVZ2.HeldItems;
using MVZ2Logic;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.HeldItems
{
    [HeldItemDefinition(BuiltinHeldItemNames.conveyor)]
    public class ConveyorBlueprintHeldItemDefinition : BlueprintHeldItemDefinition
    {
        public ConveyorBlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new ConveyorBlueprintHeldItemBehaviour(this));
        }

        public override SeedPack GetSeedPack(LevelEngine level, IHeldItemData data)
        {
            return level.GetConveyorSeedPackAt((int)data.ID);
        }
    }
}
