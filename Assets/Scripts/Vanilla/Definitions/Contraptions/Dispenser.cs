using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Contraptions
{
    public class Dispenser : DispenserFamily
    {
        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            var evocationTimer = new FrameTimer(120);
            SetEvocationTimer(entity, evocationTimer);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var contraption = entity.ToContraption();
            if (!contraption.IsEvoked())
            {
                ShootTick(entity);
                return;
            }

            EvokedUpdate(contraption);
        }

        public override void Evoke(Contraption contraption)
        {
            base.Evoke(contraption);
            var evocationTimer = GetEvocationTimer(contraption);
            evocationTimer.Reset();
            contraption.SetEvoked(true);
        }
        public static FrameTimer GetEvocationTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>("EvocationTimer");
        }
        public static void SetEvocationTimer(Entity entity, FrameTimer timer)
        {
            entity.SetProperty("EvocationTimer", timer);
        }
        private void EvokedUpdate(Contraption contraption)
        {
            var evocationTimer = GetEvocationTimer(contraption);
            if (evocationTimer.Frame % 2 == 0)
            {
                var projectile = Shoot(contraption);
                projectile.Velocity *= 2;
            }
            evocationTimer.Run();
            if (evocationTimer.Expired)
            {
                contraption.SetEvoked(false);
                var shootTimer = GetShootTimer(contraption);
                shootTimer.Reset();
            }
        }
    }
}
