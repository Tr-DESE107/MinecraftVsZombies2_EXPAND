using System.Collections.Generic;
using System.Linq;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Triggers;

namespace MVZ2Logic
{
    public static class LogicGameExt
    {
        public static bool IsSpecialUserName(this IGame game, string name)
        {
            var result = new TriggerResultBoolean()
            {
                Result = false
            };
            game.RunCallback(LogicCallbacks.IS_SPECIAL_USER_NAME, result, c => c(name, result));
            return result.Result;
        }
        public static NamespaceID[] GetInnateBlueprints(this IGame game)
        {
            var result = new TriggerResultNamespaceIDList()
            {
                Result = new List<NamespaceID>()
            };
            game.RunCallback(LogicCallbacks.GET_INNATE_BLUEPRINTS, result, c => c(result));
            return result.Result.ToArray();
        }
    }
}
