using System.Collections.Generic;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Entities;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Callbacks;

namespace MVZ2Logic
{
    public static class LogicGameExt
    {
        public static bool IsSpecialUserName(this IGame game, string name)
        {
            var result = new CallbackResult(false);
            game.RunCallbackWithResult(LogicCallbacks.IS_SPECIAL_USER_NAME, new StringCallbackParams(name), result);
            return result.GetValue<bool>();
        }
        public static NamespaceID[] GetInnateBlueprints(this IGame game)
        {
            var list = new List<NamespaceID>();
            var param = new LogicCallbacks.GetInnateBlueprintsParams()
            {
                list = list
            };
            game.RunCallback(LogicCallbacks.GET_INNATE_BLUEPRINTS, param);
            return list.ToArray();
        }
        public static NamespaceID[] GetInnateArtifacts(this IGame game)
        {
            var list = new List<NamespaceID>();
            var param = new LogicCallbacks.GetInnateArtifactsParams()
            {
                list = list
            };
            game.RunCallback(LogicCallbacks.GET_INNATE_ARTIFACTS, param);
            return list.ToArray();
        }

        public static string GetEntityName(this IGame game, NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var def = game.GetEntityDefinition(entityID);
            if (def == null)
                return entityID.ToString();
            var name = def.GetEntityName() ?? LogicStrings.UNKNOWN_ENTITY_NAME;
            return game.GetTextParticular(name, LogicStrings.CONTEXT_ENTITY_NAME);
        }
        public static string GetEntityTooltip(this IGame game, NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var def = game.GetEntityDefinition(entityID);
            if (def == null)
                return entityID.ToString();
            var tooltip = def.GetEntityTooltip() ?? LogicStrings.UNKNOWN_ENTITY_TOOLTIP;
            return game.GetTextParticular(tooltip, LogicStrings.CONTEXT_ENTITY_TOOLTIP);
        }

        public static string GetEntityDeathMessage(this IGame game, NamespaceID entityID)
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
            return game.GetTextParticular(key, LogicStrings.CONTEXT_DEATH_MESSAGE);
        }

        #region 制品
        public static string GetArtifactName(this IGame game, NamespaceID artifactID)
        {
            if (artifactID == null)
                return "null";
            var def = game.GetArtifactDefinition(artifactID);
            if (def == null)
                return artifactID.ToString();
            var name = def.GetArtifactName() ?? LogicStrings.UNKNOWN_ARTIFACT_NAME;
            return game.GetTextParticular(name, LogicStrings.CONTEXT_ARTIFACT_NAME);
        }
        public static string GetArtifactTooltip(this IGame game, NamespaceID artifactID)
        {
            if (artifactID == null)
                return "null";
            var def = game.GetArtifactDefinition(artifactID);
            if (def == null)
                return artifactID.ToString();
            var tooltip = def.GetArtifactTooltip() ?? LogicStrings.UNKNOWN_ARTIFACT_TOOLTIP;
            return game.GetTextParticular(tooltip, LogicStrings.CONTEXT_ARTIFACT_TOOLTIP);
        }
        #endregion

        public static string GetGridErrorMessage(this IGame game, NamespaceID id)
        {
            var def = game.GetGridErrorDefinition(id);
            if (def == null)
                return null;
            return def.Message;
        }
        public static string GetBlueprintErrorMessage(this IGame game, NamespaceID id)
        {
            var def = game.GetSeedErrorDefinition(id);
            if (def == null)
                return null;
            return def.Message;
        }
        public static string GetEntityCounterName(this IGame game, NamespaceID counterID)
        {
            if (counterID == null)
                return "null";
            var meta = game.GetEntityCounterMeta(counterID);
            if (meta == null)
                return counterID.ToString();
            var name = meta.Name ?? LogicStrings.UNKNOWN_ENTITY_COUNTER_NAME;
            return game.GetTextParticular(name, LogicStrings.CONTEXT_ENTITY_COUNTER_NAME);
        }
        public static string GetDifficultyName(this IGame game, NamespaceID difficulty)
        {
            if (difficulty == null)
                return "null";
            var def = game.GetDifficultyDefinition(difficulty);
            if (def == null)
                return difficulty.ToString();
            string name = def.Name ?? LogicStrings.DIFFICULTY_UNKNOWN;
            return game.GetTextParticular(name, LogicStrings.CONTEXT_DIFFICULTY);
        }
    }
}
