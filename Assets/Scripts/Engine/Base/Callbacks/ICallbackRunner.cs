using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZEngine.Callbacks
{
    public interface ICallbackRunner
    {
        void RunCallback<TArgs>(CallbackType<TArgs> callbackType, TArgs args);
        void RunCallbackWithResult<TArgs>(CallbackType<TArgs> callbackType, TArgs args, CallbackResult result);
        void RunCallbackWithResultFiltered<TArgs>(CallbackType<TArgs> callbackType, TArgs args, CallbackResult result, object filter);
        void RunCallbackFiltered<TArgs>(CallbackType<TArgs> callbackType, TArgs args, object filter);
    }
}
