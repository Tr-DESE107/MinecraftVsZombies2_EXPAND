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
using PVZEngine.Models;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.HeldItems
{
    public abstract class BlueprintHeldItemBehaviour : HeldItemBehaviour
    {
        protected BlueprintHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }

        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return target is HeldItemTargetGrid;
        }
        public override void OnUpdate(LevelEngine level, IHeldItemData data)
        {
            base.OnUpdate(level, data);
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
        public override HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            if (target is not HeldItemTargetGrid gridTarget)
                return HeldHighlight.None;

            var grid = gridTarget.Target;

            var level = grid.Level;
            var seed = GetSeedPack(level, data);
            var seedDef = seed?.Definition;
            return grid.GetSeedHeldHighlight(seedDef);
        }
        public override void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            var pointer = pointerParams.pointer;
            if (pointer.type == PointerTypes.MOUSE && pointer.button == MouseButtons.RIGHT)
            {
                OnRightMouseEvent(target, data, pointerParams);
                return;
            }
            if (pointerParams.IsInvalidClickButton())
                return;
            OnMainPointerEvent(target, data, pointerParams);
        }
        private void OnRightMouseEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            var level = target.GetLevel();
            if (level.CancelHeldItem())
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        private void OnMainPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            switch (target)
            {
                case HeldItemTargetGrid gridTarget:
                    OnPointerEventGrid(gridTarget, data, pointerParams);
                    break;
            }
        }
        private void OnPointerEventGrid(HeldItemTargetGrid gridTarget, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidReleaseAction())
                return;
            var level = gridTarget.GetLevel();
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
        private void OnPointerEventLawn(HeldItemTargetLawn lawnTarget, IHeldItemData data, PointerInteractionData pointerParams)
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
        public override void OnSetModel(LevelEngine level, IHeldItemData data, IModelInterface model)
        {
            base.OnSetModel(level, data, model);
            var seedPack = GetSeedPack(level, data);
            if (seedPack == null)
                return;
            if (seedPack.IsCommandBlock())
            {
                model.SetShaderInt("_Grayscale", 1);
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
            seed.SetStartRecharge(false);
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
