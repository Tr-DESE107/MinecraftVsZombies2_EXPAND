using System;
using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Callbacks;

namespace MVZ2.Games
{
    public partial class Game : IGameTriggerSystem, ICallbackRunner
    {
        #region 公有方法
        public void AddTrigger(ITrigger trigger)
        {
            callbacks.AddCallback(trigger);
        }
        public bool RemoveTrigger(ITrigger trigger)
        {
            return callbacks.RemoveCallback(trigger);
        }

        public void RunCallback<TArgs>(CallbackType<TArgs> callbackType, TArgs args)
        {
            callbacks.RunCallback(callbackType, args);
        }
        public void RunCallbackWithResult<TArgs>(CallbackType<TArgs> callbackType, TArgs args, CallbackResult result)
        {
            callbacks.RunCallbackWithResult(callbackType, args, result);
        }

        public void RunCallbackFiltered<TArgs>(CallbackType<TArgs> callbackType, TArgs args, object filter)
        {
            callbacks.RunCallbackFiltered(callbackType, args, filter);
        }
        public void RunCallbackWithResultFiltered<TArgs>(CallbackType<TArgs> callbackType, TArgs args, CallbackResult result, object filter)
        {
            callbacks.RunCallbackWithResultFiltered(callbackType, args, result, filter);
        }
        #endregion

        #region 属性字段
        private CallbackSystem callbacks = new CallbackSystem();
        #endregion
    }
}