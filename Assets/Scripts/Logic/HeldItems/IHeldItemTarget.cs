﻿using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2Logic.HeldItems
{
    public interface IHeldItemTarget
    {
        LevelEngine GetLevel();
    }
    public struct HeldItemTargetEntity : IHeldItemTarget
    {
        public HeldItemTargetEntity(Entity target, Vector2 pointerPosition)
        {
            Target = target;
            PointerPosition = pointerPosition;
        }
        public LevelEngine GetLevel() => Target.Level;
        public Entity Target { get; }
        public Vector2 PointerPosition { get; }
    }
    public struct HeldItemTargetGrid : IHeldItemTarget
    {
        public HeldItemTargetGrid(LawnGrid target, Vector2 pointerPosition)
        {
            Target = target;
            PointerPosition = pointerPosition;
        }
        public LevelEngine GetLevel() => Target.Level;
        public LawnGrid Target { get; }
        public Vector2 PointerPosition { get; }
    }
    public struct HeldItemTargetLawn : IHeldItemTarget
    {
        public HeldItemTargetLawn(LevelEngine level, LawnArea area)
        {
            Level = level;
            Area = area;
        }
        public LevelEngine GetLevel() => Level;
        public LevelEngine Level { get; }
        public LawnArea Area { get; }
    }
    public struct HeldItemTargetBlueprint : IHeldItemTarget
    {
        public HeldItemTargetBlueprint(LevelEngine level, int index, bool conveyor)
        {
            Level = level;
            Index = index;
            IsConveyor = conveyor;
        }
        public SeedPack GetSeedPack()
        {
            if (IsConveyor)
            {
                return Level.GetConveyorSeedPackAt(Index);
            }
            return Level.GetSeedPackAt(Index);
        }
        public LevelEngine GetLevel() => Level;
        public LevelEngine Level { get; }
        public int Index { get; }
        public bool IsConveyor { get; }
    }
}
