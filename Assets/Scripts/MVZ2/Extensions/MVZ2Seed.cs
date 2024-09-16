using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.GameContent;
using PVZEngine.Level;

namespace MVZ2
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
