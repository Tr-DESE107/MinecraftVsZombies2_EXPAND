using System.Collections.Generic;
using UnityEngine;

namespace PVZEngine
{
    public abstract class LevelBehaviour
    {
        public Game Level { get; private set; }
        protected List<int> spawnedRows = new List<int>();

        public void OnCreated(Game level)
        {
            Level = level;
        }
        protected virtual Enemy SpawnEnemy(NamespaceID defRef, int row, bool hasShard = false, float x = 1080)
        {
            Entity entity = Level.Spawn(defRef, new Vector3(x, 0, Level.GetEntityLaneZ(row)), null);
            var enemy = entity as Enemy;
            enemy.HasStarshard = hasShard;
            return enemy;
        }

        protected int GetEnemyRandomRow()
        {
            var length = Level.GetMaxLaneCount();
            if (spawnedRows.Count >= Level.GetMaxLaneCount())
            {
                spawnedRows.Clear();
            }

            var pool = new List<int>();
            for (int i = 0; i < length; i++)
            {
                if (spawnedRows.Contains(i))
                    continue;
                pool.Add(i);
            }
            var index = Level.GetSpawnRandomRange(0, pool.Count);
            int row = pool[index];

            spawnedRows.Add(row);
            return row;
        }
    }
}