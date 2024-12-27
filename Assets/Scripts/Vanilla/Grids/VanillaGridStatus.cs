using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.Vanilla.Grids
{
    public static class VanillaGridStatus
    {
        public static readonly NamespaceID alreadyTaken = Get("already_taken");
        public static readonly NamespaceID notOnStatues = Get("not_on_statues");
        public static readonly NamespaceID needLilypad = Get("need_lilypad");
        public static readonly NamespaceID notOnWater = Get("not_on_water");
        public static readonly NamespaceID notOnPlane = Get("not_on_plane");
        public static readonly NamespaceID notOnLand = Get("not_on_land");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
