using System.Collections.Generic;
using MVZ2.Level.Components;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Extensions
{
    public static partial class MVZ2Level
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
    }
}
