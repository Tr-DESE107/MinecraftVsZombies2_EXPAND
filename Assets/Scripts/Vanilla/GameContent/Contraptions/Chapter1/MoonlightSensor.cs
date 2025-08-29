﻿using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.moonlightSensor)]
    public class MoonlightSensor : ContraptionBehaviour
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
            entity.PlaySound(VanillaSoundID.sparkle);
            entity.SetAnimationBool("Sparks", true);
        }
        public static void Upgrade(Entity entity)
        {
            var timer = GetUpgradeTimer(entity);
            timer?.Reset();
            SetUpgraded(entity, true);
            entity.RemoveBuffs<MoonlightSensorLaunchingBuff>();
            entity.PlaySound(VanillaSoundID.screw);
        }
        public static FrameTimer GetProductionTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(ID, PROP_PRODUCTION_TIMER);
        }
        public static void SetProductionTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourField(ID, PROP_PRODUCTION_TIMER, timer);
        }
        public static FrameTimer GetUpgradeTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(ID, PROP_UPGRADE_TIMER);
        }
        public static void SetUpgradeTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourField(ID, PROP_UPGRADE_TIMER, timer);
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
            return entity.GetBehaviourField<bool>(ID, PROP_UPGRADED);
        }
        public static void SetUpgraded(Entity entity, bool value)
        {
            entity.SetBehaviourField(ID, PROP_UPGRADED, value);
        }
        private void ProductionUpdate(Entity entity)
        {
            var productionTimer = GetProductionTimer(entity);
            productionTimer.Run(entity.GetProduceSpeed());
            if (entity.Level.IsNoEnergy())
            {
                productionTimer.Frame = productionTimer.MaxFrame;
            }
            if (productionTimer.Expired)
            {
                var energyDirection = entity.IsFriendlyEntity() ? 1 : -1;
                entity.Level.AddEnergy(1 * energyDirection);
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
        private static readonly NamespaceID ID = VanillaContraptionID.moonlightSensor;
        public static readonly VanillaEntityPropertyMeta<bool> PROP_UPGRADED = new VanillaEntityPropertyMeta<bool>("Upgraded");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_UPGRADE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("UpgradeTimer");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_PRODUCTION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ProductionTimer");
    }
}
