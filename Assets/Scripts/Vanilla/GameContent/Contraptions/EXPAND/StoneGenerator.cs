#nullable enable  
  
using MVZ2.GameContent.Pickups;  
using MVZ2.Vanilla.Audios;  
using MVZ2.Vanilla.Contraptions;  
using MVZ2.Vanilla.Entities;  
using MVZ2.Vanilla.Properties;  
using MVZ2Logic.Level;  
using PVZEngine;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using PVZEngine.Modifiers;  
using Tools;  
using UnityEngine;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Enemies;

namespace MVZ2.GameContent.Contraptions  
{  
    [EntityBehaviourDefinition(VanillaContraptionNames.StoneGenerator)]  
    public class StoneGenerator : ContraptionBehaviour  
    {  
        public StoneGenerator(string nsp, string name) : base(nsp, name)  
        {  
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR_OFFSET));  
            AddModifier(new BooleanModifier(VanillaEntityProps.IS_LIGHT_SOURCE, BooleanOperator.And, PROP_BURNING));  
        }  
  
        public override void Init(Entity entity)  
        {  
            base.Init(entity);  
            var productionTimer = new FrameTimer(entity.RNG.Next(300, 360));  
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
            else  
            {  
                ProductionUpdate(entity);  
            }  
        }  
  
        protected override void UpdateLogic(Entity entity)  
        {  
            base.UpdateLogic(entity);  
            bool frozen = entity.IsAIFrozen();  
            if (frozen)  
            {  
                entity.SetProperty(PROP_COLOR_OFFSET, Color.clear);  
            }  
            entity.SetProperty(PROP_BURNING, !frozen);  
            entity.SetAnimationBool("Frozen", frozen);  
        }  
  
        protected override void OnEvoke(Entity entity)  
        {  
            base.OnEvoke(entity);  
            var evocationTimer = GetEvocationTimer(entity);  
            evocationTimer?.Reset();  
            entity.SetEvoked(true);  
        }  
  
        public static FrameTimer? GetProductionTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_PRODUCTION_TIMER);  
        public static void SetProductionTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_PRODUCTION_TIMER, timer);  
        public static FrameTimer? GetEvocationTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_EVOCATION_TIMER);  
        public static void SetEvocationTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_EVOCATION_TIMER, timer);

        /// <summary>  
        /// Helper: produce a blueprint pickup for a random stone contraption.  
        /// </summary>  
        // 池和权重数组（一一对应）  
        private static readonly NamespaceID[] stonePool = new NamespaceID[]
        {
        VanillaContraptionID.Stone,
        VanillaEnemyID.Cobblestone,
        };

        private static readonly int[] stonePoolWeights = new int[]
        {
        2,
        3,
        };

        private void ProduceStoneBlueprintPickup(Entity entity)
        {
            var index = entity.RNG.WeightedRandom(stonePoolWeights);
            var chosen = stonePool[index];
            var blueprintID = VanillaBlueprintID.FromEntity(chosen);

            var spawnParams = entity.GetSpawnParams();
            spawnParams.SetProperty(VanillaPickupProps.CONTENT_ID, blueprintID);
            entity.Produce(VanillaPickupID.blueprintPickup, spawnParams);
        }

        private void ProductionUpdate(Entity entity)  
        {  
            var productionTimer = GetProductionTimer(entity);  
            if (productionTimer == null)  
                return;  
            productionTimer.Run(entity.GetProduceSpeed());  
            //if (entity.Level.IsNoEnergy())  
            //{  
            //    productionTimer.Frame = productionTimer.MaxFrame;  
            //}  
  
            var color = entity.GetProperty<Color>(PROP_COLOR_OFFSET);  
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
            entity.SetProperty(PROP_COLOR_OFFSET, color);  
  
            if (productionTimer.Expired)  
            {  
                if (entity.IsFriendlyEntity())  
                {  
                    ProduceStoneBlueprintPickup(entity);  
                    entity.PlaySound(VanillaSoundID.throwSound);  
                }  
                productionTimer.ResetTime(720);  
            }  
        }  
  
        private void EvokedUpdate(Entity entity)  
        {  
            var evocationTimer = GetEvocationTimer(entity);  
            if (evocationTimer == null)  
                return;  
            evocationTimer.Run();  
            if (evocationTimer.PassedInterval(EVOCATION_INTERVAL))  
            {  
                ProduceStoneBlueprintPickup(entity);  
                entity.PlaySound(VanillaSoundID.potion);  
            }  
            if (evocationTimer.Expired)  
            {  
                entity.SetEvoked(false);  
            }  
        }  
  
        public const int EVOCATION_INTERVAL = 5;  
        public const int EVOCATION_DURATION = EVOCATION_INTERVAL * 6; // 6 blueprints total  
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_EVOCATION_TIMER = new("EvocationTimer");  
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_PRODUCTION_TIMER = new("ProductionTimer");  
        public static readonly VanillaEntityPropertyMeta<bool> PROP_BURNING = new("burning", true);  
        public static readonly VanillaBuffPropertyMeta<Color> PROP_COLOR_OFFSET = new("color_offset");  
    }  
}