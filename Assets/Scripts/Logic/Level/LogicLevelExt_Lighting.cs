using System.Collections.Generic;
using MVZ2Logic.Level.Components;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        public static ILightComponent GetLightComponent(this LevelEngine level)
        {
            return level.GetComponent<ILightComponent>();
        }
        public static bool IsIlluminated(this LevelEngine level, Entity entity)
        {
            var component = level.GetLightComponent();
            return component.IsIlluminated(entity);
        }
        public static long GetIlluminationLightSourceID(this LevelEngine level, Entity entity)
        {
            var component = level.GetLightComponent();
            return component.GetIlluminationLightSourceID(entity);
        }
        public static IEnumerable<long> GetIlluminationLightSources(this LevelEngine level, Entity entity)
        {
            var component = level.GetLightComponent();
            return component.GetAllIlluminationLightSources(entity);
        }
        public static void GetIlluminatiingEntities(this LevelEngine level, Entity entity, HashSet<long> results)
        {
            var component = level.GetLightComponent();
            component.GetIlluminatingEntities(entity, results);
        }
    }
}
