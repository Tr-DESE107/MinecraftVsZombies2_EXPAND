using MVZ2.Vanilla;
using MVZ2.Vanilla.Buffs;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(ContraptionNames.furnace)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, RechargeNames.shortTime)]
    public class Furnace : VanillaContraption
    {
        public Furnace() : base()
        {
            SetProperty(EntityProps.PLACE_SOUND, SoundID.stone);
            SetProperty(EntityProperties.SIZE, new Vector3(48, 48, 48));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            var buff = entity.Game.CreateBuff<ProductionColorBuff>();
            entity.AddBuff(buff);

            var productionTimer = new FrameTimer(entity.RNG.Next(90, 375));
            SetProductionTimer(entity, productionTimer);

            var evocationTimer = new FrameTimer(60);
            SetEvocationTimer(entity, evocationTimer);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var contraption = entity.ToContraption();
            if (contraption.IsEvoked())
            {
                EvokedUpdate(contraption);
            }
            else
            {
                ProductionUpdate(contraption);
            }
        }

        public override void Evoke(Contraption contraption)
        {
            base.Evoke(contraption);
            var evocationTimer = GetEvocationTimer(contraption);
            evocationTimer.Reset();
            contraption.SetEvoked(true);
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
        private void ProductionUpdate(Contraption contraption)
        {
            var productionTimer = GetProductionTimer(contraption);
            productionTimer.Run(contraption.GetAttackSpeed());

            var buffs = contraption.GetBuffs<ProductionColorBuff>();
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
                contraption.Produce<Redstone>();
                productionTimer.MaxFrame = 720;
                productionTimer.Reset();
            }
        }
        private void EvokedUpdate(Contraption contraption)
        {
            var evocationTimer = GetEvocationTimer(contraption);
            if (evocationTimer.Frame % 5 == 0)
            {
                contraption.Produce<Redstone>();
            }
            evocationTimer.Run();
            if (evocationTimer.Expired)
            {
                contraption.SetEvoked(false);
            }
        }
        private static readonly Color productionColor = new Color(0.5f, 0.5f, 0.5f, 0);
    }
}
