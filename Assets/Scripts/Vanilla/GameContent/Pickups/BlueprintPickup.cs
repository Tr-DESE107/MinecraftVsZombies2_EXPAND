using MVZ2.GameContent.HeldItems;
using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.SeedPacks;
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
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.blueprintPickup)]
    public class BlueprintPickup : PickupBehaviour, IHeldEntityPointerEventHandler, IHeldEntitySetModelHandler, IHeldEntityUpdateHandler, IHeldEntityGetModelIDHandler
    {
        public BlueprintPickup(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            UpdateModel(entity);
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            UpdateModel(pickup);
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            entity.Velocity = Vector3.zero;
        }
        public override void PostCollect(Entity pickup)
        {
            var level = pickup.Level;
            level.SetHeldItem(VanillaHeldTypes.entity, pickup.ID, 0);
            pickup.PlaySound(pickup.GetCollectSound());
        }
        private static SeedDefinition GetSeedDefinition(Entity pickup)
        {
            var seedID = GetBlueprintID(pickup);
            var seedDef = pickup.Level.Content.GetSeedDefinition(seedID);
            if (seedDef == null)
                return null;
            return seedDef;
        }
        private static void UpdateModel(Entity pickup)
        {
            bool isHolding = pickup.Level.IsHoldingEntity(pickup);
            pickup.SetModelProperty("BlueprintID", GetBlueprintID(pickup));
            pickup.SetModelProperty("CommandBlock", IsCommandBlock(pickup));
            pickup.SetAnimationBool("HideEnergy", true);
            pickup.SetAnimationBool("Dark", pickup.Timeout < 100 && pickup.Timeout % 20 < 10 && !isHolding);
            pickup.SetAnimationBool("Selected", isHolding);
        }
        #region 接口实现
        bool IHeldEntityBehaviour.IsHeldItemValidFor(Entity entity, IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            if (target is HeldItemTargetGrid)
                return true;
            if (target is HeldItemTargetLawn)
                return true;
            if (target is HeldItemTargetBlueprint)
                return true;
            if (target is HeldItemTargetEntity ent && ent.Target == entity)
            {
                if (pointer.pointer.type == PointerTypes.TOUCH && !IgnoresTouchRaycast(entity))
                {
                    return true;
                }
            }
            return false;
        }

        HeldHighlight IHeldEntityBehaviour.GetHeldItemHighlight(Entity entity, IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            if (target is not HeldItemTargetGrid gridTarget)
                return HeldHighlight.None;

            var grid = gridTarget.Target;

            var level = grid.Level;
            var seedDef = GetSeedDefinition(entity);
            return grid.GetSeedHeldHighlight(seedDef);
        }

        void IHeldEntityPointerEventHandler.OnHeldItemPointerEvent(Entity entity, IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            Debug.Log($"Target: {target}, Pointer: {pointerParams}");
            var pointer = pointerParams.pointer;
            if (pointer.type == PointerTypes.MOUSE && pointer.button == MouseButtons.RIGHT)
            {
                OnRightMouseEvent(entity, target, data, pointerParams);
                return;
            }
            if (pointerParams.IsInvalidClickButton())
                return;
            OnHeldItemMainPointerEvent(entity, target, data, pointerParams);
        }
        private void OnRightMouseEvent(Entity entity, IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            var level = target.GetLevel();
            if (level.CancelHeldItem())
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        private void OnHeldItemMainPointerEvent(Entity entity, IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            switch (target)
            {
                case HeldItemTargetGrid gridTarget:
                    OnHeldItemPointerEventGrid(entity, gridTarget, data, pointerParams);
                    break;
                case HeldItemTargetEntity entityTarget:
                    OnHeldItemPointerEventEntity(entity, entityTarget, data, pointerParams);
                    break;
                case HeldItemTargetLawn lawnTarget:
                    OnHeldItemPointerEventLawn(entity, lawnTarget, data, pointerParams);
                    break;
                case HeldItemTargetBlueprint blueprintTarget:
                    OnHeldItemPointerEventBlueprint(entity, blueprintTarget, data, pointerParams);
                    break;
            }
        }
        private void OnHeldItemPointerEventGrid(Entity entity, HeldItemTargetGrid target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidReleaseAction())
                return;
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
        private void OnHeldItemPointerEventEntity(Entity entity, HeldItemTargetEntity target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            var targetEntity = target.Target;
            if (targetEntity != entity)
                return;
            if (pointerParams.pointer.type != PointerTypes.TOUCH)
                return;
            switch (pointerParams.interaction)
            {
                case PointerInteraction.Down:
                    {
                        var level = target.GetLevel();
                        if (level.IsHoldingEntity(entity))
                        {
                            if (level.CancelHeldItem())
                            {
                                level.PlaySound(VanillaSoundID.tap);
                            }
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
        private void OnHeldItemPointerEventLawn(Entity entity, HeldItemTargetLawn target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidReleaseAction())
                return;
            SetIgnoreTouchRaycast(entity, false);
            var level = target.Level;
            if (level.CancelHeldItem())
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        private void OnHeldItemPointerEventBlueprint(Entity entity, HeldItemTargetBlueprint target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidReleaseAction())
                return;
            SetIgnoreTouchRaycast(entity, false);
            var level = target.Level;
            if (level.CancelHeldItem())
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        void IHeldEntityGetModelIDHandler.GetHeldItemModelID(Entity entity, LevelEngine level, IHeldItemData data, CallbackResult result)
        {
            var seedDef = GetSeedDefinition(entity);
            if (seedDef == null)
                return;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                var entityDef = level.Content.GetEntityDefinition(entityID);
                result.SetFinalValue(entityDef.GetModelID());
            }
        }
        void IHeldEntitySetModelHandler.OnHeldItemSetModel(Entity entity, LevelEngine level, IHeldItemData data, IModelInterface model)
        {
            if (entity == null)
                return;
            if (IsCommandBlock(entity))
            {
                model.SetShaderInt("_Grayscale", 1);
            }
        }
        void IHeldEntityUpdateHandler.OnHeldItemUpdate(Entity entity, LevelEngine level, IHeldItemData data)
        {
            if (entity == null || !entity.Exists())
            {
                level.ResetHeldItem();
            }
        }
        #endregion
        public static NamespaceID GetBlueprintID(Entity pickup) => pickup.GetBehaviourField<NamespaceID>(PROP_BLUEPRINT_ID);
        public static void SetBlueprintID(Entity pickup, NamespaceID value) => pickup.SetBehaviourField(PROP_BLUEPRINT_ID, value);
        public static bool IsCommandBlock(Entity pickup) => pickup.GetBehaviourField<bool>(PROP_COMMAND_BLOCK);
        public static void SetCommandBlock(Entity pickup, bool value) => pickup.SetBehaviourField(PROP_COMMAND_BLOCK, value);
        public static bool IgnoresTouchRaycast(Entity pickup) => pickup.GetBehaviourField<bool>(PROP_IGNORE_TOUCH_RAYCAST);
        public static void SetIgnoreTouchRaycast(Entity pickup, bool value) => pickup.SetBehaviourField(PROP_IGNORE_TOUCH_RAYCAST, value);
        public const int PICK_THRESOLD = 5;
        public static readonly VanillaEntityPropertyMeta<NamespaceID> PROP_BLUEPRINT_ID = new VanillaEntityPropertyMeta<NamespaceID>("BlueprintID");
        public static readonly VanillaEntityPropertyMeta<bool> PROP_COMMAND_BLOCK = new VanillaEntityPropertyMeta<bool>("CommandBlock");
        public static readonly VanillaEntityPropertyMeta<bool> PROP_IGNORE_TOUCH_RAYCAST = new VanillaEntityPropertyMeta<bool>("IgnoreRaycast");
    }
}