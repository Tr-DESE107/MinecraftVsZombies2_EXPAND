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
        public const string mummy = "mummy";
        public const string necromancer = "necromancer";

        public const string spider = "spider";
        public const string caveSpider = "cave_spider";
        public const string ghast = "ghast";

        public const string boneWall = "bone_wall";
        public const string napstablook = "napstablook";
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
        public static readonly NamespaceID mummy = Get(VanillaEnemyNames.mummy);
        public static readonly NamespaceID necromancer = Get(VanillaEnemyNames.necromancer);

        public static readonly NamespaceID spider = Get(VanillaEnemyNames.spider);
        public static readonly NamespaceID caveSpider = Get(VanillaEnemyNames.caveSpider);
        public static readonly NamespaceID ghast = Get(VanillaEnemyNames.ghast);

        public static readonly NamespaceID boneWall = Get(VanillaEnemyNames.boneWall);
        public static readonly NamespaceID napstablook = Get(VanillaEnemyNames.napstablook);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
