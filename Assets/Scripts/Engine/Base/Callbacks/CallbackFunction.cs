using System;

namespace PVZEngine.Callbacks
{
    public class CallbackFunction<TOut> : CallbackActionBase<Func<TOut>>
    {
        public TOut Run()
        {
            return action.Invoke();
        }
    }
    public class CallbackFunction<T1, TOut> : CallbackActionBase<Func<T1, TOut>>
    {
        public TOut Run(T1 param1)
        {
            return action.Invoke(param1);
        }
    }
    public class CallbackFunction<T1, T2, TOut> : CallbackActionBase<Func<T1, T2, TOut>>
    {
        public TOut Run(T1 param1, T2 param2)
        {
            return action.Invoke(param1, param2);
        }
    }
    public class CallbackFunction<T1, T2, T3, TOut> : CallbackActionBase<Func<T1, T2, T3, TOut>>
    {
        public TOut Run(T1 param1, T2 param2, T3 param3)
        {
            return action.Invoke(param1, param2, param3);
        }
    }
    public class CallbackFunction<T1, T2, T3, T4, TOut> : CallbackActionBase<Func<T1, T2, T3, T4, TOut>>
    {
        public TOut Run(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return action.Invoke(param1, param2, param3, param4);
        }
    }
}
