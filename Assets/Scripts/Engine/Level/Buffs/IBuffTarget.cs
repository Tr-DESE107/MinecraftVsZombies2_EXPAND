namespace PVZEngine.LevelManaging
{
    public interface IBuffTarget
    {
        ISerializeBuffTarget SerializeBuffTarget();
    }
    public interface ISerializeBuffTarget
    {
        IBuffTarget DeserializeBuffTarget(Level level);
    }
}
