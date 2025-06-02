﻿using MVZ2.HeldItems;
using PVZEngine.Base;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2Logic.HeldItems
{
    public abstract class HeldItemBehaviourDefinition : Definition
    {
        public HeldItemBehaviourDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return false;
        }
        public virtual HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return HeldHighlight.None;
        }
        public virtual void OnBegin(LevelEngine level, IHeldItemData data)
        {
        }
        public virtual void OnEnd(LevelEngine level, IHeldItemData data)
        {
        }
        public virtual void OnUpdate(LevelEngine level, IHeldItemData data)
        {
        }
        public virtual void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
        }
        public virtual void OnSetModel(LevelEngine level, IHeldItemData data, IModelInterface model)
        {
        }
        public virtual void GetModelID(LevelEngine level, IHeldItemData data, CallbackResult result)
        {
        }
        public virtual void GetRadius(LevelEngine level, IHeldItemData data, CallbackResult result)
        {
        }
        public override string GetDefinitionType() => LogicDefinitionTypes.HELD_ITEM_BEHAVIOUR;
    }
}
