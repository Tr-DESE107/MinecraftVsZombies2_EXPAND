﻿using System.Linq;
using PVZEngine;
using PVZEngine.Base;
using Tools;
using UnityEngine;

namespace MVZ2Logic.IZombie
{
    public abstract class IZombieLayoutDefinition : Definition
    {
        public IZombieLayoutDefinition(string nsp, string name, int columns) : base(nsp, name)
        {
            Columns = columns;
        }
        public abstract void Fill(IIZombieMap map, RandomGenerator rng);
        public void RandomFill(IIZombieMap map, NamespaceID entityID, RandomGenerator rng)
        {
            var allGrids = map.GetAllGridPositions();
            var grids = allGrids.Where(g => map.CanInsert(g, entityID)).ToArray();
            RandomFill(map, grids, entityID, grids.Count(), rng);
        }
        public void RandomFill(IIZombieMap map, NamespaceID entityID, int count, RandomGenerator rng)
        {
            var allGrids = map.GetAllGridPositions();
            var grids = allGrids.Where(g => map.CanInsert(g, entityID)).ToArray();
            RandomFill(map, grids, entityID, count, rng);
        }
        public void RandomFillAtLane(IIZombieMap map, int lane, NamespaceID entityID, int count, RandomGenerator rng)
        {
            var allGrids = map.GetAllGridPositions();
            var grids = allGrids.Where(g => g.y == lane && map.CanInsert(g, entityID)).ToArray();
            RandomFill(map, grids, entityID, count, rng);
        }
        public void RandomFillAtColumn(IIZombieMap map, int column, NamespaceID entityID, int count, RandomGenerator rng)
        {
            var allGrids = map.GetAllGridPositions();
            var grids = allGrids.Where(g => g.x == column && map.CanInsert(g, entityID)).ToArray();
            RandomFill(map, grids, entityID, count, rng);
        }
        public void RandomFill(IIZombieMap map, Vector2Int[] grids, NamespaceID entityID, int count, RandomGenerator rng)
        {
            if (grids.Length <= 0)
                return;
            var validGrids = grids.RandomTake(count, rng);
            foreach (var grid in validGrids)
            {
                Insert(map, grid, entityID);
            }
        }
        public void FillColumn(IIZombieMap map, int column, NamespaceID entityID)
        {
            var allGrids = map.GetAllGridPositions();
            var grids = allGrids.Where(g => g.x == column && map.CanInsert(g, entityID)).ToArray();
            foreach (var grid in grids)
            {
                Insert(map, grid, entityID);
            }
        }
        public void Insert(IIZombieMap map, int x, int y, NamespaceID entityID)
        {
            map.InsertEntity(x, y, entityID);
        }
        public void Insert(IIZombieMap map, Vector2Int position, NamespaceID entityID)
        {
            map.InsertEntity(position.x, position.y, entityID);
        }
        public int Columns { get; }
        public NamespaceID[] Blueprints { get; protected set; }
        public sealed override string GetDefinitionType() => LogicDefinitionTypes.I_ZOMBIE_LAYOUT;
    }
}
