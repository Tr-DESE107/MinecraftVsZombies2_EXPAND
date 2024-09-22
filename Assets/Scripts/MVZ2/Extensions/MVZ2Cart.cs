using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Extensions
{
    public static class MVZ2Cart
    {
        public static void TriggerCart(this Entity entity)
        {
            entity.State = EntityStates.CART_TRIGGERED;
            entity.Velocity = Vector3.right * 10;
            entity.Level.PlaySound(entity.GetCartTriggerSound(), entity.Pos);
        }
    }
}
