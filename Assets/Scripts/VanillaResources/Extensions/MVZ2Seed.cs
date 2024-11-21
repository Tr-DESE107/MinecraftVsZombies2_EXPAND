using MVZ2.GameContent;
using PVZEngine.SeedPacks;

namespace MVZ2.Extensions
{
    public static class MVZ2Seed
    {
        public static bool IsTwinkling(this SeedPack seed)
        {
            return seed.GetProperty<bool>(BuiltinSeedProps.TWINKLING);
        }
        public static void SetTwinkling(this SeedPack seed, bool value)
        {
            seed.SetProperty(BuiltinSeedProps.TWINKLING, value);
        }
    }
}
