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
        public virtual bool CheckRaycast(HeldItemTarget target, IHeldItemData data) => false;
        public virtual HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data) => HeldHighlight.None;
        public virtual void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction interaction) { }
        public virtual NamespaceID GetModelID(LevelEngine level, IHeldItemData data) => null;
        public virtual float GetRadius(LevelEngine level, IHeldItemData data) => 0;
        public virtual SeedPack GetSeedPack(LevelEngine level, IHeldItemData data) => null;
        public virtual void Update(LevelEngine level, IHeldItemData data) { }
        public sealed override string GetDefinitionType() => LogicDefinitionTypes.HELD_ITEM;
    }
    public enum LawnArea
    {
        Side,
        Main,
        Bottom
    }
}
