using MVZ2Logic.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaCartExt
    {
        public static bool CanCartCrush(this Entity cart, Entity target)
        {
            if (cart == null)
                return false;
            var bounds = cart.GetBounds();
            return target.Type != EntityTypes.BOSS &&
                cart.IsHostile(target) &&
                target.IsActiveEntity() &&
                cart.GetLane() == target.GetLane() &&
                target.Position.x >= bounds.min.x &&
                target.Position.x <= bounds.max.x;
        }
        public static bool IsCartTriggered(this Entity entity)
        {
            return entity.State == VanillaEntityStates.CART_TRIGGERED;
        }
        public static void TriggerCart(this Entity entity)
        {
            entity.State = VanillaEntityStates.CART_TRIGGERED;
            entity.Velocity = Vector3.right * 10;
            entity.PlaySound(entity.GetCartTriggerSound());
            entity.SetProperty(VanillaEntityProps.UPDATE_BEFORE_GAME, false);
        }
    }
}
