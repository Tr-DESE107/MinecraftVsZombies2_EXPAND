using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.LevelManaging;
using UnityEngine;

namespace MVZ2.GameContent
{
    [Definition(PickupNames.redstone)]
    public class Redstone : VanillaPickup
    {
        public Redstone(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            var level = pickup.Level;
            float alpha = 1;
            float shadowAlpha = 1;
            if (pickup.IsCollected())
            {
                var collectedTime = MVZ2Pickup.GetCollectedTime(pickup);
                var moveTime = level.GetSecondTicks(1);
                var vanishTime = level.GetSecondTicks(1.5f);
                if (collectedTime < moveTime)
                {
                    float timePercent = collectedTime / (float)moveTime;
                    var targetPos = GetMoveTargetPosition(pickup);
                    pickup.Velocity = (targetPos - pickup.Pos) * 0.2f;
                    alpha = 1;
                }
                else
                {
                    if (collectedTime == moveTime)
                    {
                        level.RemoveEnergyDelayedEntity(pickup);
                    }

                    var vanishLerp = (collectedTime - moveTime) / (float)(vanishTime - moveTime);
                    pickup.RenderScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, vanishLerp);
                    alpha = Mathf.Lerp(1, 0, vanishLerp);
                    if (collectedTime == vanishTime)
                    {
                        pickup.Remove();
                    }
                }
                shadowAlpha = 0;
            }
            else if (!pickup.IsImportantPickup() && pickup.Timeout < 15)
            {
                pickup.Velocity = Vector3.zero;
                alpha = pickup.Timeout / 15f;
                shadowAlpha = alpha;
            }
            var color = pickup.GetTint(true);
            color.a = alpha;
            pickup.SetTint(color);
            pickup.SetShadowAlpha(shadowAlpha);
        }
        public override void PostContactGround(Entity entity)
        {
            entity.Velocity = Vector3.zero;
        }
        public override void PostCollect(Entity pickup)
        {
            pickup.Velocity = Vector3.zero;
            float value = ENERGY_VALUE;

            var game = pickup.Level;
            if (game.Difficulty == LevelDifficulty.Easy)
            {
                value += 25;
            }
            game.AddEnergyDelayed(pickup, value);
            pickup.SetProperty(EntityProperties.GRAVITY, 0f);

            game.PlaySound(SoundID.points, pickup.Pos, Random.Range(0.95f, 1.5f));
        }
        private static Vector3 GetMoveTargetPosition(Entity entity)
        {
            var game = entity.Level;
            Vector3 slotPosition = MVZ2Level.GetEnergySlotEntityPosition();
            return new Vector3(slotPosition.x, slotPosition.y - COLLECTED_Z - 15, COLLECTED_Z);
        }
        public static void Disappear(Entity pickup)
        {
            pickup.Timeout = 15;
        }
        private const float COLLECTED_Z = 480;
        private const float ENERGY_VALUE = 25;
    }
}