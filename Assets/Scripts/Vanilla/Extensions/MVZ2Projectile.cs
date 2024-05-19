using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Projectile
    {
        public static bool WillDestroyOutsideLawn(this Projectile projectile)
        {
            return !projectile.GetProperty<bool>(PROP_NO_DESTROY_OUTSIDE_LAWN);
        }
        public const string PROP_NO_DESTROY_OUTSIDE_LAWN = "noDestroyOutsideLawn";
    }
}
