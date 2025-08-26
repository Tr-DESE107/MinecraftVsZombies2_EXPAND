﻿using System.Collections.Generic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Entities;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Callbacks;

namespace MVZ2Logic.Games
{
    public static class LogicGameExt
    {
        public static bool IsSpecialUserName(this IGlobalGame game, string name)
        {
            var result = new CallbackResult(false);
            game.RunCallbackWithResult(LogicCallbacks.IS_SPECIAL_USER_NAME, new StringCallbackParams(name), result);
            return result.GetValue<bool>();
        }
        public static NamespaceID[] GetInnateBlueprints(this IGlobalGame game)
        {
            var list = new List<NamespaceID>();
            var param = new LogicCallbacks.GetInnateBlueprintsParams()
            {
                list = list
            };
            game.RunCallback(LogicCallbacks.GET_INNATE_BLUEPRINTS, param);
            return list.ToArray();
        }
        public static NamespaceID[] GetInnateArtifacts(this IGlobalGame game)
        {
            var list = new List<NamespaceID>();
            var param = new LogicCallbacks.GetInnateArtifactsParams()
            {
                list = list
            };
            game.RunCallback(LogicCallbacks.GET_INNATE_ARTIFACTS, param);
            return list.ToArray();
        }

        public static string GetEntityName(this IGlobalGame game, NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var def = game.GetEntityDefinition(entityID);
            if (def == null)
                return entityID.ToString();
            var name = def.GetEntityName() ?? LogicStrings.UNKNOWN_ENTITY_NAME;
            return Global.Localization.GetTextParticular(name, LogicStrings.CONTEXT_ENTITY_NAME);
        }
        public static string GetEntityTooltip(this IGlobalGame game, NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var def = game.GetEntityDefinition(entityID);
            if (def == null)
                return entityID.ToString();
            var tooltip = def.GetEntityTooltip() ?? LogicStrings.UNKNOWN_ENTITY_TOOLTIP;
            return Global.Localization.GetTextParticular(tooltip, LogicStrings.CONTEXT_ENTITY_TOOLTIP);
        }

        public static string GetEntityDeathMessage(this IGlobalGame game, NamespaceID entityID)
        {
            string key = LogicStrings.DEATH_MESSAGE_UNKNOWN;
            if (entityID != null)
            {
                var def = game.GetEntityDefinition(entityID);
                var deathMessage = def?.GetDeathMessage();
                if (deathMessage != null)
                {
                    key = deathMessage;
                }
            }
            return Global.Localization.GetTextParticular(key, LogicStrings.CONTEXT_DEATH_MESSAGE);
        }

        #region 制品
        public static string GetArtifactName(this IGlobalGame game, NamespaceID artifactID)
        {
            if (artifactID == null)
                return "null";
            var def = game.GetArtifactDefinition(artifactID);
            if (def == null)
                return artifactID.ToString();
            var name = def.GetArtifactName() ?? LogicStrings.UNKNOWN_ARTIFACT_NAME;
            return Global.Localization.GetTextParticular(name, LogicStrings.CONTEXT_ARTIFACT_NAME);
        }
        public static string GetArtifactTooltip(this IGlobalGame game, NamespaceID artifactID)
        {
            if (artifactID == null)
                return "null";
            var def = game.GetArtifactDefinition(artifactID);
            if (def == null)
                return artifactID.ToString();
            var tooltip = def.GetArtifactTooltip() ?? LogicStrings.UNKNOWN_ARTIFACT_TOOLTIP;
            return Global.Localization.GetTextParticular(tooltip, LogicStrings.CONTEXT_ARTIFACT_TOOLTIP);
        }
        #endregion

        public static string GetGridErrorMessage(this IGlobalGame game, NamespaceID id)
        {
            var def = game.GetGridErrorDefinition(id);
            if (def == null)
                return null;
            return def.Message;
        }
        public static string GetBlueprintErrorMessage(this IGlobalGame game, NamespaceID id)
        {
            var def = game.GetSeedErrorDefinition(id);
            if (def == null)
                return null;
            return def.Message;
        }
        public static string GetEntityCounterName(this IGlobalGame game, NamespaceID counterID)
        {
            if (counterID == null)
                return "null";
            var def = game.GetEntityCounterDefinition(counterID);
            if (def == null)
                return counterID.ToString();
            var name = def.Name ?? LogicStrings.UNKNOWN_ENTITY_COUNTER_NAME;
            return Global.Localization.GetTextParticular(name, LogicStrings.CONTEXT_ENTITY_COUNTER_NAME);
        }
        public static string GetDifficultyName(this IGlobalGame game, NamespaceID difficulty)
        {
            if (difficulty == null)
                return "null";
            var def = game.GetDifficultyDefinition(difficulty);
            if (def == null)
                return difficulty.ToString();
            string name = def.Name ?? LogicStrings.DIFFICULTY_UNKNOWN;
            return Global.Localization.GetTextParticular(name, LogicStrings.CONTEXT_DIFFICULTY);
        }
        public static string GetSeedOptionName(this IGlobalGame game, NamespaceID id)
        {
            if (id == null)
                return "null";
            var def = game.GetSeedOptionDefinition(id);
            if (def == null)
                return id.ToString();
            var name = def.GetOptionName() ?? LogicStrings.UNKNOWN_OPTION_NAME;
            return Global.Localization.GetTextParticular(name, LogicStrings.CONTEXT_OPTION_NAME);
        }
        public static string GetBlueprintName(this IGlobalGame game, NamespaceID blueprintID)
        {
            string name = string.Empty;
            var definition = game.GetSeedDefinition(blueprintID);
            if (definition == null)
                return name;
            var seedType = definition.GetSeedType();
            if (seedType == SeedTypes.ENTITY)
            {
                var customEntityDef = game.GetEntitySeedDefinition(blueprintID);
                var customName = customEntityDef?.BlueprintName;
                if (!string.IsNullOrEmpty(customName))
                {
                    name = Global.Localization.GetTextParticular(customName, LogicStrings.CONTEXT_ENTITY_NAME);
                }
                else
                {
                    var entityID = definition.GetSeedEntityID();
                    name = game.GetEntityName(entityID);
                }
            }
            else if (seedType == SeedTypes.OPTION)
            {
                var optionID = definition.GetSeedOptionID();
                name = game.GetSeedOptionName(optionID);
            }
            return name;
        }
        public static string GetBlueprintTooltip(this IGlobalGame game, NamespaceID blueprintID)
        {
            var definition = game.GetSeedDefinition(blueprintID);
            if (definition == null)
                return string.Empty;
            var seedType = definition.GetSeedType();
            if (seedType == SeedTypes.ENTITY)
            {
                var customEntityDef = game.GetEntitySeedDefinition(blueprintID);
                var customTooltip = customEntityDef?.BlueprintTooltip;
                if (!string.IsNullOrEmpty(customTooltip))
                {
                    return Global.Localization.GetTextParticular(customTooltip, LogicStrings.CONTEXT_ENTITY_TOOLTIP);
                }
                else
                {
                    var entityID = definition.GetSeedEntityID();
                    return game.GetEntityTooltip(entityID);
                }
            }
            return string.Empty;
        }
    }
}
