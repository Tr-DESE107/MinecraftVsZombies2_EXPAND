using System.Linq;
using MVZ2.GameContent.Buffs.Carts;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public abstract class VanillaCart : VanillaEntity
    {
        protected VanillaCart(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.LeftFaction);
            entity.SetProperty(VanillaEntityProps.UPDATE_BEFORE_GAME, true);
            entity.AddBuff<CartFadeInBuff>();
        }

        public override void Update(Entity entity)
        {
            base.Update(entity);
            switch (entity.State)
            {
                default:
                    if (entity.Position.x < VanillaLevelExt.CART_TARGET_X)
                    {
                        entity.Velocity = Vector3.right * 10;
                    }
                    else
                    {
                        var pos = entity.Position;
                        pos.x = VanillaLevelExt.CART_TARGET_X;
                        entity.Position = pos;
                        entity.Velocity = Vector3.zero;
                    }

                    bool triggered = entity.Level.GetEntities(EntityTypes.ENEMY)
                        .Any(e => !e.IsDead && entity.IsEnemy(e) && e.GetLane() == entity.GetLane() && e.Position.x <= entity.Position.x + TRIGGER_DISTANCE);
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
                        ent.TakeDamage(58115310, new DamageEffectList(VanillaDamageEffects.DAMAGE_BOTH_ARMOR_AND_BODY, VanillaDamageEffects.MUTE), new EntityReferenceChain(entity));
                    }
                    // 如果超出屏幕，消失。
                    if (entity.GetBounds().min.x >= VanillaLevelExt.GetBorderX(true))
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