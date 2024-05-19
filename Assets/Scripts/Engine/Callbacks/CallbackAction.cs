using System;

namespace PVZEngine
{
    public class CallbackAction : CallbackActionBase<Action>
    {
        public void Run()
        {
            action.Invoke();
        }
    }
    public class CallbackAction<T1> : CallbackActionBase<Action<T1>>
    {
        public void Run(T1 param1)
        {
            action.Invoke(param1);
        }
    }
    public class CallbackAction<T1, T2> : CallbackActionBase<Action<T1, T2>>
    {
        public void Run(T1 param1, T2 param2)
        {
            action.Invoke(param1, param2);
        }
    }
    public class CallbackAction<T1, T2, T3> : CallbackActionBase<Action<T1, T2, T3>>
    {
        public void Run(T1 param1, T2 param2, T3 param3)
        {
            action.Invoke(param1, param2, param3);
        }
    }
    public class CallbackAction<T1, T2, T3, T4> : CallbackActionBase<Action<T1, T2, T3, T4>>
    {
        public void Run(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            action.Invoke(param1, param2, param3, param4);
        }
    }
}
