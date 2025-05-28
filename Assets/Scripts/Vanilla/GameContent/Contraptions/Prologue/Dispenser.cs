using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.dispenser)]
    public class Dispenser : DispenserFamily
    {
        public Dispenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
            var evocationTimer = new FrameTimer(120);
            SetEvocationTimer(entity, evocationTimer);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                ShootTick(entity);
                return;
            }

            EvokedUpdate(entity);
        }

        // 核心修改：添加随机发射逻辑
        public override Entity Shoot(Entity entity)
        {
            if (entity.RNG.Next(6) == 0)
            {
                var param = entity.GetShootParams();
                param.projectileID = VanillaProjectileID.purpleArrow;
                param.damage *= 4;
                entity.TriggerAnimation("Shoot");
                return entity.ShootProjectile(param);
            }
            return base.Shoot(entity);
        }


        private void EvokedUpdate(Entity entity)
        {
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Run();
            if (evocationTimer.PassedInterval(2))
            {
                // 直接调用基类方法
                var projectile = base.Shoot(entity);
                projectile.Velocity *= 2;
            }
            if (evocationTimer.Expired)
            {
                entity.SetEvoked(false);
                var shootTimer = GetShootTimer(entity);
                shootTimer.Reset();
            }
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Reset();
            entity.SetEvoked(true);
        }

        public static FrameTimer GetEvocationTimer(Entity entity) =>
            entity.GetBehaviourField<FrameTimer>(ID, PROP_EVOCATION_TIMER);

        public static void SetEvocationTimer(Entity entity, FrameTimer timer) =>
            entity.SetBehaviourField(ID, PROP_EVOCATION_TIMER, timer);

        private static readonly NamespaceID ID = VanillaContraptionID.dispenser;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_EVOCATION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("EvocationTimer");
    }
}