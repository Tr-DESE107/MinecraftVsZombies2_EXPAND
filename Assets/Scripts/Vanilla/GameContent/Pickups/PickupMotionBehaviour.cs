using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.pickupMotion)]
    public class PickupMotionBehaviour : EntityBehaviourDefinition
    {
        public PickupMotionBehaviour(string nsp, string name) : base(nsp, name)
        {
            AddModifier(ColorModifier.Multiply(EngineEntityProps.TINT, PROP_TINT_MULT));
            AddModifier(new BooleanModifier(VanillaEntityProps.SHADOW_HIDDEN, PROP_SHADOW_HIDDEN));
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            var level = pickup.Level;
            float alpha = 1;
            bool shadowHidden = false;
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
                        level.RemoveDelayedMoney(pickup);
                    }

                    var vanishLerp = (collectedTime - moveTime) / (float)(vanishTime - moveTime);
                    pickup.SetDisplayScale(Vector3.one * Mathf.Lerp(1, 0.5f, vanishLerp));
                    alpha = Mathf.Lerp(1, 0, vanishLerp);
                    if (collectedTime == vanishTime)
                    {
                        pickup.Remove();
                    }
                }
                shadowHidden = true;

                pickup.SetSortingOrder(9999);
                if (pickup.GetPickupDestination() == PickupDestination.MONEY)
                {
                    pickup.SetSortingLayer(SortingLayers.money);
                }
                else
                {
                    pickup.SetSortingLayer(SortingLayers.frontUI);
                }
            }
            var color = GetTintMultiplier(pickup);
            color.a = alpha;
            SetTintMultiplier(pickup, color);
            SetShadowAlpha(pickup, shadowHidden);
        }
        private static Vector3 GetMoveTargetPosition(Entity entity)
        {
            Vector3 slotPosition;
            float targetZ;
            var level = entity.Level;
            var dest = entity.GetPickupDestination();
            switch (dest)
            {
                case PickupDestination.MONEY:
                    slotPosition = level.GetMoneyPanelEntityPosition();
                    targetZ = COLLECTED_Z_MONEY;
                    break;
                default:
                    slotPosition = level.GetEnergySlotEntityPosition();
                    targetZ = COLLECTED_Z_ENERGY;
                    break;
            }
            return GetMoveTargetPosition(slotPosition, targetZ);
        }
        private static Vector3 GetMoveTargetPosition(Vector3 target, float targetZ)
        {
            return new Vector3(target.x, target.y - targetZ - 15, targetZ);
        }
        public static Color GetTintMultiplier(Entity entity) => entity.GetBehaviourField<Color>(PROP_TINT_MULT);
        public static void SetTintMultiplier(Entity entity, Color value) => entity.SetBehaviourField(PROP_TINT_MULT, value);
        public static bool IsShadowHidden(Entity entity) => entity.GetBehaviourField<bool>(PROP_SHADOW_HIDDEN);
        public static void SetShadowAlpha(Entity entity, bool value) => entity.SetBehaviourField(PROP_SHADOW_HIDDEN, value);
        private static readonly VanillaEntityPropertyMeta<Color> PROP_TINT_MULT = new VanillaEntityPropertyMeta<Color>("tint_mult", Color.white);
        private static readonly VanillaEntityPropertyMeta<bool> PROP_SHADOW_HIDDEN = new VanillaEntityPropertyMeta<bool>("shadow_hidden");
        private const float COLLECTED_Z_MONEY = -100;
        private const float COLLECTED_Z_ENERGY = 480;
    }
}