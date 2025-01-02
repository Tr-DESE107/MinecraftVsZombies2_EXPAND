using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.furnace)]
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

            var evocationTimer = new FrameTimer(60);
            SetEvocationTimer(entity, evocationTimer);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (entity.IsEvoked())
            {
                EvokedUpdate(entity);
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
            entity.SetAnimationBool("Frozen", frozen);
            entity.SetLightSource(!frozen);
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
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
            productionTimer.Run(entity.GetProduceSpeed());
            if (entity.Level.IsNoProduction())
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
                entity.Produce(VanillaPickupID.redstone);
                entity.PlaySound(VanillaSoundID.throwSound);
                productionTimer.ResetTime(720);
            }
        }
        private void EvokedUpdate(Entity entity)
        {
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Run();
            if (evocationTimer.PassedInterval(5))
            {
                entity.Produce(VanillaPickupID.redstone);
                entity.PlaySound(VanillaSoundID.potion);
            }
            if (evocationTimer.Expired)
            {
                entity.SetEvoked(false);
            }
        }
        private static readonly Color productionColor = new Color(0.5f, 0.5f, 0.5f, 0);
    }
}
