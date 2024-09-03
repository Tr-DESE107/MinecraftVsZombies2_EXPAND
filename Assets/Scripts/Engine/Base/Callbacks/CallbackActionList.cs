using System;

namespace PVZEngine.Callbacks
{
    public class CallbackActionList : CallbackListBase<CallbackAction, Action>
    {
        public void Run()
        {
            foreach (var callback in callbacks)
            {
                callback.Run();
            }
        }
        public void RunFiltered(object filter)
        {
            foreach (var callback in callbacks)
            {
                if (!callback.FilterParam(filter))
                    continue;
                callback.Run();
            }
        }
    }
    public class CallbackActionList<T1> : CallbackListBase<CallbackAction<T1>, Action<T1>>
    {
        public void Run(T1 param1)
        {
            foreach (var callback in callbacks)
            {
                callback.Run(param1);
            }
        }
        public void RunFiltered(object filter, T1 param1)
        {
            foreach (var callback in callbacks)
            {
                if (!callback.FilterParam(filter))
                    continue;
                callback.Run(param1);
            }
        }
    }
    public class CallbackActionList<T1, T2> : CallbackListBase<CallbackAction<T1, T2>, Action<T1, T2>>
    {
        public void Run(T1 param1, T2 param2)
        {
            foreach (var callback in callbacks)
            {
                callback.Run(param1, param2);
            }
        }
        public void RunFiltered(object filter, T1 param1, T2 param2)
        {
            foreach (var callback in callbacks)
            {
                if (!callback.FilterParam(filter))
                    continue;
                callback.Run(param1, param2);
            }
        }
    }
    public class CallbackActionList<T1, T2, T3> : CallbackListBase<CallbackAction<T1, T2, T3>, Action<T1, T2, T3>>
    {
        public void Run(T1 param1, T2 param2, T3 param3)
        {
            foreach (var callback in callbacks)
            {
                callback.Run(param1, param2, param3);
            }
        }
        public void RunFiltered(object filter, T1 param1, T2 param2, T3 param3)
        {
            foreach (var callback in callbacks)
            {
                if (!callback.FilterParam(filter))
                    continue;
                callback.Run(param1, param2, param3);
            }
        }
    }
    public class CallbackActionList<T1, T2, T3, T4> : CallbackListBase<CallbackAction<T1, T2, T3, T4>, Action<T1, T2, T3, T4>>
    {
        public void Run(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            foreach (var callback in callbacks)
            {
                callback.Run(param1, param2, param3, param4);
            }
        }
        public void RunFiltered(object filter, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            foreach (var callback in callbacks)
            {
                if (!callback.FilterParam(filter))
                    continue;
                callback.Run(param1, param2, param3, param4);
            }
        }
    }
}
