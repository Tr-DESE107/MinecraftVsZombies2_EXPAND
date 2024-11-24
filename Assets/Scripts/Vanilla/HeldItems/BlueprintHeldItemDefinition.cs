using System.Linq;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.HeldItems
{
    [Definition(BuiltinHeldItemNames.blueprint)]
    public class BlueprintHeldItemDefinition : HeldItemDefinition
    {
        public BlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, long id)
        {
            return HeldFlags.None;
        }
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, long id)
        {
            var flags = HeldFlags.None;
            if (IsValidOnGrid(grid, id, out _))
            {
                flags |= HeldFlags.Valid;
            }
            return flags;
        }
        public override string GetHeldErrorMessageOnGrid(LawnGrid grid, long id)
        {
            if (!IsValidOnGrid(grid, id, out var message))
            {
                return message;
            }
            return null;
        }
        public override bool UseOnGrid(LawnGrid grid, long id)
        {
            var level = grid.Level;
            var seed = level.GetSeedPackAt((int)id);
            if (seed == null)
                return false;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var x = level.GetEntityColumnX(grid.Column);
                var z = level.GetEntityLaneZ(grid.Lane);
                var y = level.GetGroundY(x, z);

                var position = new Vector3(x, y, z);
                var entityID = seedDef.GetSeedEntityID();
                var entityDef = level.Content.GetEntityDefinition(entityID);
                level.Spawn(entityID, position, null);
                level.AddEnergy(-seedDef.GetCost());
                level.SetRechargeTimeToUsed(seed);
                seed.ResetRecharge();
                level.PlaySound(entityDef.GetPlaceSound(), position);
                return true;
            }
            return false;
        }
        public override void UseOnLawn(LevelEngine level, LawnArea area, long id)
        {
            base.UseOnLawn(level, area, id);
            if (area != LawnArea.Side)
                return;
            if (level.CancelHeldItem())
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        private bool IsValidOnGrid(LawnGrid grid, long id, out string errorMessage)
        {
            errorMessage = null;
            var level = grid.Level;
            var seed = level.GetSeedPackAt((int)id);
            if (seed == null)
                return false;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                var entityDef = level.Content.GetEntityDefinition(entityID);
                if (entityDef.Type == EntityTypes.PLANT)
                {
                    if (!grid.CanPlace(entityDef))
                    {
                        if (grid.GetTakenEntities().Any(e => e.IsEntityOf(VanillaObstacleID.gargoyleStatue)))
                            errorMessage = VanillaStrings.ADVICE_CANNOT_PLACE_ON_STATUES;
                        return false;
                    }
                }
            }
            return true;
        }
        public override bool IsForGrid() => true;
        public override bool IsForEntity() => false;
    }
}
