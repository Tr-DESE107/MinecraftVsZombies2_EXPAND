using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    public static class ProjectileProps
    {
        public const string PIERCING = "piercing";
        public const string POINT_TO_DIRECTION = "pointToDirection";
        public const string NO_DESTROY_OUTSIDE_LAWN = "noDestroyOutsideLawn";
        public const string CAN_HIT_SPAWNER = "canHitSpawner";
        public const string COLLIDING_ENTITIES = "collidingEntities";
        public const string NO_HIT_ENTITIES = "noHitEntities";

        public static bool PointsTowardDirection(this Entity entity)
        {
            return entity.GetProperty<bool>(POINT_TO_DIRECTION);
        }
    }
}
