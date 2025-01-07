using PVZEngine;

namespace MVZ2Logic.SeedPacks
{
    public interface ISeedOptionMeta
    {
        string ID { get; }
        int Cost { get; }
        SpriteReference Icon { get; }
    }
}
