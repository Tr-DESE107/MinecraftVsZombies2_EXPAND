using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class EnemyNames
    {
        public const string zombie = "zombie";
        public const string leatherCappedZombie = "leather_capped_zombie";
        public const string ironHelmettedZombie = "iron_helmetted_zombie";
        public const string flagZombie = "flag_zombie";

        public const string skeleton = "skeleton";
    }
    public static class EnemyID
    {
        public static readonly NamespaceID zombie = Get(EnemyNames.zombie);
        public static readonly NamespaceID leatherCappedZombie = Get(EnemyNames.leatherCappedZombie);
        public static readonly NamespaceID ironHelmettedZombie = Get(EnemyNames.ironHelmettedZombie);
        public static readonly NamespaceID flagZombie = Get(EnemyNames.flagZombie);

        public static readonly NamespaceID skeleton = Get(EnemyNames.skeleton);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
