﻿using System.Collections.Generic;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2Logic.Callbacks
{
    public static class LogicLevelCallbacks
    {
        public struct GetBlueprintNotRecommondedParams
        {
            public LevelEngine level;
            public NamespaceID blueprintID;

            public GetBlueprintNotRecommondedParams(LevelEngine level, NamespaceID blueprintID)
            {
                this.level = level;
                this.blueprintID = blueprintID;
            }
        }
        public struct GetBlueprintWarningsParams
        {
            public LevelEngine level;
            public NamespaceID[] blueprintsForChoose;
            public BlueprintChooseItem[] chosenBlueprints;
            public List<string> warnings;

            public GetBlueprintWarningsParams(LevelEngine level, NamespaceID[] blueprintsForChoose, BlueprintChooseItem[] chosenBlueprints, List<string> warnings)
            {
                this.level = level;
                this.blueprintsForChoose = blueprintsForChoose;
                this.chosenBlueprints = chosenBlueprints;
                this.warnings = warnings;
            }
        }
        public struct PostBlueprintSelectionParams
        {
            public LevelEngine level;
            public BlueprintChooseItem[] chosenBlueprints;

            public PostBlueprintSelectionParams(LevelEngine level, BlueprintChooseItem[] chosenBlueprints)
            {
                this.level = level;
                this.chosenBlueprints = chosenBlueprints;
            }
        }

        public readonly static CallbackType<LevelCallbackParams> POST_LEVEL_STOP = new();
        public readonly static CallbackType<LevelCallbackParams> PRE_BATTLE = new();
        public readonly static CallbackType<GetBlueprintNotRecommondedParams> GET_BLUEPRINT_NOT_RECOMMONDED = new();
        public readonly static CallbackType<GetBlueprintWarningsParams> GET_BLUEPRINT_WARNINGS = new();
        public readonly static CallbackType<PostBlueprintSelectionParams> POST_BLUEPRINT_SELECTION = new();
    }
}
