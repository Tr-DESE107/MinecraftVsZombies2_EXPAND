using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.furnace)]
    public class Furnace : ContraptionBehaviour
    {
        public Furnace(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            entity.AddBuff<ProductionColorBuff>();

            var productionTimer = new FrameTimer(entity.RNG.Next(90, 375));
            SetProductionTimer(entity, productionTimer);

            var evocationTimer = new FrameTimer(EVOCATION_DURATION);
            SetEvocationTimer(entity, evocationTimer);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (entity.IsEvoked())
            {
                EvokedUpdate(entity);
            }
            else if (entity.Level.IsIZombie())
            {
                IZombieUpdate(entity);
            }
            else
            {
                ProductionUpdate(entity);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            bool frozen = entity.IsAIFrozen();
            if (entity.Level.IsIZombie())
            {
                if (GetDroppedRedstones(entity) >= GetRedstonesToDrop(entity, 0))
                {
                    frozen = true;
                }
            }
            entity.SetAnimationBool("Frozen", frozen);
            entity.SetLightSource(!frozen);
        }

        public override void PostDeath(Entity entity, DeathInfo deathInfo)
        {
            base.PostDeath(entity, deathInfo);
            if (entity.Level.IsIZombie())
            {
                var redstonesToDrop = GetRedstonesToDrop(entity, 0);
                DropRedstones(entity, redstonesToDrop);
            }
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Reset();
            entity.SetEvoked(true);
        }
        public static FrameTimer GetProductionTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_PRODUCTION_TIMER);
        public static void SetProductionTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_PRODUCTION_TIMER, timer);
        public static FrameTimer GetEvocationTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_EVOCATION_TIMER);
        public static void SetEvocationTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_EVOCATION_TIMER, timer);
        public static int GetDroppedRedstones(Entity entity) => entity.GetBehaviourField<int>(PROP_DROPPED_REDSTONES);
        public static void SetDroppedRedstones(Entity entity, int value) => entity.SetBehaviourField(PROP_DROPPED_REDSTONES, value);
        private void ProductionUpdate(Entity entity)
        {
            var productionTimer = GetProductionTimer(entity);
            productionTimer.Run(entity.GetProduceSpeed());
            if (entity.Level.IsNoEnergy())
            {
                productionTimer.Frame = productionTimer.MaxFrame;
            }

            var buffs = entity.GetBuffs<ProductionColorBuff>();
            foreach (var buff in buffs)
            {
                var color = buff.GetProperty<Color>(ProductionColorBuff.PROP_COLOR);
                float colorValue = color.a;
                if (productionTimer.Frame < 30)
                {
                    colorValue = Mathf.Lerp(1, 0, productionTimer.Frame / 30f);
                }
                else
                {
                    colorValue = Mathf.Max(0, colorValue - 1 / 30f);
                }
                color.r = 1;
                color.g = 1;
                color.b = 1;
                color.a = colorValue;
                buff.SetProperty(ProductionColorBuff.PROP_COLOR, color);
            }
            if (productionTimer.Expired)
            {
                if (entity.IsFriendlyEntity())
                {
                    entity.Produce(VanillaPickupID.redstone);
                    entity.PlaySound(VanillaSoundID.throwSound);
                }
                else
                {
                    entity.Level.AddEnergy(-25);
                }
                productionTimer.ResetTime(720);
            }
        }

        #region 我是僵尸
        private void IZombieUpdate(Entity entity)
        {
            var hp = entity.Health;
            if (!entity.IsFriendlyEntity())
            {
                hp = 0;
            }
            var redstonesToDrop = GetRedstonesToDrop(entity, hp);
            DropRedstones(entity, redstonesToDrop);
        }
        private void DropRedstones(Entity entity, int targetCount)
        {
            var droppedRedstones = GetDroppedRedstones(entity);
            var count = targetCount - droppedRedstones;
            if (count <= 0)
                return;
            for (int i = 0; i < count; i++)
            {
                entity.Produce(VanillaPickupID.redstone);
            }
            SetDroppedRedstones(entity, targetCount);
        }
        private int GetRedstonesToDrop(Entity entity, float hp)
        {
            var totalCount = entity.Level.GetIZFurnaceRedstoneCount();
            var hpPerRedstone = entity.GetMaxHealth() / totalCount;
            return Mathf.FloorToInt(totalCount - hp / hpPerRedstone);
        }
        #endregion
        private void EvokedUpdate(Entity entity)
        {
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Run();
            if (evocationTimer.PassedInterval(EVOCATION_INTERVAL))
            {
                entity.Produce(VanillaPickupID.redstone);
                entity.PlaySound(VanillaSoundID.potion);
            }
            if (evocationTimer.Expired)
            {
                entity.SetEvoked(false);
            }
        }
        public const int EVOCATION_INTERVAL = 5;
        public const int EVOCATION_REDSTONES = 6;
        public const int EVOCATION_DURATION = EVOCATION_INTERVAL * EVOCATION_REDSTONES;
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_EVOCATION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("EvocationTimer");
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_PRODUCTION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ProductionTimer");
        private static readonly VanillaEntityPropertyMeta<int> PROP_DROPPED_REDSTONES = new VanillaEntityPropertyMeta<int>("LastRemaionRedstones");
    }
}
