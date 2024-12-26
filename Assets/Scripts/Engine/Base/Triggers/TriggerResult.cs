namespace PVZEngine.Triggers
{
    public abstract class TriggerResult<T>
    {
        public T Result { get; set; }
    }
    public class TriggerResultBoolean : TriggerResult<bool>
    {
    }
    public class TriggerResultInt : TriggerResult<int>
    {
    }
    public class TriggerResultNamespaceID : TriggerResult<NamespaceID>
    {
    }
}
