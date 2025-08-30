namespace PVZEngine.Level
{
    public interface ILevelSourceReference
    {
        ILevelSourceReference Clone();
        ILevelSourceTarget GetTarget(LevelEngine level);
        ILevelSourceReference Parent { get; }
        int Faction { get; }
        NamespaceID DefinitionID { get; }
        long ID { get; }
    }
    public interface ILevelSourceTarget
    {
    }
}
