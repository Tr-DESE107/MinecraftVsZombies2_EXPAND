namespace PVZEngine.Level
{
    public interface IBuffTarget
    {
        ISerializeBuffTarget SerializeBuffTarget();
    }
    public interface ISerializeBuffTarget
    {
        IBuffTarget DeserializeBuffTarget(LevelEngine level);
    }
}
