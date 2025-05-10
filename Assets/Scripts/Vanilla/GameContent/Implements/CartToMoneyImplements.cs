using MVZ2.Vanilla.Entities;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Implements
{
    public class CartToMoneyImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_LEVEL_CLEAR, PostLevelClearCallback);
        }
        private void PostLevelClearCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            foreach (var cart in level.GetEntities(EntityTypes.CART))
            {
                if (cart.IsCartTriggered())
                    continue;
                cart.SetTurnToMoneyTimer(new FrameTimer(30 + 15 * (level.GetMaxLaneCount() - cart.GetLane())));
            }
        }
    }
}
