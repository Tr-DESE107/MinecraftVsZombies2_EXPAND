using System.Collections.Generic;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Grids;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Grids;

namespace MVZ2.GameContent.Implements
{
    public class CarrierImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_INIT, EntityInitCallback, filter: EntityTypes.PLANT);
        }
        private void EntityInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            gridBuffer.Clear();
            entity.GetTakenGridsNonAlloc(gridBuffer);
            foreach (var grid in gridBuffer)
            {
                var carrier = grid.GetCarrierEntity();
                if (carrier == null)
                    continue;
                foreach (var carrierBehaviour in carrier.Definition.GetBehaviours<ICarrierBehaviour>())
                {
                    carrierBehaviour.UpdateCarrier(carrier);
                }
            }
        }
        private static List<LawnGrid> gridBuffer = new List<LawnGrid>();
    }
}