namespace PVZEngine.Level
{
    public interface IBuffTarget
    {
        ISerializeBuffTarget SerializeBuffTarget();
        bool RemoveBuff(Buff buff);
    }
    public interface ISerializeBuffTarget
    {
        IBuffTarget DeserializeBuffTarget(LevelEngine level);
    }
}
