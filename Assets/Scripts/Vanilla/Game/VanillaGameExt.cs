using System;
using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.Game
{
    public static class VanillaGameExt
    {
        public static void AddCallback<T>(this IGameTriggerSystem game, CallbackReference<T> callbackRef, T action, int priority = 0, object filterValue = null) where T : Delegate
        {
            game.AddTrigger(new ModTrigger(callbackRef.ID, action, priority, filterValue));
        }
    }
}
