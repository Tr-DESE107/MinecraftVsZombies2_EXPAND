using MVZ2.Level.Components;
using PVZEngine.Level;

namespace MVZ2.Extensions
{
    public static partial class MVZ2Level
    {
        public static bool HasBehaviour<T>(this LevelEngine level) where T : StageBehaviour
        {
            return level.StageDefinition.HasBehaviour<T>();
        }
    }
}
