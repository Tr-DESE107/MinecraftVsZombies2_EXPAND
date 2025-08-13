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

        public const string SkeletonHead = "SkeletonHead";
        public const string ZombieHead = "ZombieHead";
        public const string RedEyeZombieHead = "RedEyeZombieHead";
        public const string HostHead = "HostHead";
        public const string FlagSkeleton = "FlagSkeleton";
        public const string MeleeSkeleton = "MeleeSkeleton";

        public const string skeleton = "skeleton";
        public const string gargoyle = "gargoyle";
        public const string ghost = "ghost";
        public const string mummy = "mummy";
        public const string necromancer = "necromancer";

        public const string KingSkeleton = "KingSkeleton";
        public const string MegaZombie = "MegaZombie";
        public const string SuperMegaZombie = "SuperMegaZombie";
        public const string EvilMage = "EvilMage";
        public const string WitherBoneWall = "WitherBoneWall";

        public const string HostZombie = "HostZombie";
        public const string BloodlustHostZombie = "BloodlustHostZombie";
        public const string SpiderQueen = "SpiderQueen";
        public const string HostMutant = "HostMutant";
        public const string HostIMP = "HostIMP";

        public const string spider = "spider";
        public const string caveSpider = "cave_spider";
        public const string ghast = "ghast";
        public const string motherTerror = "mother_terror";
        public const string parasiteTerror = "parasite_terror";

        public const string WitherSkeleton = "WitherSkeleton";
        public const string LeatherWitherSkeleton = "LeatherWitherSkeleton";
        public const string IronWitherSkeleton = "IronWitherSkeleton";
        public const string FlagWitherSkeleton = "FlagWitherSkeleton";

        public const string mesmerizer = "mesmerizer";
        public const string berserker = "berserker";
        public const string dullahan = "dullahan";
        public const string hellChariot = "hell_chariot";
        public const string anubisand = "anubisand";

        public const string NetherWarrior = "NetherWarrior";
        public const string NetherArcher = "NetherArcher";
        public const string NetherVanguard = "NetherVanguard";
        public const string AngryReverser = "AngryReverser";
        public const string RaiderSkull = "RaiderSkull";
        public const string Anubiskull = "Anubiskull";

        public const string reflectiveBarrierZombie = "reflective_barrier_zombie";
        public const string talismanZombie = "talisman_zombie";
        public const string wickedHermitZombie = "wicked_hermit_zombie";
        public const string shikaisenZombie = "shikaisen_zombie";
        public const string emperorZombie = "emperor_zombie";

        public const string MonkZombie = "MonkZombie";
        public const string LeatherMonkZombie = "LeatherMonkZombie";
        public const string IronMonkZombie = "IronMonkZombie";
        public const string FlagMonkZombie = "FlagMonkZombie";
        public const string SixQiZombie = "SixQiZombie";


        public const string HeavyGutant = "HeavyGutant";
        public const string Endermite = "Endermite";

        public const string mutantZombie = "mutant_zombie";
        public const string megaMutantZombie = "mega_mutant_zombie";
        public const string imp = "imp";

        public const string boneWall = "bone_wall";
        public const string napstablook = "napstablook";
        public const string reverseSatellite = "reverse_satellite";
        public const string skeletonHorse = "skeleton_horse";
        public const string dullahanHead = "dullahan_head";
        public const string soulsand = "soulsand";
        public const string seijaCursedDoll = "seija_cursed_doll";
        public const string bedserker = "bedserker";
        public const string skeletonWarrior = "skeleton_warrior";
        public const string skeletonMage = "skeleton_mage";
        public const string shikaisenStaff = "shikaisen_staff";

    }
    public static class VanillaEnemyID
    {
        public static readonly NamespaceID zombie = Get(VanillaEnemyNames.zombie);
        public static readonly NamespaceID leatherCappedZombie = Get(VanillaEnemyNames.leatherCappedZombie);
        public static readonly NamespaceID ironHelmettedZombie = Get(VanillaEnemyNames.ironHelmettedZombie);
        public static readonly NamespaceID flagZombie = Get(VanillaEnemyNames.flagZombie);

        public static readonly NamespaceID SkeletonHead = Get(VanillaEnemyNames.SkeletonHead);
        public static readonly NamespaceID ZombieHead = Get(VanillaEnemyNames.ZombieHead);
        public static readonly NamespaceID RedEyeZombieHead = Get(VanillaEnemyNames.RedEyeZombieHead);
        public static readonly NamespaceID HostHead = Get(VanillaEnemyNames.HostHead);
        public static readonly NamespaceID FlagSkeleton = Get(VanillaEnemyNames.FlagSkeleton);
        public static readonly NamespaceID MeleeSkeleton = Get(VanillaEnemyNames.MeleeSkeleton);

        public static readonly NamespaceID skeleton = Get(VanillaEnemyNames.skeleton);
        public static readonly NamespaceID gargoyle = Get(VanillaEnemyNames.gargoyle);
        public static readonly NamespaceID ghost = Get(VanillaEnemyNames.ghost);
        public static readonly NamespaceID mummy = Get(VanillaEnemyNames.mummy);
        public static readonly NamespaceID necromancer = Get(VanillaEnemyNames.necromancer);

        public static readonly NamespaceID EvilMage = Get(VanillaEnemyNames.EvilMage);
        public static readonly NamespaceID WitherBoneWall = Get(VanillaEnemyNames.WitherBoneWall);
        public static readonly NamespaceID KingSkeleton = Get(VanillaEnemyNames.KingSkeleton);
        public static readonly NamespaceID MegaZombie = Get(VanillaEnemyNames.MegaZombie);
        public static readonly NamespaceID SuperMegaZombie = Get(VanillaEnemyNames.SuperMegaZombie);

        public static readonly NamespaceID HostZombie = Get(VanillaEnemyNames.HostZombie);
        public static readonly NamespaceID BloodlustHostZombie = Get(VanillaEnemyNames.BloodlustHostZombie);
        public static readonly NamespaceID SpiderQueen = Get(VanillaEnemyNames.SpiderQueen);
        public static readonly NamespaceID HostMutant = Get(VanillaEnemyNames.HostMutant);
        public static readonly NamespaceID HostIMP = Get(VanillaEnemyNames.HostIMP);

        public static readonly NamespaceID spider = Get(VanillaEnemyNames.spider);
        public static readonly NamespaceID caveSpider = Get(VanillaEnemyNames.caveSpider);
        public static readonly NamespaceID ghast = Get(VanillaEnemyNames.ghast);
        public static readonly NamespaceID motherTerror = Get(VanillaEnemyNames.motherTerror);
        public static readonly NamespaceID parasiteTerror = Get(VanillaEnemyNames.parasiteTerror);

        public static readonly NamespaceID WitherSkeleton = Get(VanillaEnemyNames.WitherSkeleton);
        public static readonly NamespaceID LeatherWitherSkeleton = Get(VanillaEnemyNames.LeatherWitherSkeleton);
        public static readonly NamespaceID IronWitherSkeleton = Get(VanillaEnemyNames.IronWitherSkeleton);
        public static readonly NamespaceID FlagWitherSkeleton = Get(VanillaEnemyNames.FlagWitherSkeleton);

        public static readonly NamespaceID mesmerizer = Get(VanillaEnemyNames.mesmerizer);
        public static readonly NamespaceID berserker = Get(VanillaEnemyNames.berserker);
        public static readonly NamespaceID dullahan = Get(VanillaEnemyNames.dullahan);
        public static readonly NamespaceID hellChariot = Get(VanillaEnemyNames.hellChariot);
        public static readonly NamespaceID anubisand = Get(VanillaEnemyNames.anubisand);

        public static readonly NamespaceID NetherWarrior = Get(VanillaEnemyNames.NetherWarrior);
        public static readonly NamespaceID NetherArcher = Get(VanillaEnemyNames.NetherArcher);
        public static readonly NamespaceID NetherVanguard = Get(VanillaEnemyNames.NetherVanguard);
        public static readonly NamespaceID AngryReverser = Get(VanillaEnemyNames.AngryReverser);
        public static readonly NamespaceID RaiderSkull = Get(VanillaEnemyNames.RaiderSkull);
        public static readonly NamespaceID Anubiskull = Get(VanillaEnemyNames.Anubiskull);

        public static readonly NamespaceID MonkZombie = Get(VanillaEnemyNames.MonkZombie);
        public static readonly NamespaceID LeatherMonkZombie = Get(VanillaEnemyNames.LeatherMonkZombie);
        public static readonly NamespaceID IronMonkZombie = Get(VanillaEnemyNames.IronMonkZombie);
        public static readonly NamespaceID FlagMonkZombie = Get(VanillaEnemyNames.FlagMonkZombie);
        public static readonly NamespaceID SixQiZombie = Get(VanillaEnemyNames.SixQiZombie);

        public static readonly NamespaceID Endermite = Get(VanillaEnemyNames.Endermite);
        public static readonly NamespaceID HeavyGutant = Get(VanillaEnemyNames.HeavyGutant);

        public static readonly NamespaceID reflectiveBarrierZombie = Get(VanillaEnemyNames.reflectiveBarrierZombie);
        public static readonly NamespaceID talismanZombie = Get(VanillaEnemyNames.talismanZombie);
        public static readonly NamespaceID wickedHermitZombie = Get(VanillaEnemyNames.wickedHermitZombie);
        public static readonly NamespaceID shikaisenZombie = Get(VanillaEnemyNames.shikaisenZombie);
        public static readonly NamespaceID emperorZombie = Get(VanillaEnemyNames.emperorZombie);

        public static readonly NamespaceID mutantZombie = Get(VanillaEnemyNames.mutantZombie);
        public static readonly NamespaceID megaMutantZombie = Get(VanillaEnemyNames.megaMutantZombie);
        public static readonly NamespaceID imp = Get(VanillaEnemyNames.imp);

        public static readonly NamespaceID boneWall = Get(VanillaEnemyNames.boneWall);
        public static readonly NamespaceID napstablook = Get(VanillaEnemyNames.napstablook);
        public static readonly NamespaceID reverseSatellite = Get(VanillaEnemyNames.reverseSatellite);
        public static readonly NamespaceID skeletonHorse = Get(VanillaEnemyNames.skeletonHorse);
        public static readonly NamespaceID dullahanHead = Get(VanillaEnemyNames.dullahanHead);
        public static readonly NamespaceID soulsand = Get(VanillaEnemyNames.soulsand);
        public static readonly NamespaceID seijaCursedDoll = Get(VanillaEnemyNames.seijaCursedDoll);
        public static readonly NamespaceID bedserker = Get(VanillaEnemyNames.bedserker);
        public static readonly NamespaceID skeletonWarrior = Get(VanillaEnemyNames.skeletonWarrior);
        public static readonly NamespaceID skeletonMage = Get(VanillaEnemyNames.skeletonMage);
        public static readonly NamespaceID shikaisenStaff = Get(VanillaEnemyNames.shikaisenStaff);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
