using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2Logic.HeldItems
{
    public abstract class HeldItemDefinition : Definition
    {
        public HeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public abstract HeldFlags GetHeldFlagsOnEntity(Entity entity, long id);
        public abstract HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, long id);
        public virtual void HoverOnEntity(Entity entity, long id) { }
        public virtual bool UseOnEntity(Entity entity, long id) { return false; }
        public virtual string GetHeldErrorMessageOnGrid(LawnGrid grid, long id) => null;
        public virtual bool UseOnGrid(LawnGrid grid, long id) { return false; }
        public virtual void UseOnLawn(LevelEngine level, LawnArea area, long id) { }
        public virtual bool IsForEntity() => false;
        public virtual bool IsForGrid() => false;
        public virtual bool IsForPickup() => false;
        public virtual NamespaceID GetModelID(LevelEngine level, long id) => null;
    }
    public enum LawnArea
    {
        Side,
        Main,
        Bottom
    }
}
