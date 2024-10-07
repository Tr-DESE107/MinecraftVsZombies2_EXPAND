using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.GameContent;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public static class VanillaPickupExt
    {
        public static void Collect(this Entity pickup)
        {
            if (!PreCollect(pickup))
                return;
            pickup.State = EntityStates.COLLECTED;
            if (pickup.Definition is ICollectiblePickup collectible)
                collectible.PostCollect(pickup);
        }
        private static bool PreCollect(Entity entity)
        {
            if (entity.Definition is ICollectiblePickup collectible)
            {
                if (collectible.PreCollect(entity) == false)
                {
                    return false;
                }
            }
            var game = Global.Game;
            var triggers = game.GetTriggers(VanillaLevelCallbacks.PRE_PICKUP_COLLECT);
            foreach (var trigger in triggers)
            {
                var result = trigger.Invoke(entity);
                if (result is bool boolValue && !boolValue)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
