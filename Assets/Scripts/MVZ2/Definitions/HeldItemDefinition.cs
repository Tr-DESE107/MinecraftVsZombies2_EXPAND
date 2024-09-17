using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public abstract class HeldItemDefinition : Definition
    {
        public HeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public abstract bool IsValidOnEntity(Entity entity, int id);
        public virtual bool UseOnEntity(Entity entity, int id) { return false; }
        public virtual void HoverOnEntity(Entity entity, int id) { }
        public abstract bool IsValidOnGrid(LawnGrid grid, int id);
        public virtual bool UseOnGrid(LawnGrid grid, int id) { return false; }
        public virtual void UseOnLawn(LevelEngine level, LawnArea area, int id) { }
    }
    public enum LawnArea
    {
        Side,
        Main,
        Bottom
    }
}
