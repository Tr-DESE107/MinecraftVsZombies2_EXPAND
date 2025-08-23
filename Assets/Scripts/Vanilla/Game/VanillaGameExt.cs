using System.Linq;
using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2.Vanilla.Game
{
    public static class VanillaGameExt
    {
        public static string GetEntityName(this IGame game, NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var def = game.GetEntityDefinition(entityID);
            if (def == null)
                return entityID.ToString();
            var name = def.Name ?? VanillaStrings.UNKNOWN_ENTITY_NAME;
            return game.GetTextParticular(name, VanillaStrings.CONTEXT_ENTITY_NAME);
        }
        public static string GetEntityCounterName(this IGame game, NamespaceID counterID)
        {
            if (counterID == null)
                return "null";
            var meta = game.GetEntityCounterMeta(counterID);
            if (meta == null)
                return counterID.ToString();
            var name = meta.Name ?? VanillaStrings.UNKNOWN_ENTITY_COUNTER_NAME;
            return game.GetTextParticular(name, VanillaStrings.CONTEXT_ENTITY_COUNTER_NAME);
        }
        public static bool IsRandomChina(this IGame game)
        {
            var userName = game.GetCurrentUserName();
            return game.IsRandomChinaUserName(userName);
        }
        public static bool IsRandomChinaUserName(this IGame game, string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return randomChinaNames.Any(n => n.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }
        public static readonly string[] randomChinaNames = new string[]
        {
            "RandomChina",
            "RandmChina",
        };
        public static bool IsDebugUser(this IGame game)
        {
            var userName = game.GetCurrentUserName();
            return game.IsDebugUserName(userName);
        }
        public static bool IsDebugUserName(this IGame game, string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return debugUserNames.Any(n => n.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }
        public static readonly string[] debugUserNames = new string[]
        {
            "debug",
        };
    }
}
