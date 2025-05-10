using System.Collections.Generic;
using MVZ2Logic.Callbacks;
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
    }
}
