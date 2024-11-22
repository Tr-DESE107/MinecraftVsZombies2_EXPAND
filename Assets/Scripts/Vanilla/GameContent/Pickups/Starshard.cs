using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [Definition(VanillaPickupNames.starshard)]
    public class Starshard : VanillaPickup
    {
        public Starshard(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity pickup)
        {
            base.Init(pickup);
            pickup.Level.PlaySound(VanillaSoundID.starshardAppear);

            var rng = pickup.RNG;
            var angle = rng.Next(0, 360f);
            var x = Mathf.Sin(Mathf.Deg2Rad * angle);
            var z = Mathf.Cos(Mathf.Deg2Rad * angle);
            pickup.Velocity = new Vector3(x, 0, z) * 5;

            pickup.SetModelProperty("Ring1Rotation", new Vector3(rng.Next(0, 360f), rng.Next(0, 360f), rng.Next(0, 360f)));
            pickup.SetModelProperty("Ring2Rotation", new Vector3(rng.Next(0, 360f), rng.Next(0, 360f), rng.Next(0, 360f)));
            pickup.SetSortingOrder(9999);
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            var level = pickup.Level;
            float alpha = 1;
            if (pickup.IsCollected())
            {
                var collectedTime = pickup.GetCollectedTime();
                var moveTime = level.GetSecondTicks(1);
                var vanishTime = level.GetSecondTicks(1.5f);
                if (collectedTime < moveTime)
                {
                    var targetPos = GetMoveTargetPosition(pickup);
                    pickup.Velocity = (targetPos - pickup.Position) * 0.2f;
                    alpha = 1;
                }
                else
                {
                    var vanishLerp = (collectedTime - moveTime) / (float)(vanishTime - moveTime);
                    pickup.RenderScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, vanishLerp);
                    alpha = Mathf.Lerp(1, 0, vanishLerp);
                    if (collectedTime == vanishTime)
                    {
                        pickup.Remove();
                    }
                }
            }
            else
            {
                var velocity = pickup.Velocity;
                if (pickup.Position.z < GetMinZ() && velocity.z < 0 || pickup.Position.z > GetMaxZ() && velocity.z > 0)
                {
                    velocity.z *= -1;
                }
                if (pickup.Position.x < GetMinX() && velocity.x < 0 || pickup.Position.x > GetMaxX() && velocity.x > 0)
                {
                    velocity.x *= -1;
                }
                pickup.Velocity = velocity;
                if (!pickup.IsImportantPickup() && pickup.Timeout < 150)
                {
                    alpha = (Mathf.Cos((1 - pickup.Timeout / 150f) * 10 * 2 * Mathf.PI) + 1) * 0.5f;
                }
            }
            var color = pickup.GetTint(true);
            color.a = alpha;
            pickup.SetTint(color);
        }

        public override bool? PreCollect(Entity pickup)
        {
            if (pickup.Level.GetStarshardCount() >= pickup.Level.GetStarshardSlotCount())
            {
                pickup.PlaySound(VanillaSoundID.buzzer);
                return false;
            }
            return base.PreCollect(pickup);
        }
        public override void PostCollect(Entity pickup)
        {
            base.PostCollect(pickup);
            pickup.Velocity = Vector3.zero;

            var level = pickup.Level;
            level.AddStarshardCount(1);
            pickup.SetGravity(0);

            pickup.PlaySound(pickup.GetCollectSound());
        }
        private static Vector3 GetMoveTargetPosition(Entity entity)
        {
            var level = entity.Level;
            Vector3 slotPosition = level.GetStarshardEntityPosition();
            return new Vector3(slotPosition.x, slotPosition.y - COLLECTED_Z - 15, COLLECTED_Z);
        }
        public static void Disappear(Entity pickup)
        {
            pickup.Timeout = 15;
        }
        public static float GetMinX()
        {
            return VanillaLevelExt.GetPickupBorderX(false) + 10;
        }
        public static float GetMaxX()
        {
            return VanillaLevelExt.GetPickupBorderX(true) - 10;
        }
        public static float GetMinZ()
        {
            return 40;
        }
        public static float GetMaxZ()
        {
            return VanillaLevelExt.SCREEN_HEIGHT - 120;
        }
        public override NamespaceID GetModelID(LevelEngine level)
        {
            var areaID = level.AreaID;
            var modelId = new NamespaceID(areaID.spacename, $"starshard.{areaID.path}").ToModelID(EngineModelID.TYPE_ENTITY);
            if (Global.Game.GetModelMeta(modelId) == null)
            {
                modelId = new NamespaceID(areaID.spacename, $"starshard.default").ToModelID(EngineModelID.TYPE_ENTITY);
            }
            return modelId;
        }
        private const float COLLECTED_Z = 0;
    }
}