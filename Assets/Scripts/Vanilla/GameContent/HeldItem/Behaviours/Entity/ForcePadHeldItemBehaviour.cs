﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.forcePad)]
    public class ForcePadHeldItemBehaviour : EntityHeldItemBehaviour
    {
        public ForcePadHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
        }

        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return target is HeldItemTargetGrid targetGrid;
        }
        public override HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return HeldHighlight.Green();
        }
        public override void GetModelID(LevelEngine level, IHeldItemData data, CallbackResult result)
        {
            result.SetFinalValue(VanillaModelID.targetHeldItem);
        }
        public override void OnUpdate(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data);
            if (entity == null || !entity.Exists() || entity.IsAIFrozen())
            {
                level.ResetHeldItem();
                return;
            }
        }
        public override void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.IsInvalidClickButton() || pointerParams.IsInvalidReleaseAction())
                return;
            if (target is not HeldItemTargetGrid targetGrid)
                return;
            if (targetGrid.Target == null)
                return;
            var level = target.GetLevel();
            var entity = GetEntity(level, data);
            ForcePad.SetDragTargetLocked(entity, true);
            ForcePad.SetDragTarget(entity, targetGrid.Target.GetEntityPosition());
            ForcePad.SetDragTimeout(entity, 30);
            level.ResetHeldItem();
            entity.PlaySound(VanillaSoundID.magnetic);
        }
    }
}
