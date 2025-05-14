using System.Collections.Generic;
using MVZ2.HeldItems;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.SeedPacks;
using static UnityEngine.GraphicsBuffer;

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
        public void Update(LevelEngine level, IHeldItemData data)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.Update(level, data);
            }
        }
        public bool IsValidFor(HeldItemTarget target, IHeldItemData data)
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour.IsValidFor(target, data))
                    return true;
            }
            return false;
        }
        public HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data)
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour.IsValidFor(target, data))
                {
                    var highlight = behaviour.GetHighlight(target, data);
                    if (highlight.mode != HeldHighlightMode.None)
                        return highlight;
                }
            }
            return HeldHighlight.None;
        }
        public void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour.IsValidFor(target, data))
                {
                    behaviour.Use(target, data, interaction);
                }
            }
        }

        public virtual NamespaceID GetModelID(LevelEngine level, IHeldItemData data) => null;
        public virtual void PostSetModel(LevelEngine level, IHeldItemData data, IModelInterface model) { }
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
