using PVZEngine;

namespace MVZ2.Vanilla.Grids
{
    public static class VanillaGridStatus
    {
        public static readonly NamespaceID alreadyTaken = Get("already_taken");
        public static readonly NamespaceID notOnStatues = Get("not_on_statues");
        public static readonly NamespaceID notOnSpawners = Get("not_on_spawners");
        public static readonly NamespaceID needLilypad = Get("need_lilypad");
        public static readonly NamespaceID notOnWater = Get("not_on_water");
        public static readonly NamespaceID notOnPlane = Get("not_on_plane");
        public static readonly NamespaceID notOnLand = Get("not_on_land");
        public static readonly NamespaceID onlyCanSleep = Get("only_can_sleep");
        public static readonly NamespaceID onlyCanMill = Get("only_can_mill");
        public static readonly NamespaceID onlyUpgrade = Get("only_upgrade");
        public static readonly NamespaceID onlyDrivenser = Get("only_drivenser");
        public static readonly NamespaceID firstAid = Get("first_aid");
        public static readonly NamespaceID notUnlocked = Get("not_unlocked");
        public static readonly NamespaceID rightOfLine = Get("right_of_line");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
