using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public abstract class SpawnDefinition : Definition
    {
        public SpawnDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        #region 预览
        public NamespaceID GetPreviewEntityID() => GetPreviewBehaviour().GetPreviewEntityID();
        public NamespaceID[] GetCounterTags(LevelEngine level) => GetPreviewBehaviour().GetCounterTags(level);
        protected abstract ISpawnPreviewBehaviour GetPreviewBehaviour();
        #endregion

        #region 生成
        public void PreSpawnAtWave(LevelEngine level, int wave, ref float totalPoints) => GetInLevelBehaviour().PreSpawnAtWave(level, wave, ref totalPoints);
        public int GetWeight(LevelEngine level) => GetInLevelBehaviour().GetWeight(level);
        public int GetSpawnLevel(LevelEngine level) => GetInLevelBehaviour().GetSpawnLevel(level);
        public bool CanSpawnInLevel(LevelEngine level) => GetInLevelBehaviour().CanSpawnInLevel(level);
        public int GetRandomSpawnLane(LevelEngine level) => GetInLevelBehaviour().GetRandomSpawnLane(level);
        public NamespaceID GetSpawnEntityID() => GetInLevelBehaviour().GetSpawnEntityID();
        protected abstract ISpawnInLevelBehaviour GetInLevelBehaviour();
        #endregion

        #region 无尽
        public bool CanAppearInEndless(LevelEngine level) => GetEndlessBehaviour().CanAppearInEndless(level);
        protected abstract ISpawnEndlessBehaviour GetEndlessBehaviour();
        #endregion

        public sealed override string GetDefinitionType() => EngineDefinitionTypes.SPAWN;
    }
    public interface ISpawnInLevelBehaviour
    {
        void PreSpawnAtWave(LevelEngine level, int wave, ref float totalPoints);
        NamespaceID GetSpawnEntityID();
        int GetSpawnLevel(LevelEngine level);
        int GetWeight(LevelEngine level);
        bool CanSpawnInLevel(LevelEngine level);
        int GetRandomSpawnLane(LevelEngine level);
    }
    public interface ISpawnPreviewBehaviour
    {
        NamespaceID GetPreviewEntityID();
        NamespaceID[] GetCounterTags(LevelEngine level);
    }
    public interface ISpawnEndlessBehaviour
    {
        bool CanAppearInEndless(LevelEngine level);
    }
}
