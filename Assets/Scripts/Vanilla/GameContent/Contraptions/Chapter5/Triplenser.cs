using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.triplenser)]
    public class Triplenser : DispenserFamily
    {
        public Triplenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                ShootTick(entity);
                return;
            }
        }
        public override void OnShootTick(Entity entity)
        {
            var lane = entity.GetLane();
            var maxLane = entity.Level.GetMaxLaneCount();
            int makeupCount = 0;
            for (int i = lane - 1; i <= lane + 1; i++)
            {
                var bullet = Shoot(entity);
                if (i < 0 || i >= maxLane)
                {
                    makeupCount++;
                    bullet.Velocity *= 1 + (MAKE_UP_VELOCITY_MULTIPLIER_INCREAMENT * makeupCount);
                }
                else if (i != lane)
                {
                    bullet.StartChangingLane(i);
                }
            }
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
        }
        protected override Detector GetDetector()
        {
            return new DispenserDetector()
            {
                ignoreHighEnemy = true,
                innerLaneExpansion = 1,
                outerLaneExpansion = 1,
            };
        }
        public const float MAKE_UP_VELOCITY_MULTIPLIER_INCREAMENT = 0.2f;
    }
}
