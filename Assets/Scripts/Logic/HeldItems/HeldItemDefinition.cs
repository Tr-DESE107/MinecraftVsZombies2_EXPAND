using System.Collections.Generic;
using MVZ2.HeldItems;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

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
        public virtual void Update(LevelEngine level, IHeldItemData data) { }
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
                    if (highlight != HeldHighlight.None)
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
        public virtual float GetRadius(LevelEngine level, IHeldItemData data) => 0;
        public virtual SeedPack GetSeedPack(LevelEngine level, IHeldItemData data) => null;
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
