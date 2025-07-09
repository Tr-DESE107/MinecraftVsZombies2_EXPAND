using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.energyPickup)]
    public class EnergyPickup : PickupBehaviour
    {
        public EnergyPickup(string nsp, string name) : base(nsp, name)
        {
            SetProperty(PROP_TINT_MULT, Color.white);
            AddModifier(ColorModifier.Multiply(EngineEntityProps.TINT, PROP_TINT_MULT));
            AddModifier(new FloatModifier(VanillaEntityProps.SHADOW_ALPHA, NumberOperator.Multiply, PROP_SHADOW_ALPHA));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            var level = pickup.Level;
            float alpha = 1;
            float shadowAlpha = 1;
            if (pickup.IsCollected())
            {
                var collectedTime = pickup.GetCollectedTime();
                var moveTime = level.GetSecondTicks(1);
                var vanishTime = level.GetSecondTicks(1.5f);
                if (collectedTime < moveTime)
                {
                    float timePercent = collectedTime / (float)moveTime;
                    var targetPos = GetMoveTargetPosition(pickup);
                    pickup.Velocity = (targetPos - pickup.Position) * 0.2f;
                    alpha = 1;
                }
                else
                {
                    if (collectedTime == moveTime)
                    {
                        level.RemoveEnergyDelayedEntity(pickup);
                    }

                    var vanishLerp = (collectedTime - moveTime) / (float)(vanishTime - moveTime);
                    pickup.SetDisplayScale(Vector3.one * Mathf.Lerp(1, 0.5f, vanishLerp));
                    alpha = Mathf.Lerp(1, 0, vanishLerp);
                    if (collectedTime == vanishTime)
                    {
                        pickup.Remove();
                    }
                }
                shadowAlpha = 0;
            }
            var color = GetTintMultiplier(pickup);
            color.a = alpha;
            SetTintMultiplier(pickup, color);
            SetShadowAlpha(pickup, shadowAlpha);
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            entity.Velocity = Vector3.zero;
        }
        public override void PostCollect(Entity pickup)
        {
            base.PostCollect(pickup);
            pickup.Velocity = Vector3.zero;
            float value = pickup.GetEnergyValue();

            pickup.Level.AddEnergyDelayed(pickup, value);
            pickup.SetGravity(0);

            pickup.PlaySound(pickup.GetCollectSound(), Random.Range(0.95f, 1.5f));
        }
        private static Vector3 GetMoveTargetPosition(Entity entity)
        {
            var level = entity.Level;
            Vector3 slotPosition = level.GetEnergySlotEntityPosition();
            return new Vector3(slotPosition.x, slotPosition.y - COLLECTED_Z - 15, COLLECTED_Z);
        }
        public static void Disappear(Entity pickup)
        {
            pickup.Timeout = 15;
        }
        public static Color GetTintMultiplier(Entity entity) => entity.GetBehaviourField<Color>(PROP_TINT_MULT);
        public static void SetTintMultiplier(Entity entity, Color value) => entity.SetBehaviourField(PROP_TINT_MULT, value);
        public static float GetShadowAlpha(Entity entity) => entity.GetBehaviourField<float>(PROP_SHADOW_ALPHA);
        public static void SetShadowAlpha(Entity entity, float value) => entity.SetBehaviourField(PROP_SHADOW_ALPHA, value);
        private const float COLLECTED_Z = 480;
        private static readonly VanillaEntityPropertyMeta<Color> PROP_TINT_MULT = new VanillaEntityPropertyMeta<Color>("tint_mult");
        private static readonly VanillaEntityPropertyMeta<float> PROP_SHADOW_ALPHA = new VanillaEntityPropertyMeta<float>("shadow_alpha");
    }
}