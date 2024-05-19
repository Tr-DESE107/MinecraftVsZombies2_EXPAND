using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public partial class Game
    {
        public int CurrentWave { get; set; }
        public int TotalFlags { get; set; }
        public int CurrentFlag { get; set; }
        public LevelBehaviour Behaviour { get; private set; }
        public int GetFinalWaveIndex()
        {
            return TotalFlags * WAVES_PER_FLAG;
        }
        public void RunFinalWaveEvent()
        {
        }
        public void RunHugeWaveEvent()
        {
        }
        public EnemySpawnPool getEnemyPool()
        {
            return GetProperty<EnemySpawnPool>(GameProperties.ENEMY_SPAWN_POOL);
        }
        public bool WillEnemySpawn(NamespaceID entityRef)
        {
            var pool = getEnemyPool();
            if (pool == null)
                return false;
            return pool.entries.Any(e => e.entityRef == entityRef);
        }
        public const int WAVES_PER_FLAG = 10;
    }
    public class EnemySpawnPool
    {
        public List<EnemySpawnEntry> entries = new List<EnemySpawnEntry>();
    }
    public class EnemySpawnEntry
    {
        public NamespaceID entityRef;
        public int earliestFlag;
    }
}