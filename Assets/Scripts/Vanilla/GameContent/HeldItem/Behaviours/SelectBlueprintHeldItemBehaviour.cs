using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Games;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.HeldItems
{
    internal class SelectBlueprintHeldItemBehaviour : HeldItemBehaviour
    {
        public SelectBlueprintHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }
        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerInteraction)
        {
            var pointer = pointerInteraction.pointer;
            switch (target)
            {
                case HeldItemTargetBlueprint:
                    return true;
            }
            return false;
        }
        public override HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return HeldHighlight.None;
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
                case HeldItemTargetBlueprint blueprintTarget:
                    OnPointerEventBlueprint(blueprintTarget, data, pointerParams);
                    break;
            }
        }
        private void OnPointerEventBlueprint(HeldItemTargetBlueprint target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.pointer.type == PointerTypes.TOUCH)
            {
                if (pointerParams.interaction != PointerInteraction.Release && pointerParams.interaction != PointerInteraction.Down)
                    return;
            }
            else if (pointerParams.pointer.type == PointerTypes.MOUSE)
            {
                if (pointerParams.interaction != PointerInteraction.Down)
                    return;
            }
            else if (pointerParams.pointer.type == PointerTypes.KEY)
            {
                if (pointerParams.interaction != PointerInteraction.Key)
                    return;
            }
            var level = target.Level;
            var seedPack = target.GetSeedPack();
            bool holdingStarshard = level.IsHoldingStarshard();
            bool canInstantEvoke = seedPack?.CanInstantEvoke() ?? false;
            bool instantEvoke = canInstantEvoke && holdingStarshard;

            // 进行立即触发检测。
            bool holdingTrigger = level.IsHoldingTrigger();
            bool canInstantTrigger = seedPack?.CanInstantTrigger() ?? false;
            bool usingTrigger = holdingTrigger && canInstantTrigger;

            bool swapped = Global.IsTriggerSwapped();
            bool instantTrigger = canInstantTrigger && holdingTrigger != swapped;

            bool skipCancelHeld = usingTrigger || instantEvoke;
            if (pointerParams.interaction != PointerInteraction.Release || skipCancelHeld)
            {
                PickupBlueprint(seedPack, instantTrigger, instantEvoke, skipCancelHeld);
            }
        }
        private void PickupBlueprint(SeedPack blueprint, bool instantTrigger, bool instantEvoke, bool skipCancelHeld = false)
        {
            var level = blueprint.Level;
            // 先取消已经手持的物品。
            if (!skipCancelHeld)
            {
                if (level.IsHoldingItem())
                {
                    if (level.CancelHeldItem())
                    {
                        level.PlaySound(VanillaSoundID.tap);
                        return;
                    }
                }
            }
            // 无法拾取蓝图。
            if (!blueprint.CanPick())
            {
                level.PlaySound(VanillaSoundID.buzzer);
                return;
            }
            var blueprintDef = blueprint.Definition;
            var seedType = blueprintDef.GetSeedType();
            if (seedType == SeedTypes.ENTITY)
            {
                var type = BuiltinHeldTypes.blueprint;
                var index = -1;
                if (blueprint is ClassicSeedPack seedPack)
                {
                    index = level.GetSeedPackIndex(seedPack);
                }
                else if (blueprint is ConveyorSeedPack conveyor)
                {
                    type = BuiltinHeldTypes.conveyor;
                    index = level.GetConveyorSeedPackIndex(conveyor);
                }
                var data = new HeldItemData()
                {
                    Type = type,
                    ID = index,
                    InstantTrigger = instantTrigger,
                    InstantEvoke = instantEvoke,
                    Priority = 0,
                };
                // 设置当前手持物品。
                level.SetHeldItem(data);
                level.PlaySound(VanillaSoundID.pick);
            }
            else if (seedType == SeedTypes.OPTION)
            {
                var optionID = blueprintDef.GetSeedOptionID();
                var optionDef = level.Content.GetSeedOptionDefinition(optionID);
                if (optionDef == null)
                    return;
                optionDef.Use(blueprint);
                level.AddEnergy(-blueprint.GetCost());
            }
        }
    }
}
