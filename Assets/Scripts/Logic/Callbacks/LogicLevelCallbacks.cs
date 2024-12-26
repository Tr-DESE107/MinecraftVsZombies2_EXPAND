using MVZ2Logic.Triggers;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Triggers;

namespace MVZ2Logic.Callbacks
{
    public static class LogicLevelCallbacks
    {
        public delegate void GetBlueprintNotRecommonded(LevelEngine level, NamespaceID blueprintID, TriggerResultBoolean result);

        public readonly static CallbackReference<GetBlueprintNotRecommonded> GET_BLUEPRINT_NOT_RECOMMONDED = new();
    }
}
