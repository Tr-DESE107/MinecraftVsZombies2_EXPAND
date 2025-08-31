using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Enemy.waterStainSlide)]
    public class WaterStainSlideBuff : BuffDefinition
    {
        public WaterStainSlideBuff(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_UPDATE, PostUpdateCallback, filter: EntityTypes.ENEMY);
            AddModifier(new FloatModifier(EngineEntityProps.FRICTION, NumberOperator.Multiply, 0.01f));
        }
        private void PostUpdateCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            if (entity.IsDead)
                return;
            if (!entity.IsOnGround)
                return;
            if (!entity.HasBuff(VanillaBuffID.Enemy.waterStainSlide))
                return;
            if (!entity.IsAboveLand())
                return;
            entity.Position += entity.GetFacingDirection() * SLIDE_SPEED;
        }
        public const float SLIDE_SPEED = 0.5f;
    }
}
