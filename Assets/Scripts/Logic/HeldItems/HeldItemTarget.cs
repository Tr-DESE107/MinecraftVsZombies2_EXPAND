using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2Logic.HeldItems
{
    public abstract class HeldItemTarget
    {
    }
    public class HeldItemTargetEntity : HeldItemTarget
    {
        public HeldItemTargetEntity(Entity target)
        {
            Target = target;
        }
        public Entity Target { get; }
    }
    public class HeldItemTargetGrid : HeldItemTarget
    {
        public HeldItemTargetGrid(LawnGrid target, Vector2 pointerPosition)
        {
            Target = target;
            PointerPosition = pointerPosition;
        }
        public LawnGrid Target { get; }
        public Vector2 PointerPosition { get; }
    }
    public class HeldItemTargetLawn : HeldItemTarget
    {
        public HeldItemTargetLawn(LevelEngine level, LawnArea area)
        {
            Level = level;
            Area = area;
        }
        public LevelEngine Level { get; }
        public LawnArea Area { get; }
    }
}
