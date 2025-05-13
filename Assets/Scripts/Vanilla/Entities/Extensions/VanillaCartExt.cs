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
                target.ExistsAndAlive() &&
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
            entity.SetCanUpdateBeforeGameStart(false);
            foreach (var behaviour in entity.Definition.GetBehaviours<CartBehaviour>())
            {
                behaviour.PostTrigger(entity);
            }
        }
        public static void ChargeUpCartTrigger(this Entity entity)
        {
            CartCommonBehaviour.ChargeUpTrigger(entity);
        }
    }
}
