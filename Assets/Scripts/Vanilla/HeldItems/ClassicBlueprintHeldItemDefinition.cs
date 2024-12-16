using System.Linq;
using MVZ2.GameContent.Obstacles;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.Vanilla.HeldItems
{
    [Definition(BuiltinHeldItemNames.blueprint)]
    public class ClassicBlueprintHeldItemDefinition : BlueprintHeldItemDefinition
    {
        public ClassicBlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        protected override SeedPack GetSeedPackAt(LevelEngine level, int index)
        {
            return level.GetSeedPackAt(index);
        }
        protected override void PostPlaceEntity(LawnGrid grid, IHeldItemData data, SeedPack seed, Entity entity)
        {
            base.PostPlaceEntity(grid, data, seed, entity);
            var level = grid.Level;
            var seedDef = seed.Definition;
            level.AddEnergy(-seedDef.GetCost());
            level.SetRechargeTimeToUsed(seed);
            seed.ResetRecharge();
        }
    }
}
