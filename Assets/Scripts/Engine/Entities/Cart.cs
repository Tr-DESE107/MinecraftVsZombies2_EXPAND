using System.Linq;
using UnityEngine;

namespace PVZEngine
{
    public sealed class Cart : Entity
    {
        public Cart(Game level, int id, int seed) : base(level, id, seed)
        {
            SetFaction(Game.Option.LeftFaction);
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            switch (State)
            {
                default:
                    bool triggered = Game.GetEntities(EntityTypes.ENEMY).Any(e => !e.IsDead && IsEnemy(e) && e.GetLane() == GetLane() && e.Pos.x <= Pos.x + TRIGGER_DISTANCE);
                    if (triggered)
                    {
                        StartEngine();
                    }
                    break;
                case CartStates.TRIGGERED:
                    // 获取所有接触到的僵尸。
                    foreach (Entity entity in Game.GetEntities().Where(e => CanCrush(e)))
                    {
                        // 碰到小车的僵尸受到伤害。
                        entity.TakeDamage(58115310, new DamageEffectList(DamageFlags.DAMAGE_BOTH_ARMOR_AND_BODY), new EntityReference(this));
                    }

                    // 如果超出屏幕，消失。
                    if (Pos.x - 0.28f >= Game.GetBorderX(true))
                    {
                        Remove();
                    }
                    break;
            }
        }
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

        public bool CanCrush(Entity entity)
        {
            if (entity == null)
                return false;
            var bouns = GetBounds();
            return !(entity is Boss) &&
            IsEnemy(entity) &&
            entity.IsActiveEntity() &&
            entity.GetLane() == GetLane() &&
            entity.Pos.x >= bouns.min.x &&
            entity.Pos.x <= bouns.max.x;
        }

        private void StartEngine()
        {
            State = CartStates.TRIGGERED;
            Velocity = Vector3.right * 2;
        }

        public override int Type => EntityTypes.CART;
        public int State { get; set; }
        private const float TRIGGER_DISTANCE = 0.28f;
    }

    public static class CartStates
    {
        public const int IDLE = 0;
        public const int TRIGGERED = 1;
    }
}