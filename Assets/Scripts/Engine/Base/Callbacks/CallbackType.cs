﻿namespace PVZEngine.Callbacks
{
    public interface ICallbackType
    {
        internal ICallbackHandler CreateHandler();
    }
    public sealed class CallbackType<TArgs> : ICallbackType
    {
        public CallbackType() { }
        ICallbackHandler ICallbackType.CreateHandler() => new CallbackHandler<TArgs>();
    }
}
