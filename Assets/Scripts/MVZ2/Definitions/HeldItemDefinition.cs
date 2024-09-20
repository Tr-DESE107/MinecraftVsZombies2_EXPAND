using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public abstract class HeldItemDefinition : Definition
    {
        public HeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public abstract bool IsValidOnEntity(Entity entity, long id);
        public virtual bool UseOnEntity(Entity entity, long id) { return false; }
        public virtual void HoverOnEntity(Entity entity, long id) { }
        public abstract bool IsValidOnGrid(LawnGrid grid, long id);
        public virtual bool UseOnGrid(LawnGrid grid, long id) { return false; }
        public virtual void UseOnLawn(LevelEngine level, LawnArea area, long id) { }
    }
    public enum LawnArea
    {
        Side,
        Main,
        Bottom
    }
}
