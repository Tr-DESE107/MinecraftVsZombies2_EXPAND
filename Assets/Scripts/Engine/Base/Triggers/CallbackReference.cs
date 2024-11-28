using System;

namespace PVZEngine.Triggers
{
    public abstract class CallbackReference
    {
        public CallbackReference(NamespaceID id)
        {
            ID = id;
        }

        public NamespaceID ID { get; }
    }
    public class CallbackReference<T> : CallbackReference where T : Delegate
    {
        public CallbackReference(NamespaceID id) : base(id)
        {
        }
    }
}
