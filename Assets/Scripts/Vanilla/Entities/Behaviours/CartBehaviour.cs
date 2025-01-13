using System.Linq;
using MVZ2.GameContent.Buffs.Carts;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public abstract class CartBehaviour : VanillaEntityBehaviour
    {
        protected CartBehaviour(string nsp, string name) : base(nsp, name)
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
            var velocity = entity.Velocity;
            switch (entity.State)
            {
                default:
                    if (entity.Position.x < VanillaLevelExt.CART_TARGET_X)
                    {
                        velocity.x = 10;
                    }
                    else
                    {
                        var pos = entity.Position;
                        pos.x = VanillaLevelExt.CART_TARGET_X;
                        entity.Position = pos;
                        velocity.x = 0;
                    }

                    bool triggered = entity.Level.GetEntities(EntityTypes.ENEMY)
                        .Any(e => !e.IsDead && entity.IsHostile(e) && e.GetLane() == entity.GetLane() && e.Position.x <= entity.Position.x + TRIGGER_DISTANCE);
                    if (triggered)
                    {
                        entity.TriggerCart();
                    }
                    break;
                case VanillaEntityStates.CART_TRIGGERED:
                    velocity.x = 10;
                    // 获取所有接触到的僵尸。
                    foreach (Entity ent in entity.Level.FindEntities(e => entity.CanCartCrush(e)))
                    {
                        // 碰到小车的僵尸受到伤害。
                        ent.TakeDamage(58115310, new DamageEffectList(VanillaDamageEffects.DAMAGE_BOTH_ARMOR_AND_BODY, VanillaDamageEffects.MUTE), entity);
                    }
                    // 如果超出屏幕，消失。
                    if (entity.GetBounds().min.x >= VanillaLevelExt.GetBorderX(true))
                    {
                        entity.Remove();
                    }
                    break;
            }
            entity.Velocity = velocity;
            TurnToMoneyUpdate(entity);
        }
        private void TurnToMoneyUpdate(Entity entity)
        {
            FrameTimer timer = entity.GetTurnToMoneyTimer();
            if (timer == null)
                return;
            timer.Run();
            if (timer.Expired)
            {
                var level = entity.Level;
                var gemType = GemEffect.GemType.Ruby;
                if (level.Difficulty == VanillaDifficulties.easy)
                {
                    gemType = GemEffect.GemType.Emerald;
                }
                var gemEffect = GemEffect.SpawnGemEffect(level, gemType, entity.Position, entity, true);
                gemEffect.PlaySound(VanillaSoundID.points, 1 + (level.GetMaxLaneCount() - entity.GetLane() - 1) * 0.1f);
                level.ShowMoney();
                entity.Remove();
            }
        }
        public const float TRIGGER_DISTANCE = 28;
    }

}