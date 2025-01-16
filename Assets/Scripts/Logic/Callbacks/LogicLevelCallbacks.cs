using System.Collections.Generic;
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
        public delegate void GetBlueprintWarnings(LevelEngine level, NamespaceID[] blueprintsForChoose, NamespaceID[] chosenBlueprints, List<string> warnings);
        public delegate void PostLevelStop(LevelEngine level);

        public readonly static CallbackReference<GetBlueprintNotRecommonded> GET_BLUEPRINT_NOT_RECOMMONDED = new();
        public readonly static CallbackReference<GetBlueprintWarnings> GET_BLUEPRINT_WARNINGS = new();
        public readonly static CallbackReference<PostLevelStop> POST_LEVEL_STOP = new();
    }
}
