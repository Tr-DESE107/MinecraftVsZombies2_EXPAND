using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Modifiers;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.aboveCloud)]
    public class AboveCloudBuff : BuffDefinition
    {
        public AboveCloudBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.AddMultiple, PROP_GRAVITY_ADDITION, VanillaModifierPriorities.WATER_GRAVITY));
            AddModifier(new FloatModifier(EngineEntityProps.FRICTION, NumberOperator.Multiply, PROP_FRICTION_MULTI));
            AddModifier(new FloatModifier(EngineEntityProps.GROUND_LIMIT_OFFSET, NumberOperator.Add, -1000f));
            AddModifier(new BooleanModifier(VanillaEnemyProps.HARMLESS, PROP_FALLING));
            AddModifier(new BooleanModifier(VanillaEntityProps.SHADOW_HIDDEN, PROP_FALLING));
        }
        public override void PostRemove(Buff buff)
        {
            base.PostRemove(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            CheckInteractionCallback(entity, buff, false);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            UpdateInCloud(entity, buff);
        }
        private void UpdateInCloud(Entity entity, Buff buff)
        {
            float frictionMulti = 1;
            float gravityAddition = 0;
            bool falling = false;

            var groundY = entity.GetGroundY();
            bool insideCloud = entity.Position.y < groundY;
            if (insideCloud)
            {
                // 低于地面
                var interaction = entity.GetAirInteraction();
                frictionMulti = 0.2f;
                falling = entity.IsDead || interaction == AirInteraction.FALL_OFF;
                if (interaction == AirInteraction.REMOVE)
                {
                    // 移除
                    RemoveInteraction(entity);
                }
                else if (falling)
                {
                    // 跌落
                    FallInteraction(entity);
                }
                else
                {
                    // 漂浮
                    FloatInteraction(entity, out gravityAddition);
                }
            }
            buff.SetProperty(PROP_FRICTION_MULTI, frictionMulti);
            buff.SetProperty(PROP_GRAVITY_ADDITION, gravityAddition);
            buff.SetProperty(PROP_FALLING, falling);
            CheckInteractionCallback(entity, buff, insideCloud);
        }
        private void RemoveInteraction(Entity entity)
        {
            entity.PlayAirSplashEffect();
            entity.PlayAirSplashSound();
            if (entity.Type == EntityTypes.ENEMY)
            {
                entity.Neutralize();
            }
            entity.Remove();
            TriggerAirInteraction(entity, AirInteraction.ACTION_REMOVE);
        }
        private void FallInteraction(Entity entity)
        {
            if (entity.Position.y <= FALL_OFF_Y && !entity.IsDead)
            {
                entity.Die(new DamageEffectList(VanillaDamageEffects.FALL_OFF, VanillaDamageEffects.SELF_DAMAGE, VanillaDamageEffects.REMOVE_ON_DEATH), entity, null);
            }
        }
        private void FloatInteraction(Entity entity, out float gravityAddition)
        {
            var groundY = entity.GetGroundY();
            float height = entity.GetScaledSize().y;
            float sinkPercentage = (groundY - entity.Position.y) / height;
            var t = sinkPercentage / FLOAT_THRESOLD;
            float verticalFriction = Mathf.Lerp(1, 0.5f, t);

            gravityAddition = Mathf.LerpUnclamped(0, -1, t);

            var velocity = entity.Velocity;
            velocity.y *= verticalFriction;
            entity.Velocity = velocity;
        }
        private void CheckInteractionCallback(Entity entity, Buff buff, bool inside)
        {
            if (inside != buff.GetProperty<bool>(PROP_INSIDE_CLOUD))
            {
                buff.SetProperty<bool>(PROP_INSIDE_CLOUD, inside);
                entity.PlayAirSplashSound();
                if (inside)
                {
                    TriggerAirInteraction(entity, AirInteraction.ACTION_ENTER);
                }
                else
                {
                    TriggerAirInteraction(entity, AirInteraction.ACTION_EXIT);
                }
            }
        }
        private void TriggerAirInteraction(Entity entity, int action)
        {
            var callbackParam = new VanillaLevelCallbacks.WaterInteractionParams()
            {
                entity = entity,
                action = action
            };
            entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_AIR_INTERACTION, callbackParam, action);
        }
        public const float FLOAT_THRESOLD = 0.3333333f;
        public const float FALL_OFF_Y = -600f;
        public static readonly VanillaBuffPropertyMeta<float> PROP_FRICTION_MULTI = new VanillaBuffPropertyMeta<float>("friction_multi");
        public static readonly VanillaBuffPropertyMeta<float> PROP_GRAVITY_ADDITION = new VanillaBuffPropertyMeta<float>("GravityAddition");
        public static readonly VanillaBuffPropertyMeta<bool> PROP_FALLING = new VanillaBuffPropertyMeta<bool>("falling");
        public static readonly VanillaBuffPropertyMeta<bool> PROP_INSIDE_CLOUD = new VanillaBuffPropertyMeta<bool>("inside_cloud");
    }
}
