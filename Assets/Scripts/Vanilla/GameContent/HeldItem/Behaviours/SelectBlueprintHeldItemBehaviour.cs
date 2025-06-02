﻿using MVZ2.GameContent.Pickups;
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
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.selectBlueprint)]
    public class SelectBlueprintHeldItemBehaviour : HeldItemBehaviourDefinition
    {
        public SelectBlueprintHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerInteraction)
        {
            var pointer = pointerInteraction.pointer;
            switch (target)
            {
                case HeldItemTargetBlueprint:
                    return true;
                case HeldItemTargetEntity entityTarget:
                    return entityTarget.Target.IsBlueprintPickup();
            }
            return false;
        }
        public override HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return HeldHighlight.None;
        }
        public override void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidClickButton())
                return;
            OnMainPointerEvent(target, data, pointerParams);
        }
        private void OnMainPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            switch (target)
            {
                case HeldItemTargetBlueprint blueprintTarget:
                    OnPointerEventBlueprint(blueprintTarget, data, pointerParams);
                    break;
                case HeldItemTargetEntity entityTarget:
                    OnPointerEventEntity(entityTarget, data, pointerParams);
                    break;
            }
        }
        private PickData GetBlueprintPickData(LevelEngine level, bool canInstantEvoke, bool canInstantTrigger)
        {
            // 进行激发检测。
            bool holdingStarshard = level.IsHoldingStarshard();
            bool willEvoke = holdingStarshard && canInstantEvoke;

            // 进行立即触发检测。
            bool holdingTrigger = level.IsHoldingTrigger();
            bool willTrigger = holdingTrigger && canInstantTrigger;
            bool swapped = Global.IsTriggerSwapped();
            bool triggerValue = canInstantTrigger && holdingTrigger != swapped;

            return new PickData(willEvoke, willTrigger, triggerValue);
        }

        #region 蓝图
        private void OnPointerEventBlueprint(HeldItemTargetBlueprint target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (!IsValidPointer(pointerParams))
                return;
            var level = target.Level;
            var seedPack = target.GetSeedPack();
            if (seedPack == null)
                return;

            var pickData = GetBlueprintPickData(seedPack);

            var enchanted = pickData.IsEnchanted();
            if (pointerParams.interaction == PointerInteraction.Release && !enchanted)
                return;
            PickupBlueprint(seedPack, pickData);
        }
        private PickData GetBlueprintPickData(SeedPack seedPack)
        {
            bool canInstantEvoke = seedPack.CanInstantEvoke();
            bool canInstantTrigger = seedPack.CanInstantTrigger();
            return GetBlueprintPickData(seedPack.Level, canInstantEvoke, canInstantTrigger);
        }
        private void PickupBlueprint(SeedPack blueprint, PickData pickData)
        {
            var level = blueprint.Level;
            // 先取消已经手持的物品。
            if (!pickData.IsEnchanted() && level.IsHoldingExclusiveItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(VanillaSoundID.tap);
                }
                return;
            }
            // 无法拾取蓝图。
            if (!blueprint.CanPick())
            {
                level.PlaySound(VanillaSoundID.buzzer);
                return;
            }
            var blueprintDef = blueprint.Definition;
            var seedType = blueprintDef.GetSeedType();
            switch (seedType)
            {
                case SeedTypes.ENTITY:
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
                            InstantTrigger = pickData.triggerValue,
                            InstantEvoke = pickData.instantEvoke,
                            Priority = 0,
                        };
                        // 设置当前手持物品。
                        level.SetHeldItem(data);
                        level.PlaySound(VanillaSoundID.pick);
                        break;
                    }

                case SeedTypes.OPTION:
                    {
                        var optionID = blueprintDef.GetSeedOptionID();
                        var optionDef = level.Content.GetSeedOptionDefinition(optionID);
                        if (optionDef == null)
                            return;
                        optionDef.Use(blueprint);
                        level.AddEnergy(-blueprint.GetCost());
                        break;
                    }
            }
        }
        #endregion

        #region 蓝图掉落物
        private void OnPointerEventEntity(HeldItemTargetEntity target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (!IsValidPointer(pointerParams))
                return;
            var pickup = target.Target;
            var level = pickup.Level;
            var seedDefinition = BlueprintPickup.GetSeedDefinition(pickup);
            if (seedDefinition == null)
                return;

            var pickData = GetBlueprintPickData(level, seedDefinition);

            var enchanted = pickData.IsEnchanted();
            if (pointerParams.interaction == PointerInteraction.Release && !enchanted)
                return;
            PickupBlueprintPickup(pickup, pickData);
        }
        private PickData GetBlueprintPickData(LevelEngine level, SeedDefinition seedDef)
        {
            bool canInstantEvoke = seedDef.CanInstantEvoke();
            bool canInstantTrigger = seedDef.IsTriggerActive() && seedDef.CanInstantTrigger();
            return GetBlueprintPickData(level, canInstantEvoke, canInstantTrigger);
        }
        private void PickupBlueprintPickup(Entity pickup, PickData pickData)
        {
            var level = pickup.Level;
            // 先取消已经手持的物品。
            if (!pickData.IsEnchanted() && level.IsHoldingExclusiveItem())
            {
                if (level.CancelHeldItem())
                {
                    level.PlaySound(VanillaSoundID.tap);
                }
                return;
            }
            var blueprintDef = BlueprintPickup.GetSeedDefinition(pickup);
            var seedType = blueprintDef.GetSeedType();
            switch (seedType)
            {
                case SeedTypes.ENTITY:
                    {
                        var type = VanillaHeldTypes.blueprintPickup;
                        var data = new HeldItemData()
                        {
                            Type = type,
                            ID = pickup.ID,
                            InstantTrigger = pickData.triggerValue,
                            InstantEvoke = pickData.instantEvoke,
                            Priority = 0,
                        };
                        // 设置当前手持物品。
                        level.SetHeldItem(data);
                        pickup.PlaySound(VanillaSoundID.pick);
                        break;
                    }

                case SeedTypes.OPTION:
                    {
                        var optionID = blueprintDef.GetSeedOptionID();
                        var optionDef = level.Content.GetSeedOptionDefinition(optionID);
                        if (optionDef == null)
                            return;
                        optionDef.Use(level, blueprintDef);
                        break;
                    }
            }
        }
        #endregion

        private bool IsValidPointer(PointerInteractionData pointerParams)
        {
            if (pointerParams.pointer.type == PointerTypes.TOUCH)
            {
                if (pointerParams.interaction != PointerInteraction.Release && pointerParams.interaction != PointerInteraction.Down)
                    return false;
            }
            else if (pointerParams.pointer.type == PointerTypes.MOUSE)
            {
                if (pointerParams.interaction != PointerInteraction.Down)
                    return false;
            }
            else if (pointerParams.pointer.type == PointerTypes.KEY)
            {
                if (pointerParams.interaction != PointerInteraction.Key)
                    return false;
            }
            return true;
        }

        private struct PickData
        {
            public bool instantEvoke;
            public bool instantTrigger;
            public bool triggerValue;

            public PickData(bool instantEvoke, bool instantTrigger, bool triggerValue)
            {
                this.instantEvoke = instantEvoke;
                this.instantTrigger = instantTrigger;
                this.triggerValue = triggerValue;
            }

            public bool IsEnchanted()
            {
                return instantEvoke || instantTrigger;
            }
        }
    }
}
