using MVZ2.Vanilla;
using PVZEngine;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace MVZ2.GameContent
{
    public class Miner : VanillaEffect
    {
        #region 公有方法
        public override void Init(Entity entity)
        {
            var timer = new FrameTimer(START_TIME);
            SetProductTimer(entity, timer);
            entity.ShadowVisible = true;
        }
        public override void Update(Entity entity)
        {
            if (!entity.Game.IsNoProduction())
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