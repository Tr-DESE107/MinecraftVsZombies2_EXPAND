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
        public abstract HeldFlags GetHeldFlagsOnEntity(Entity entity, IHeldItemData data);
        public abstract HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, IHeldItemData data);
        public virtual bool FilterEntityPointerPhase(Entity entity, PointerPhase phase) => true;
        public virtual bool FilterGridPointerPhase(PointerPhase phase) => true;
        public virtual bool FilterLawnPointerPhase(PointerPhase phase) => true;
        public virtual bool UseOnEntity(Entity entity, IHeldItemData data) { return false; }
        public virtual bool UseOnGrid(LawnGrid grid, IHeldItemData data, NamespaceID targetLayer) { return false; }
        public virtual void UseOnLawn(LevelEngine level, LawnArea area, IHeldItemData data) { }
        public virtual bool IsForEntity() => false;
        public virtual bool IsForGrid() => false;
        public virtual bool IsForPickup() => false;
        public virtual NamespaceID GetModelID(LevelEngine level, long id) => null;
        public virtual float GetRadius() => 0;
        public virtual SeedPack GetSeedPack(LevelEngine level, IHeldItemData data) => null;
        public virtual void Update(LevelEngine level) { }
    }
    public enum LawnArea
    {
        Side,
        Main,
        Bottom
    }
}
