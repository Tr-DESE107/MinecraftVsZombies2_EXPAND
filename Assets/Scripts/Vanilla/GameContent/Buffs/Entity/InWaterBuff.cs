using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.inWater)]
    public class InWaterBuff : BuffDefinition
    {
        public InWaterBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.AddMultiplie, PROP_GRAVITY_ADDITION));
            AddModifier(new FloatModifier(EngineEntityProps.FRICTION, NumberOperator.Add, 0.15f));
            AddModifier(new FloatModifier(EngineEntityProps.GROUND_LIMIT_OFFSET, NumberOperator.Add, -100f));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var groundY = entity.GetGroundY();
            float gravityAddition = 0;
            if (entity.Position.y < groundY) 
            {
                bool willSink = entity.IsDead || entity.GetWaterInteraction() == WaterInteraction.DROWN;

                float height = entity.GetScaledSize().y;
                float thresold = 0.33333f;
                float thresoldHeight = thresold * height;
                float sinkPercentage = (groundY - entity.Position.y) / height;

                if (!willSink)
                {
                    gravityAddition = Mathf.LerpUnclamped(0, -1, sinkPercentage / thresold);
                }
                else
                {
                    if (entity.Position.y <= -thresoldHeight && !entity.IsDead)
                    {
                        entity.Die(new DamageEffectList(VanillaDamageEffects.DROWN, VanillaDamageEffects.SELF_DAMAGE), entity, null);
                    }
                }
                float verticalFriction = Mathf.Lerp(1, 0.5f, sinkPercentage / thresold);

                var velocity = entity.Velocity;
                velocity.y *= verticalFriction;
                entity.Velocity = velocity;
            }
            buff.SetProperty(PROP_GRAVITY_ADDITION, gravityAddition);
        }
        public const string PROP_GRAVITY_ADDITION = "GravityAddition";
    }
}
