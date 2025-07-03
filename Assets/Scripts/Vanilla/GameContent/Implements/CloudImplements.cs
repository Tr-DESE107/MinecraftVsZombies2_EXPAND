using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Implements
{
    public class CloudImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_UPDATE, EntityUpdateCallback);
        }
        private void EntityUpdateCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            UpdateCloud(entity);
        }
        private void UpdateCloud(Entity entity)
        {
            var interaction = entity.GetAirInteraction();
            bool aboveCloud = entity.IsAboveCloud();
            bool shouldHaveBuff = aboveCloud;
            if (shouldHaveBuff != entity.HasBuff<AboveCloudBuff>())
            {
                if (!shouldHaveBuff && interaction == AirInteraction.FALL_OFF && entity.Position.y < entity.GetGroundY() - 20f) // 掉下去的不能再回陆地
                {
                    return;
                }
                if (shouldHaveBuff)
                {
                    entity.AddBuff<AboveCloudBuff>();
                }
                else
                {
                    entity.RemoveBuffs<AboveCloudBuff>();
                }
            }
        }
    }
}