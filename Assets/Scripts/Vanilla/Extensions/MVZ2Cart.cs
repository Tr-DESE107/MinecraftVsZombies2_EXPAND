using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Cart
    {
        //public void TurnToMoney()
        //{
        //    Id moneyId;
        //    if (Level.Difficulty == GameDifficulty.Easy)
        //    {
        //        moneyId = DropID.emerald;
        //    }
        //    else
        //    {
        //        moneyId = DropID.ruby;
        //    }

        //    Pickup money = CreateDrop(moneyId, Pos, Vector3.zero);
        //    money.Collect();

        //    Remove();
        //}
        public static NamespaceID GetCartTriggerSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(CartProps.CART_TRIGGER_SOUND);
        }
        public static bool CanCartCrush(this Entity cart, Entity target)
        {
            if (cart == null)
                return false;
            var bounds = cart.GetBounds();
            return target.Type != EntityTypes.BOSS &&
                cart.IsEnemy(target) &&
                target.IsActiveEntity() &&
                cart.GetLane() == target.GetLane() &&
                target.Pos.x >= bounds.min.x &&
                target.Pos.x <= bounds.max.x;
        }
        public static bool IsCartTriggered(this Entity entity)
        {
            return entity.State == EntityStates.CART_TRIGGERED;
        }
        public static void TriggerCart(this Entity entity)
        {
            entity.State = EntityStates.CART_TRIGGERED;
            entity.Velocity = Vector3.right * 10;
            entity.Level.PlaySound(entity.GetCartTriggerSound(), entity.Pos);
        }
    }
}
