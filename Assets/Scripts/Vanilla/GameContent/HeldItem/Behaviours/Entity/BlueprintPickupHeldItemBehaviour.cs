using MVZ2.GameContent.Pickups;
using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.blueprintPickup)]
    public class BlueprintPickupHeldItemBehaviour : EntityHeldItemBehaviour, IHeldTwinkleEntityBehaviour
    {
        public BlueprintPickupHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
        }

        public override HeldTargetFlag GetHeldTargetMask(LevelEngine level)
        {
            return HeldTargetFlag.Pickup;
        }
        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            if (target is HeldItemTargetGrid)
                return true;
            if (target is HeldItemTargetLawn)
                return true;
            if (target is HeldItemTargetBlueprint)
                return true;
            var entity = GetEntity(target.GetLevel(), data);
            if (target is HeldItemTargetEntity ent && ent.Target == entity)
            {
                if (pointer.pointer.type == PointerTypes.TOUCH && !IgnoresTouchRaycast(entity))
                {
                    return true;
                }
            }
            return false;
        }

        public override HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            if (target is not HeldItemTargetGrid gridTarget)
                return HeldHighlight.None;

            var grid = gridTarget.Target;

            var level = grid.Level;
            var entity = GetEntity(level, data);
            var seedDef = GetSeedDefinition(entity);
            return grid.GetSeedHeldHighlight(seedDef);
        }

        public override void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidClickButton())
                return;
            OnHeldItemMainPointerEvent(target, data, pointerParams);
        }
        private void OnHeldItemMainPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            switch (target)
            {
                case HeldItemTargetGrid gridTarget:
                    OnHeldItemPointerEventGrid(gridTarget, data, pointerParams);
                    break;
                case HeldItemTargetEntity entityTarget:
                    OnHeldItemPointerEventEntity(entityTarget, data, pointerParams);
                    break;
                case HeldItemTargetLawn lawnTarget:
                    OnHeldItemPointerEventLawn(lawnTarget, data, pointerParams);
                    break;
                case HeldItemTargetBlueprint blueprintTarget:
                    OnHeldItemPointerEventBlueprint(blueprintTarget, data, pointerParams);
                    break;
            }
        }
        private void OnHeldItemPointerEventGrid(HeldItemTargetGrid target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidReleaseAction())
                return;
            var entity = GetEntity(target.GetLevel(), data);
            var seedDef = GetSeedDefinition(entity);
            if (seedDef != null)
            {
                var grid = target.Target;
                var level = grid.Level;
                if (grid.CanPlaceBlueprint(seedDef.GetID(), out var error))
                {
                    if (seedDef.GetSeedType() == SeedTypes.ENTITY)
                    {
                        var commandBlock = IsCommandBlock(entity);
                        entity.Remove();
                        grid.UseEntityBlueprintDefinition(seedDef, data, commandBlock);
                        level.ResetHeldItem();
                        return;
                    }
                }
                else
                {
                    var message = Global.Game.GetGridErrorMessage(error);
                    if (!string.IsNullOrEmpty(message))
                    {
                        level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, message, 0, 150);
                    }
                }
            }
            SetIgnoreTouchRaycast(entity, false);
        }
        private void OnHeldItemPointerEventEntity(HeldItemTargetEntity target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            var targetEntity = target.Target;
            var entity = GetEntity(target.GetLevel(), data);
            if (targetEntity != entity)
                return;
            if (pointerParams.pointer.type != PointerTypes.TOUCH)
                return;
            switch (pointerParams.interaction)
            {
                case PointerInteraction.Down:
                    {
                        var level = target.GetLevel();
                        if (level.CancelHeldItem())
                        {
                            level.PlaySound(VanillaSoundID.tap);
                        }
                    }
                    break;
                case PointerInteraction.BeginDrag:
                    {
                        SetIgnoreTouchRaycast(entity, true);
                    }
                    break;
            }
        }
        private void OnHeldItemPointerEventLawn(HeldItemTargetLawn target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidReleaseAction())
                return;
            var entity = GetEntity(target.GetLevel(), data);
            SetIgnoreTouchRaycast(entity, false);
            var level = target.Level;
            if (level.CancelHeldItem())
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        private void OnHeldItemPointerEventBlueprint(HeldItemTargetBlueprint target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidReleaseAction())
                return;
            var entity = GetEntity(target.GetLevel(), data);
            SetIgnoreTouchRaycast(entity, false);
            var level = target.Level;
            if (level.CancelHeldItem())
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        public override void GetModelID(LevelEngine level, IHeldItemData data, CallbackResult result)
        {
            var entity = GetEntity(level, data);
            var seedDef = GetSeedDefinition(entity);
            if (seedDef == null)
                return;
            result.SetFinalValue(seedDef.GetModelID());
        }
        public override void OnSetModel(LevelEngine level, IHeldItemData data, IModelInterface model)
        {
            var entity = GetEntity(level, data);
            if (entity == null)
                return;
            if (IsCommandBlock(entity))
            {
                model.SetShaderInt("_Grayscale", 1);
            }
        }
        public override void OnUpdate(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data);
            if (entity == null || !entity.Exists())
            {
                level.ResetHeldItem();
            }
        }
        public static bool IsCommandBlock(Entity entity) => BlueprintPickup.IsCommandBlock(entity);
        public static SeedDefinition GetSeedDefinition(Entity entity) => BlueprintPickup.GetSeedDefinition(entity);
        public static bool IgnoresTouchRaycast(Entity entity) => BlueprintPickup.IgnoresTouchRaycast(entity);
        public static void SetIgnoreTouchRaycast(Entity entity, bool value) => BlueprintPickup.SetIgnoreTouchRaycast(entity, value);

        public bool ShouldMakeEntityTwinkle(Entity entity, IHeldItemData data)
        {
            var level = entity.Level;
            var blueprintPickup = GetEntity(level, data);
            var seedEntityID = BlueprintPickup.GetSeedEntityID(blueprintPickup);
            var entityDef = level.Content.GetEntityDefinition(seedEntityID);

            return entityDef != null && entityDef.IsUpgradeBlueprint() && entity.CanUpgradeToContraption(entityDef);
        }
    }
}
