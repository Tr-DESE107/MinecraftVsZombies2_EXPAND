using System;

namespace PVZEngine.Triggers
{
    public abstract class CallbackReference
    {
    }
    public class CallbackReference<T> : CallbackReference where T : Delegate
    {
    }
}
