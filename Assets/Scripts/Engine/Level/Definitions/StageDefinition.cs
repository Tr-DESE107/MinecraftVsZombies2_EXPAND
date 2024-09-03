using System.Collections.Generic;
using PVZEngine.LevelManaging;

namespace PVZEngine.Definitions
{
    public abstract class StageDefinition : Definition
    {
        public StageDefinition(string nsp, string name) : base(nsp, name)
        {
            SetProperty(StageProperties.FIRST_WAVE_TIME, 540);
            SetProperty(StageProperties.WAVES_PER_FLAG, 10);
        }
        public virtual void Start(Level level) { }
        public virtual void Update(Level level) { }
        public virtual void PrepareForBattle(Level level) { }
        public virtual void PostWave(Level level, int wave) { }
        public virtual void PostHugeWave(Level level) { }
        public virtual void PostFinalWave(Level level) { }
        public virtual void PostEnemySpawned(Entity entity) { }
        public abstract IEnumerable<IEnemySpawnEntry> GetEnemyPool();
    }
}
