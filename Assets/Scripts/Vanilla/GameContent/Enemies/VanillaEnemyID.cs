using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Enemies
{
    public static class VanillaEnemyNames
    {
        public const string zombie = "zombie";
        public const string leatherCappedZombie = "leather_capped_zombie";
        public const string ironHelmettedZombie = "iron_helmetted_zombie";
        public const string flagZombie = "flag_zombie";

        public const string skeleton = "skeleton";
        public const string gargoyle = "gargoyle";
        public const string ghost = "ghost";
    }
    public static class VanillaEnemyID
    {
        public static readonly NamespaceID zombie = Get(VanillaEnemyNames.zombie);
        public static readonly NamespaceID leatherCappedZombie = Get(VanillaEnemyNames.leatherCappedZombie);
        public static readonly NamespaceID ironHelmettedZombie = Get(VanillaEnemyNames.ironHelmettedZombie);
        public static readonly NamespaceID flagZombie = Get(VanillaEnemyNames.flagZombie);

        public static readonly NamespaceID skeleton = Get(VanillaEnemyNames.skeleton);
        public static readonly NamespaceID gargoyle = Get(VanillaEnemyNames.gargoyle);
        public static readonly NamespaceID ghost = Get(VanillaEnemyNames.ghost);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
