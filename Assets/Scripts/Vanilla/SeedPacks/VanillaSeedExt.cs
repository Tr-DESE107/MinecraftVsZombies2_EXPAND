using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Vanilla.Contraptions;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.SeedPacks
{
    public static class VanillaSeedExt
    {
        public static bool CanInstantTrigger(this SeedPack seedPack)
        {
            var blueprintDef = seedPack?.Definition;
            return blueprintDef.IsTriggerActive() && blueprintDef.CanInstantTrigger();
        }
    }
}
