﻿using MVZ2Logic.Callbacks;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        public static bool IsBlueprintNotRecommmended(this LevelEngine level, NamespaceID blueprint)
        {
            var result = new CallbackResult(false);
            var param = new LogicLevelCallbacks.GetBlueprintNotRecommondedParams()
            {
                level = level,
                blueprintID = blueprint
            };
            level.Triggers.RunCallbackWithResultFiltered(LogicLevelCallbacks.GET_BLUEPRINT_NOT_RECOMMONDED, param, result, blueprint);
            return result.GetValue<bool>();
        }
        public static void SetBehaviourField<T>(this LevelEngine level, NamespaceID id, PropertyKey<T> name, T value)
        {
            level.SetProperty(name, value);
        }
        public static T GetBehaviourField<T>(this LevelEngine level, NamespaceID id, PropertyKey<T> name)
        {
            return level.GetProperty<T>(name);
        }
        public static void SetBehaviourField<T>(this LevelEngine level, PropertyKey<T> name, T value)
        {
            level.SetProperty(name, value);
        }
        public static T GetBehaviourField<T>(this LevelEngine level, PropertyKey<T> name)
        {
            return level.GetProperty<T>(name);
        }
    }
}
