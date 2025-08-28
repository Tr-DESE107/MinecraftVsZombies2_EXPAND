using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.zombieCloudRaindrop)]
    public class ZombieCloudRaindrop : EntityBehaviourDefinition
    {
        public ZombieCloudRaindrop(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_DISPLAY_SCALE_MULTIPLIER));
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var t = Mathf.Abs(entity.Velocity.y) / MAX_STRETCH_VELOCITY;
            var scaleMulti = new Vector3(1, Mathf.Lerp(0, 1, t), 1);
            entity.SetProperty(PROP_DISPLAY_SCALE_MULTIPLIER, scaleMulti);
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            var position = entity.Position;
            position.y = entity.Level.GetGroundY(position.x, position.y);
            if (!entity.Level.IsAirAt(position.x, position.z) && !entity.Level.IsWaterAt(position.x, position.z))
            {
                WaterStain.UpdateStain(entity.Level, position, entity);
            }
            entity.Remove();
        }
        public const float MAX_STRETCH_VELOCITY = 10;
        public static readonly VanillaEntityPropertyMeta<Vector3> PROP_DISPLAY_SCALE_MULTIPLIER = new VanillaEntityPropertyMeta<Vector3>("scale_multiplier");
    }
}