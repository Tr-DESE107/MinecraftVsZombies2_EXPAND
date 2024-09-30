using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    public abstract class StageBehaviour
    {
        public StageBehaviour(StageDefinition stageDef)
        {
        }
        public virtual void Start(LevelEngine level) { }
        public virtual void Update(LevelEngine level) { }
        public virtual void PrepareForBattle(LevelEngine level) { }
        public virtual void PostWave(LevelEngine level, int wave) { }
        public virtual void PostHugeWave(LevelEngine level) { }
        public virtual void PostFinalWave(LevelEngine level) { }
        public virtual void PostEnemySpawned(Entity entity) { }
        public bool LevelHasBehaviour<T>(LevelEngine level) where T : StageBehaviour
        {
            return level.StageDefinition.HasBehaviour<T>();
        }
    }
}
