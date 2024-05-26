using System.Collections.Generic;

namespace PVZEngine
{
    public abstract class StageDefinition : Definition
    {
        public StageDefinition(string nsp, string name) : base(nsp, name)
        {
            SetProperty(StageProperties.FIRST_WAVE_TIME, 540);
            SetProperty(StageProperties.WAVES_PER_FLAG, 10);
        }
        public virtual void Start(Game level) { }
        public virtual void Update(Game level) { }
        public virtual void PostWave(Game level, int wave) { }
        public virtual void PostHugeWave(Game level) { }
        public virtual void PostFinalWave(Game level) { }
        public virtual void PostEnemySpawned(Entity entity) { }
        public abstract IEnumerable<IEnemySpawnEntry> GetEnemyPool();
    }
}
