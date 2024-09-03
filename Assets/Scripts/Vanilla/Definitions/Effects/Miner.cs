using MVZ2.Vanilla;
using PVZEngine.LevelManaging;
using Tools;

namespace MVZ2.GameContent.Effects
{
    [Definition(EffectNames.miner)]
    public class Miner : VanillaEffect
    {
        #region 公有方法
        public Miner(string nsp, string name) : base(nsp, name)
        {

        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var timer = new FrameTimer(START_TIME);
            SetProductTimer(entity, timer);
            entity.SetShadowHidden(false);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (!entity.Level.IsNoProduction())
            {
                var timer = GetProductTimer(entity);
                timer.Run();
                if (timer.Expired)
                {
                    entity.Produce<Redstone>();
                    timer.MaxFrame = PRODUCE_TIME;
                    timer.Reset();
                }
                entity.SetAnimationBool("Open", true);
            }
            else
            {
                entity.SetAnimationBool("Open", false);
            }
        }
        public static FrameTimer GetProductTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>(PROP_PRODUCE_TIMER);
        }
        public static void SetProductTimer(Entity entity, FrameTimer value)
        {
            entity.SetProperty(PROP_PRODUCE_TIMER, value);
        }
        #endregion

        #region 属性字段
        public const string PROP_IS_OPEN = "isOpen";
        public const string PROP_PRODUCE_TIMER = "produceTimer";
        public const int START_TIME = 120;
        public const int PRODUCE_TIME = 300;
        #endregion
    }
}