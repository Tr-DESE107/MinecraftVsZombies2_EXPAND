using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class MVZ2Level
    {
        public static bool HasBehaviour<T>(this LevelEngine level) where T : StageBehaviour
        {
            return level.StageDefinition.HasBehaviour<T>();
        }
    }
}
