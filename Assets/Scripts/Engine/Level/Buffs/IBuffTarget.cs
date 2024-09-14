namespace PVZEngine.LevelManagement
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
