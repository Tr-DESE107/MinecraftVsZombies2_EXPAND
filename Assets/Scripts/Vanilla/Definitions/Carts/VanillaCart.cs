using System.Linq;
using MVZ2.Extensions;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Carts
{
    public abstract class VanillaCart : VanillaEntity
    {
        protected VanillaCart(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinCartProps.CART_TRIGGER_SOUND, SoundID.minecart);
            SetProperty(EntityProperties.CAN_UNDER_GROUND, true);
            SetProperty(EntityProperties.SIZE, new Vector3(56, 32, 56));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.LeftFaction);
            UpdateCartTint(entity);
        }

        public override void Update(Entity entity)
        {
            base.Update(entity);
            switch (entity.State)
            {
                default:
                    if (entity.Pos.x < BuiltinLevel.CART_TARGET_X)
                    {
                        entity.Velocity = Vector3.right * 10;
                    }
                    else
                    {
                        var pos = entity.Pos;
                        pos.x = BuiltinLevel.CART_TARGET_X;
                        entity.Pos = pos;
                        entity.Velocity = Vector3.zero;
                    }
                    UpdateCartTint(entity);

                    bool triggered = entity.Level.GetEntities(EntityTypes.ENEMY)
                        .Any(e => !e.IsDead && entity.IsEnemy(e) && e.GetLane() == entity.GetLane() && e.Pos.x <= entity.Pos.x + TRIGGER_DISTANCE);
                    if (triggered)
                    {
                        entity.TriggerCart();
                    }
                    break;
                case EntityStates.CART_TRIGGERED:
                    entity.Velocity = Vector3.right * 10;
                    // 获取所有接触到的僵尸。
                    foreach (Entity ent in entity.Level.FindEntities(e => entity.CanCartCrush(e)))
                    {
                        // 碰到小车的僵尸受到伤害。
                        ent.TakeDamage(58115310, new DamageEffectList(DamageFlags.DAMAGE_BOTH_ARMOR_AND_BODY, DamageEffects.MUTE), new EntityReferenceChain(entity));
                    }
                    // 如果超出屏幕，消失。
                    if (entity.GetBounds().min.x >= BuiltinLevel.GetBorderX(true))
                    {
                        entity.Remove();
                    }
                    break;
            }
        }
        private void UpdateCartTint(Entity cart)
        {
            var alpha = Mathf.Lerp(0, 1, (cart.Pos.x - BuiltinLevel.CART_START_X) / (BuiltinLevel.CART_TARGET_X - BuiltinLevel.CART_START_X));
            var tint = cart.GetTint();
            tint = new Color(1, 1, 1, alpha);
            cart.SetTint(tint);
            cart.SetShadowAlpha(alpha);
        }
        public override int Type => EntityTypes.CART;
        public const float TRIGGER_DISTANCE = 28;
    }

}