using MVZ2Logic.Callbacks;
using MVZ2Logic.Triggers;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Triggers;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        public static bool IsBlueprintNotRecommmended(this LevelEngine level, NamespaceID blueprint)
        {
            var result = new TriggerResultBoolean();
            level.Triggers.RunCallbackFiltered(LogicLevelCallbacks.GET_BLUEPRINT_NOT_RECOMMONDED, blueprint, result, c => c(level, blueprint, result));
            return result.Result;
        }
        public static void SetBehaviourField(this LevelEngine level, NamespaceID id, PropertyKey name, object value)
        {
            level.SetProperty(name, value);
        }
        public static T GetBehaviourField<T>(this LevelEngine level, NamespaceID id, PropertyKey name)
        {
            return level.GetProperty<T>(name);
        }
        public static void SetBehaviourField(this LevelEngine level, PropertyKey name, object value)
        {
            level.SetProperty(name, value);
        }
        public static T GetBehaviourField<T>(this LevelEngine level, PropertyKey name)
        {
            return level.GetProperty<T>(name);
        }
    }
}
