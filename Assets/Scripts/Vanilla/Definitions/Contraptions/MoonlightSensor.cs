using MVZ2.Extensions;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.moonlightSensor)]
    [EntitySeedDefinition(25, VanillaMod.spaceName, VanillaRechargeNames.shortTime)]
    public class MoonlightSensor : VanillaContraption
    {
        public MoonlightSensor(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.AddBuff<MoonlightSensorLaunchingBuff>();
            var productionTimer = new FrameTimer(PRODUCTION_INTERVAL);
            SetProductionTimer(entity, productionTimer);
            var upgradeTimer = new FrameTimer(UPGRADE_TIME);
            SetUpgradeTimer(entity, upgradeTimer);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            ProductionUpdate(entity);
            UpgradeUpdate(entity);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.UpdateShineRing();
            entity.SetAnimationBool("Upgraded", GetUpgraded(entity));
        }

        public override bool CanEvoke(Entity entity)
        {
            return base.CanEvoke(entity) && !entity.HasBuff<MoonlightSensorEvokedBuff>();
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.AddBuff<MoonlightSensorEvokedBuff>();
            Upgrade(entity);
            entity.PlaySound(SoundID.sparkle);
        }
        public static void Upgrade(Entity entity)
        {
            var timer = GetUpgradeTimer(entity);
            timer?.Reset();
            SetUpgraded(entity, true);
            entity.RemoveBuffs(entity.GetBuffs<MoonlightSensorLaunchingBuff>());
            entity.PlaySound(SoundID.screw);
        }
        public static FrameTimer GetProductionTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>("ProductionTimer");
        }
        public static void SetProductionTimer(Entity entity, FrameTimer timer)
        {
            entity.SetProperty("ProductionTimer", timer);
        }
        public static FrameTimer GetUpgradeTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>("UpgradeTimer");
        }
        public static void SetUpgradeTimer(Entity entity, FrameTimer timer)
        {
            entity.SetProperty("UpgradeTimer", timer);
        }
        public static FrameTimer GetOrCreateUpgradeTimer(Entity entity)
        {
            var timer = GetUpgradeTimer(entity);
            if (timer == null)
            {
                timer = new FrameTimer(UPGRADE_TIME);
                SetUpgradeTimer(entity, timer);
            }
            return timer;
        }
        public static bool GetUpgraded(Entity entity)
        {
            return entity.GetProperty<bool>("Upgraded");
        }
        public static void SetUpgraded(Entity entity, bool value)
        {
            entity.SetProperty("Upgraded", value);
        }
        private void ProductionUpdate(Entity entity)
        {
            var productionTimer = GetProductionTimer(entity);
            productionTimer.Run(entity.GetProduceSpeed());
            if (entity.Level.IsNoProduction())
            {
                productionTimer.Frame = productionTimer.MaxFrame;
            }
            if (productionTimer.Expired)
            {
                entity.Level.AddEnergy(1);
                productionTimer.Reset();
            }
        }
        private void UpgradeUpdate(Entity entity)
        {
            if (GetUpgraded(entity))
                return;
            var upgradeTimer = GetOrCreateUpgradeTimer(entity);
            upgradeTimer.Run();
            if (upgradeTimer.Expired)
            {
                Upgrade(entity);
            }
        }
        public const int PRODUCTION_INTERVAL = 30;
        public const int UPGRADE_TIME = 3600;
        private static readonly Color productionColor = new Color(0.5f, 0.5f, 0.5f, 0);
    }
}
