using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

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
        public override void Update(LevelEngine level, IHeldItemData data)
        {
            base.Update(level, data);
            if (!IsValid(level, data))
            {
                level.ResetHeldItem();
            }
        }
        public bool IsValid(LevelEngine level, IHeldItemData data)
        {
            var seedPack = GetSeedPack(level, data);
            if (seedPack == null)
                return false;
            return seedPack.CanPick();
        }
        public override HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data)
        {
            if (target is not HeldItemTargetGrid gridTarget)
                return HeldHighlight.None;

            var grid = gridTarget.Target;

            var level = grid.Level;
            var seed = GetSeedPack(level, data);
            var seedDef = seed?.Definition;
            return grid.GetSeedHeldHighlight(seedDef);
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


                        var level = target.GetLevel();
                        var seed = GetSeedPack(level, data);
                        var seedDef = seed?.Definition;
                        if (seedDef == null)
                            return;
                        var grid = gridTarget.Target;
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
                            CostBlueprint(grid, data);
                            grid.UseEntityBlueprint(seed, data);
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


        protected virtual void CostBlueprint(LawnGrid grid, IHeldItemData data)
        {

        }
        protected SeedPack GetSeedPack(LevelEngine level, IHeldItemData data)
        {
            if (Definition is not IBlueprintHeldItemDefinition blueprintHeld)
                return null;
            return blueprintHeld.GetSeedPack(level, data);
        }
    }

    public class ClassicBlueprintHeldItemBehaviour : BlueprintHeldItemBehaviour
    {
        public ClassicBlueprintHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }

        protected override void CostBlueprint(LawnGrid grid, IHeldItemData data)
        {
            var level = grid.Level;
            var seed = GetSeedPack(level, data);
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

        protected override void CostBlueprint(LawnGrid grid, IHeldItemData data)
        {
            grid.Level.RemoveConveyorSeedPackAt((int)data.ID);
        }
    }
}
