using MVZ2Logic.Callbacks;
using MVZ2Logic.Triggers;
using PVZEngine;
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
    }
}
