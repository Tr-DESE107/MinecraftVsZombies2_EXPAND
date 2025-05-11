using System.Linq;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class UpgradePlaceMethod : PlaceMethod
    {
        public override NamespaceID GetPlaceError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            var entities = grid.GetEntities();
            if (entities.Count() <= 0 || !entities.Any(e => e.IsEntityOf(entity.GetUpgradeFromEntity()) && e.IsFriendlyEntity()))
            {
                return VanillaGridStatus.onlyUpgrade;
            }
            return null;
        }
        public override Entity PlaceEntity(PlacementDefinition placement, LawnGrid grid, EntityDefinition entityDef)
        {
            var upgradeFromEntity = entityDef.GetUpgradeFromEntity();
            if (NamespaceID.IsValid(upgradeFromEntity))
            {
                var entity = grid.GetEntities().FirstOrDefault(e => e.IsEntityOf(upgradeFromEntity));
                if (entity != null && entity.Exists())
                {
                    return entity.UpgradeToContraption(entityDef.GetID());
                }
            }
            return null;
        }
    }
}
