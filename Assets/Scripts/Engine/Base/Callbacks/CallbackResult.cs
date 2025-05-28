﻿using Tools;

namespace PVZEngine.Callbacks
{
    public class CallbackResult
    {
        public CallbackResult()
        {
        }
        public CallbackResult(object value)
        {
            SetValue(value);
        }
        public bool IsBreakRequested { get; private set; }
        private object value;
        public void SetValue(object value)
        {
            this.value = value;
        }
        public void SetFinalValue(object value)
        {
            SetValue(value);
            Break();
        }
        public T GetValue<T>()
        {
            if (value.TryToGeneric<T>(out var v))
                return v;
            return default;
        }
        public void Break() => IsBreakRequested = true;
    }
}
