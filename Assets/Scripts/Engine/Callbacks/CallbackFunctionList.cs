using System;
using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public class CallbackFunctionList<TOut> : CallbackListBase<CallbackFunction<TOut>, Func<TOut>>
    {
    }
    public class CallbackFunctionList<T1, TOut> : CallbackListBase<CallbackFunction<T1, TOut>, Func<T1, TOut>>
    {
    }
    public class CallbackFunctionList<T1, T2, TOut> : CallbackListBase<CallbackFunction<T1, T2, TOut>, Func<T1, T2, TOut>>
    {
    }
    public class CallbackFunctionList<T1, T2, T3, TOut> : CallbackListBase<CallbackFunction<T1, T2, T3, TOut>, Func<T1, T2, T3, TOut>>
    {
    }
    public class CallbackFunctionList<T1, T2, T3, T4, TOut> : CallbackListBase<CallbackFunction<T1, T2, T3, T4, TOut>, Func<T1, T2, T3, T4, TOut>>
    {
    }
}
