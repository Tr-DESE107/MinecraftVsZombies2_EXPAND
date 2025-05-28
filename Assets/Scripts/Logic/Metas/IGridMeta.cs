namespace MVZ2Logic.Spawns
{
    public interface IGridLayerMeta
    {
        string ID { get; }
    }
    public interface IGridErrorMeta
    {
        string ID { get; }
        string Message { get; }
    }
}
