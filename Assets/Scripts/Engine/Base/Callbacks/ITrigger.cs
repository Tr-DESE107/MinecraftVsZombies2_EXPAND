namespace PVZEngine.Callbacks
{
    public interface ITrigger
    {
        public ICallbackType Type { get; }
        public int Priority { get; }
    }
}
