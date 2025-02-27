using System.Collections.Generic;

namespace PVZEngine.Triggers
{
    public interface IInterruptSource
    {
        bool IsInterrupted { get; }
    }
    public abstract class TriggerResult<T> : IInterruptSource
    {
        public void Interrupt()
        {
            IsInterrupted = true;
        }
        public T Result { get; set; }
        public bool IsInterrupted { get; private set; }
    }
    public class TriggerResultBoolean : TriggerResult<bool>
    {
    }
    public class TriggerResultInt : TriggerResult<int>
    {
    }
    public class TriggerResultFloat : TriggerResult<float>
    {
    }
    public class TriggerResultNamespaceID : TriggerResult<NamespaceID>
    {
    }
    public class TriggerResultNamespaceIDList : TriggerResult<List<NamespaceID>>
    {
    }
}
