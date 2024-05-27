using System.Linq;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Carts
{
    public abstract class VanillaCart : VanillaEntity
    {
        protected VanillaCart(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Game.Option.LeftFaction);
        }

        public override void Update(Entity entity)
        {
            base.Update(entity);
            switch (entity.State)
            {
                default:
                    bool triggered = entity.Game.GetEntities(EntityTypes.ENEMY)
                        .Any(e => !e.IsDead && entity.IsEnemy(e) && e.GetLane() == entity.GetLane() && e.Pos.x <= entity.Pos.x + TRIGGER_DISTANCE);
                    if (triggered)
                    {
                        entity.TriggerCart();
                    }
                    break;
                case EntityStates.CART_TRIGGERED:
                    // 获取所有接触到的僵尸。
                    foreach (Entity ent in entity.Game.GetEntities().Where(e => entity.CanCartCrush(e)))
                    {
                        // 碰到小车的僵尸受到伤害。
                        ent.TakeDamage(58115310, new DamageEffectList(DamageFlags.DAMAGE_BOTH_ARMOR_AND_BODY), new EntityReferenceChain(entity));
                    }
                    // 如果超出屏幕，消失。
                    if (entity.GetBounds().min.x >= MVZ2Game.GetBorderX(true))
                    {
                        entity.Remove();
                    }
                    break;
            }
        }
        public override int Type => EntityTypes.CART;
        public const float TRIGGER_DISTANCE = 28;
    }

}