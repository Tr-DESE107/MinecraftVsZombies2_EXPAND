using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.HeldItems;
using MVZ2Logic.SeedPacks;
using MVZ2Logic;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using MVZ2.Vanilla.Grids;
using MVZ2Logic.Level;
using MVZ2.Vanilla;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine.Base;

namespace MVZ2.GameContent.HeldItems
{
    public abstract class BlueprintHeldItemBehaviour : HeldItemBehaviour
    {
        protected BlueprintHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }

        public override bool IsValidFor(HeldItemTarget target, IHeldItemData data)
        {
            return target is HeldItemTargetGrid;
        }
        public override HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data)
        {
            if (target is not HeldItemTargetGrid gridTarget)
                return HeldHighlight.None;

            var grid = gridTarget.Target;

            var level = grid.Level;
            var seed = Definition.GetSeedPack(level, data);
            if (seed != null)
            {
                var seedDef = seed.GetDefinitionID();
                if (grid.CanPlaceBlueprint(seedDef, out _))
                {
                    return HeldHighlight.Green;
                }
                else
                {
                    return HeldHighlight.Red;
                }
            }
            return HeldHighlight.None;
        }
        public override void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
            switch (target)
            {
                case HeldItemTargetGrid gridTarget:
                    {
                        var targetPhase = Global.IsMobile() ? PointerInteraction.Release : PointerInteraction.Press;
                        if (interaction != targetPhase)
                            return;

                        var grid = gridTarget.Target;

                        var level = grid.Level;
                        var seed = Definition.GetSeedPack(level, data);
                        if (seed == null)
                            return;
                        var seedDef = seed.Definition;
                        if (!grid.CanPlaceBlueprint(seedDef.GetID(), out var error))
                        {
                            var message = Global.Game.GetGridErrorMessage(error);
                            if (!string.IsNullOrEmpty(message))
                            {
                                level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, message, 0, 150);
                            }
                            return;
                        }

                        if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                        {
                            OnUseBlueprint(grid, data, seed);
                            seed.UseOnGrid(grid, data);
                            level.ResetHeldItem();
                        }
                    }
                    break;
                case HeldItemTargetLawn lawnTarget:
                    {
                        var level = lawnTarget.Level;
                        var area = lawnTarget.Area;

                        if (area == LawnArea.Side)
                        {
                            if (level.CancelHeldItem())
                            {
                                level.PlaySound(VanillaSoundID.tap);
                            }
                        }
                    }
                    break;
            }
        }
        protected abstract void OnUseBlueprint(LawnGrid grid, IHeldItemData data, SeedPack seed);
    }

    public class ClassicBlueprintHeldItemBehaviour : BlueprintHeldItemBehaviour
    {
        public ClassicBlueprintHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }

        protected override void OnUseBlueprint(LawnGrid grid, IHeldItemData data, SeedPack seed)
        {
            var level = grid.Level;
            level.AddEnergy(-seed.GetCost());
            level.SetRechargeTimeToUsed(seed);
            seed.ResetRecharge();
        }
    }
    public class ConveyorBlueprintHeldItemBehaviour : BlueprintHeldItemBehaviour
    {
        public ConveyorBlueprintHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }

        protected override void OnUseBlueprint(LawnGrid grid, IHeldItemData data, SeedPack seed)
        {
            seed.Level.RemoveConveyorSeedPackAt((int)data.ID);
        }
    }
}
