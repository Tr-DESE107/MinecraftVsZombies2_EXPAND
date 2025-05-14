using System.Linq;
using PVZEngine;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public abstract class IZombieLayout
    {
        public IZombieLayout(int columns)
        {
            Columns = columns;
        }
        public abstract void Fill(IZombieMap map, RandomGenerator rng);
        public void RandomFill(IZombieMap map, NamespaceID entityID, RandomGenerator rng)
        {
            var allGrids = map.GetAllGridPositions();
            var grids = allGrids.Where(g => map.CanInsert(g, entityID)).ToArray();
            RandomFill(map, grids, entityID, grids.Count(), rng);
        }
        public void RandomFill(IZombieMap map, NamespaceID entityID, int count, RandomGenerator rng)
        {
            var allGrids = map.GetAllGridPositions();
            var grids = allGrids.Where(g => map.CanInsert(g, entityID)).ToArray();
            RandomFill(map, grids, entityID, count, rng);
        }
        public void RandomFill(IZombieMap map, Vector2Int[] grids, NamespaceID entityID, int count, RandomGenerator rng)
        {
            var validGrids = grids.RandomTake(count, rng);
            foreach (var grid in validGrids)
            {
                map.InsertEntity(grid.x, grid.y, entityID);
            }
        }
        public int Columns { get; }
    }
}
