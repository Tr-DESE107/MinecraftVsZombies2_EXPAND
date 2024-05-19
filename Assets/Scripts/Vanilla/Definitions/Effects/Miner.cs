using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent
{
    public class Miner : VanillaEffect
    {
        #region 公有方法
        public override void Init(Entity entity)
        {
            var timer = new FrameTimer(START_TIME);
            SetProductTimer(entity, timer);
        }
        public override void Update(Entity entity)
        {
            if (entity.Game.IsNoProduction())
            {
                SetOpen(entity, false);
            }

            if (IsOpen(entity))
            {
                var timer = GetProductTimer(entity);
                timer.Run();
                if (timer.Expired)
                {
                    Produce(entity);
                    timer.Frame = PRODUCE_TIME;
                }
            }
        }
        public static bool IsOpen(Entity entity)
        {
            return entity.GetProperty<bool>(PROP_IS_OPEN);
        }
        public static void SetOpen(Entity entity, bool value)
        {
            entity.SetProperty(PROP_IS_OPEN, value);
        }
        public static FrameTimer GetProductTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>(PROP_PRODUCE_TIMER);
        }
        public static void SetProductTimer(Entity entity, FrameTimer value)
        {
            entity.SetProperty(PROP_PRODUCE_TIMER, value);
        }
        #endregion

        #region 私有方法
        private static void Produce(Entity entity)
        {
            float xSpeed;
            float maxSpeed = 1.6f;
            Vector3 position = entity.Pos;
            position = new Vector3(position.x, 0, position.y - 16);

            var level = entity.Game;
            var rng = entity.RNG;
            if (position.x <= level.GetBorderX(false) + 150)
            {
                xSpeed = rng.Next(0, maxSpeed);
            }
            else if (position.x >= level.GetBorderX(true) - 150)
            {
                xSpeed = rng.Next(-maxSpeed, 0);
            }
            else
            {
                xSpeed = rng.Next(-maxSpeed, maxSpeed);
            }
            Vector3 dropVelocity = new Vector3(xSpeed, 7, 0);
            var redstoneDef = level.GetEntityDefinition<Redstone>();
            var redstone = level.Spawn(redstoneDef, position, entity);
            redstone.Velocity = dropVelocity;

            level.PlaySound(SoundID.throwSound, entity.Pos);
        }
        #endregion

        #region 属性字段
        public const string PROP_IS_OPEN = "isOpen";
        public const string PROP_PRODUCE_TIMER = "produceTimer";
        public const int START_TIME = 120;
        public const int PRODUCE_TIME = 300;
        #endregion
    }
}