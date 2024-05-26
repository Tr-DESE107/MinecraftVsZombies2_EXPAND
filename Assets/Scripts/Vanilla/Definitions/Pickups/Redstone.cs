using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent
{
    [Definition(PickupNames.redstone)]
    public class Redstone : VanillaPickup
    {
        public Redstone(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            var pickup = entity.ToPickup();
            var level = pickup.Game;
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
            else if (!MVZ2Pickup.IsImportant(pickup) && pickup.Timeout < 15)
            {
                pickup.Velocity = Vector3.zero;
                alpha = pickup.Timeout / 15f;
                shadowAlpha = alpha;
            }
            var color = entity.GetTint(true);
            color.a = alpha;
            entity.SetTint(color);
            pickup.ShadowAlpha = shadowAlpha;
        }
        public override void PostContactGround(Entity entity)
        {
            entity.Velocity = Vector3.zero;
        }
        public override void PostCollect(Pickup pickup)
        {
            pickup.Velocity = Vector3.zero;
            float value = ENERGY_VALUE;

            var game = pickup.Game;
            if (game.Difficulty == GameDifficulty.Easy)
            {
                value += 25;
            }
            game.AddEnergyDelayed(pickup, value);
            pickup.SetProperty(EntityProperties.GRAVITY, 0f);

            game.PlaySound(SoundID.points, pickup.Pos, Random.Range(0.95f, 1.5f));
        }
        private static Vector3 GetMoveTargetPosition(Entity entity)
        {
            var game = entity.Game;
            Vector3 slotPosition = MVZ2Game.GetEnergySlotEntityPosition();
            return new Vector3(slotPosition.x, slotPosition.y - COLLECTED_Z - 15, COLLECTED_Z);
        }
        public static void Disappear(Pickup pickup)
        {
            pickup.Timeout = 15;
        }
        private const float COLLECTED_Z = 480;
        private const float ENERGY_VALUE = 25;
    }
}