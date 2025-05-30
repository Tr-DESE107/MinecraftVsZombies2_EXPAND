using System.Collections.Generic;
using MVZ2.HeldItems;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2Logic.HeldItems
{
    public abstract class HeldItemDefinition : Definition
    {
        public HeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        protected void AddBehaviour(HeldItemBehaviour behaviour)
        {
            behaviours.Add(behaviour);
        }
        public virtual void Begin(LevelEngine level, IHeldItemData data)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.OnBegin(level, data);
            }
        }
        public virtual void End(LevelEngine level, IHeldItemData data)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.OnEnd(level, data);
            }
        }
        public virtual void Update(LevelEngine level, IHeldItemData data)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.OnUpdate(level, data);
            }
        }
        public virtual bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerData pointer)
        {
            var interactionData = new PointerInteractionData()
            {
                pointer = pointer,
                interaction = PointerInteraction.Hover
            };
            foreach (var behaviour in behaviours)
            {
                if (behaviour.IsValidFor(target, data, interactionData))
                    return true;
            }
            return false;
        }
        public virtual HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerData pointer)
        {
            var interactionData = new PointerInteractionData()
            {
                pointer = pointer,
                interaction = PointerInteraction.Hover
            };
            foreach (var behaviour in behaviours)
            {
                if (behaviour.IsValidFor(target, data, interactionData))
                {
                    var highlight = behaviour.GetHighlight(target, data, interactionData);
                    if (highlight.mode != HeldHighlightMode.None)
                        return highlight;
                }
            }
            return HeldHighlight.None;
        }
        public virtual void DoPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour.IsValidFor(target, data, pointerParams))
                {
                    behaviour.OnPointerEvent(target, data, pointerParams);
                }
            }
        }
        public virtual void PostSetModel(LevelEngine level, IHeldItemData data, IModelInterface model)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.OnSetModel(level, data, model);
            }
        }
        public virtual NamespaceID GetModelID(LevelEngine level, IHeldItemData data) => null;
        public virtual float GetRadius(LevelEngine level, IHeldItemData data) => 0;
        public sealed override string GetDefinitionType() => LogicDefinitionTypes.HELD_ITEM;

        private List<HeldItemBehaviour> behaviours = new List<HeldItemBehaviour>();
    }
    public enum LawnArea
    {
        Side,
        Main,
        Bottom
    }
}
