using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaCart : EntityDefinition
    {
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var cart = entity.ToCart();
            switch (cart.State)
            {
                case CartStates.TRIGGERED:
                    // 如果超出屏幕，消失。
                    if (entity.GetBounds().min.x >= MVZ2Game.GetBorderX(true))
                    {
                        entity.Remove();
                    }
                    break;
            }
        }
        public override int Type => EntityTypes.CART;
    }

}