using System.Collections.Generic;
using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public abstract class StageDefinition : Definition
    {
        public StageDefinition(string nsp, string name) : base(nsp, name)
        {
            SetProperty(StageProperties.FIRST_WAVE_TIME, 540);
            SetProperty(StageProperties.WAVES_PER_FLAG, 10);
        }
        public virtual void Start(LevelEngine level) { }
        public virtual void Update(LevelEngine level) { }
        public virtual void PrepareForBattle(LevelEngine level) { }
        public virtual void PostWave(LevelEngine level, int wave) { }
        public virtual void PostHugeWave(LevelEngine level) { }
        public virtual void PostFinalWave(LevelEngine level) { }
        public virtual void PostEnemySpawned(Entity entity) { }
        public virtual void AddCallbacks() { }
        public virtual void RemoveCallbacks() { }
        public virtual IEnumerable<IEnemySpawnEntry> GetEnemyPool()
        {
            yield break;
        }
    }
}
