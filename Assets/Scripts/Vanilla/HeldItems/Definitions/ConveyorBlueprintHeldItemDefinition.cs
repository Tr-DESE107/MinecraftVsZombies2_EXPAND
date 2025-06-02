﻿using MVZ2.GameContent.HeldItems;
using MVZ2Logic;
using MVZ2Logic.HeldItems;

namespace MVZ2.Vanilla.HeldItems
{
    [HeldItemDefinition(BuiltinHeldItemNames.conveyor)]
    public class ConveyorBlueprintHeldItemDefinition : BlueprintHeldItemDefinition
    {
        public ConveyorBlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(VanillaHeldItemBehaviourID.conveyorBlueprint);
        }
    }
}
