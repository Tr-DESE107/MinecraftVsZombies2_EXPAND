using MVZ2.GameContent;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    [Definition(HeldItemNames.blueprint)]
    public class BlueprintHeldItemDefinition : HeldItemDefinition
    {
        public BlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        public override bool IsValidOnEntity(Entity entity, int id)
        {
            return false;
        }
        public override bool IsValidOnGrid(LawnGrid grid, int id)
        {
            var level = grid.Level;
            var seed = level.GetSeedPackAt(id);
            if (seed == null)
                return false;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                var entityDef = level.ContentProvider.GetEntityDefinition(entityID);
                if (entityDef.Type == EntityTypes.PLANT)
                {
                    if (!grid.CanPlace(entityDef))
                        return false;
                }
            }
            return true;
        }
        public override bool UseOnGrid(LawnGrid grid, int id)
        {
            var level = grid.Level;
            var seed = level.GetSeedPackAt(id);
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
                var entityDef = level.ContentProvider.GetEntityDefinition(entityID);
                level.Spawn(entityID, position, null);
                level.AddEnergy(-seedDef.GetCost());
                level.SetRechargeTimeToUsed(seed);
                seed.ResetRecharge();
                level.PlaySound(entityDef.GetPlaceSound(), position);
                return true;
            }
            return false;
        }
        public override void UseOnLawn(LevelEngine level, LawnArea area, int id)
        {
            base.UseOnLawn(level, area, id);
            if (area != LawnArea.Side)
                return;
            if (level.CancelHeldItem())
            {
                level.PlaySound(SoundID.tap);
            }
        }
    }
}
