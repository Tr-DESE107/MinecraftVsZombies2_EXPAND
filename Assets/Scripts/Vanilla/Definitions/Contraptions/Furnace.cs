using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(ContraptionNames.furnace)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, RechargeNames.shortTime)]
    public class Furnace : VanillaContraption
    {
        public Furnace(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProps.PLACE_SOUND, SoundID.stone);
            SetProperty(EntityProperties.SIZE, new Vector3(48, 48, 48));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            entity.AddBuff<ProductionColorBuff>();

            var productionTimer = new FrameTimer(entity.RNG.Next(90, 375));
            SetProductionTimer(entity, productionTimer);

            var evocationTimer = new FrameTimer(60);
            SetEvocationTimer(entity, evocationTimer);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (entity.IsEvoked())
            {
                EvokedUpdate(entity);
            }
            else
            {
                ProductionUpdate(entity);
            }
        }

        public override void Evoke(Entity entity)
        {
            base.Evoke(entity);
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Reset();
            entity.SetEvoked(true);
        }
        public static FrameTimer GetProductionTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>("ProductionTimer");
        }
        public static void SetProductionTimer(Entity entity, FrameTimer timer)
        {
            entity.SetProperty("ProductionTimer", timer);
        }
        public static FrameTimer GetEvocationTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>("EvocationTimer");
        }
        public static void SetEvocationTimer(Entity entity, FrameTimer timer)
        {
            entity.SetProperty("EvocationTimer", timer);
        }
        private void ProductionUpdate(Entity entity)
        {
            var productionTimer = GetProductionTimer(entity);
            productionTimer.Run(entity.GetAttackSpeed());
            if (entity.Level.IsNoProduction())
            {
                productionTimer.Frame = productionTimer.MaxFrame;
            }

            var buffs = entity.GetBuffs<ProductionColorBuff>();
            foreach (var buff in buffs)
            {
                var color = buff.GetProperty<Color>(ProductionColorBuff.PROP_COLOR);
                float colorValue = color.r;
                if (productionTimer.Frame < 30)
                {
                    colorValue = Mathf.Lerp(1, 0, productionTimer.Frame / 30f);
                }
                else
                {
                    colorValue = Mathf.Max(0, colorValue - 1 / 30f);
                }
                color.r = colorValue;
                color.g = colorValue;
                color.b = colorValue;
                buff.SetProperty(ProductionColorBuff.PROP_COLOR, color);
            }
            if (productionTimer.Expired)
            {
                entity.Produce<Redstone>();
                productionTimer.MaxFrame = 720;
                productionTimer.Reset();
            }
        }
        private void EvokedUpdate(Entity entity)
        {
            var evocationTimer = GetEvocationTimer(entity);
            if (evocationTimer.Frame % 5 == 0)
            {
                entity.Produce<Redstone>();
            }
            evocationTimer.Run();
            if (evocationTimer.Expired)
            {
                entity.SetEvoked(false);
            }
        }
        private static readonly Color productionColor = new Color(0.5f, 0.5f, 0.5f, 0);
    }
}
