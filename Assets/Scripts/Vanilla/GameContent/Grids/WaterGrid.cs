using System.Linq;
using System.Threading;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Grids
{
    [Definition(VanillaGridNames.water)]
    public class WaterGrid : GridDefinition
    {
        public WaterGrid(string nsp, string name) : base(nsp, name)
        {
        }
        public override void CanPlaceEntity(LawnGrid grid, NamespaceID entityID, TriggerResultNamespaceID error)
        {
            var level = grid.Level;
            var entityDef = level.Content.GetEntityDefinition(entityID);
            if (entityDef.Type == EntityTypes.PLANT)
            {
                var carrier = grid.GetLayerEntity(VanillaGridLayers.carrier);
                bool hasLilyPad = carrier != null && carrier.IsEntityOf(VanillaContraptionID.lilyPad);
                if (hasLilyPad)
                {
                    if (!entityDef.CanPlaceOnPlane())
                    {
                        error.Result = VanillaGridStatus.notOnPlane;
                        return;
                    }
                    if (!entityDef.CanPlaceOnLilypad())
                    {
                        error.Result = VanillaGridStatus.notOnLilypad;
                        return;
                    }
                }
                else
                {
                    if (!entityDef.CanPlaceOnWater() && entityDef.CanPlaceOnPlane())
                    {
                        error.Result = VanillaGridStatus.needLilypad;
                        return;
                    }
                }

                var layersToTake = entityDef.GetGridLayersToTake();
                if (layersToTake.Any(l => grid.GetLayerEntity(l) != null))
                {
                    error.Result = VanillaGridStatus.alreadyTaken;
                    return;
                }
            }
        }
    }
}
