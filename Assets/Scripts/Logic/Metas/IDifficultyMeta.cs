using PVZEngine;

namespace MVZ2Logic.Level
{
    public interface IDifficultyMeta
    {
        string ID { get; }
        string Name { get; }
        NamespaceID BuffID { get; }
        NamespaceID IZombieBuffID { get; }
        int CartConvertMoney { get; }
        int ClearMoney { get; }
        int RerunClearMoney { get; }
        int PuzzleMoney { get; }
    }
}
