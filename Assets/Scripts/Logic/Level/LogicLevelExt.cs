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
            foreach (var trigger in level.Triggers.GetTriggers(LogicLevelCallbacks.GET_BLUEPRINT_NOT_RECOMMONDED))
            {
                if (!trigger.Filter(blueprint))
                    continue;
                trigger.Run(level, blueprint, result);
            }
            return result.Result;
        }
        public static void SetBehaviourField(this LevelEngine level, NamespaceID id, string name, object value)
        {
            level.SetField(id.ToString(), name, value);
        }
        public static T GetBehaviourField<T>(this LevelEngine level, NamespaceID id, string name)
        {
            return level.GetField<T>(id.ToString(), name);
        }
    }
}
