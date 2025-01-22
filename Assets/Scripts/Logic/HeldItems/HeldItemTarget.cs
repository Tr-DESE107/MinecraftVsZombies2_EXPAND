using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2Logic.HeldItems
{
    public abstract class HeldItemTarget
    {
        public abstract LevelEngine GetLevel();
    }
    public class HeldItemTargetEntity : HeldItemTarget
    {
        public HeldItemTargetEntity(Entity target, Vector2 pointerPosition)
        {
            Target = target;
            PointerPosition = pointerPosition;
        }
        public override LevelEngine GetLevel() => Target.Level;
        public Entity Target { get; }
        public Vector2 PointerPosition { get; }
    }
    public class HeldItemTargetGrid : HeldItemTarget
    {
        public HeldItemTargetGrid(LawnGrid target, Vector2 pointerPosition)
        {
            Target = target;
            PointerPosition = pointerPosition;
        }
        public override LevelEngine GetLevel() => Target.Level;
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
        public override LevelEngine GetLevel() => Level;
        public LevelEngine Level { get; }
        public LawnArea Area { get; }
    }
}
