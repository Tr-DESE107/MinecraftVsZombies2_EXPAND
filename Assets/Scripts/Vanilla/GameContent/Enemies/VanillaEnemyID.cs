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
        public const string motherTerror = "mother_terror";
        public const string parasiteTerror = "parasite_terror";

        public const string mesmerizer = "mesmerizer";
        public const string berserker = "berserker";
        public const string dullahan = "dullahan";
        public const string hellChariot = "hell_chariot";
        public const string anubisand = "anubisand";

        public const string mutantZombie = "mutant_zombie";
        public const string megaMutantZombie = "mega_mutant_zombie";
        public const string imp = "imp";

        public const string boneWall = "bone_wall";
        public const string napstablook = "napstablook";
        public const string skeletonHorse = "skeleton_horse";
        public const string dullahanHead = "dullahan_head";
        public const string soulsand = "soulsand";
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
        public static readonly NamespaceID motherTerror = Get(VanillaEnemyNames.motherTerror);
        public static readonly NamespaceID parasiteTerror = Get(VanillaEnemyNames.parasiteTerror);

        public static readonly NamespaceID mesmerizer = Get(VanillaEnemyNames.mesmerizer);
        public static readonly NamespaceID berserker = Get(VanillaEnemyNames.berserker);
        public static readonly NamespaceID dullahan = Get(VanillaEnemyNames.dullahan);
        public static readonly NamespaceID hellChariot = Get(VanillaEnemyNames.hellChariot);
        public static readonly NamespaceID anubisand = Get(VanillaEnemyNames.anubisand);

        public static readonly NamespaceID mutantZombie = Get(VanillaEnemyNames.mutantZombie);
        public static readonly NamespaceID megaMutantZombie = Get(VanillaEnemyNames.megaMutantZombie);
        public static readonly NamespaceID imp = Get(VanillaEnemyNames.imp);

        public static readonly NamespaceID boneWall = Get(VanillaEnemyNames.boneWall);
        public static readonly NamespaceID napstablook = Get(VanillaEnemyNames.napstablook);
        public static readonly NamespaceID skeletonHorse = Get(VanillaEnemyNames.skeletonHorse);
        public static readonly NamespaceID dullahanHead = Get(VanillaEnemyNames.dullahanHead);
        public static readonly NamespaceID soulsand = Get(VanillaEnemyNames.soulsand);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
