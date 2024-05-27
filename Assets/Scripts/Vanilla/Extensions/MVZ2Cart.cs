using MVZ2.GameContent;
using PVZEngine;
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

        public static bool CanCartCrush(this Entity entity, Entity target)
        {
            if (entity == null)
                return false;
            var bounds = entity.GetBounds();
            return target.Type != EntityTypes.BOSS &&
                entity.IsEnemy(target) &&
                target.IsActiveEntity() &&
                entity.GetLane() == target.GetLane() &&
                target.Pos.x >= bounds.min.x &&
                target.Pos.x <= bounds.max.x;
        }
        public static void TriggerCart(this Entity entity)
        {
            entity.State = EntityStates.CART_TRIGGERED;
            entity.Velocity = Vector3.right * 2;
        }
    }
}
