using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Effects
{
    public static class VanillaEffectNames
    {
        public const string miner = "miner";
        public const string mineDebris = "mine_debris";
        public const string brokenArmor = "broken_armor";
        public const string fragment = "fragment";
        public const string starParticles = "star_particles";
        public const string gemEffect = "gem_effect";
        public const string smoke = "smoke";
        public const string thunderBolt = "thunder_bolt";
        public const string evocationStar = "evocation_star";
        public const string rain = "rain";
        public const string shineRing = "shine_ring";
        public const string stunStars = "stun_stars";
        public const string stunningFlash = "stunning_flash";
        public const string explosion = "explosion";
        public const string soulfire = "soulfire";
        public const string soulfireBurn = "soulfire_burn";
        public const string soulfireBlast = "soulfire_blast";
        public const string mummyGas = "mummy_gas";
        public const string burningGas = "burning_gas";
        public const string healParticles = "heal_particles";
        public const string boneParticles = "bone_particles";
        public const string bloodParticles = "blood_particles";
        public const string smokeCluster = "smoke_cluster";
        public const string electricArc = "electric_arc";
        public const string goreParticles = "gore_particles";
        public const string frankensteinJumpTrail = "frankenstein_jump_trail";
        public const string frankensteinHead = "frankenstein_head";
        public const string splashParticles = "splash_particles";
        public const string gearParticles = "gear_particles";
        public const string pow = "pow";
        public const string vortex = "vortex";
        public const string giantSpike = "giant_spike";
        public const string fireBreath = "fire_breath";
        public const string weaknessGas = "weakness_gas";
        public const string magneticLine = "magnetic_line";
        public const string hoe = "hoe";
        public const string breakoutBoard = "breakout_board";
        public const string nightmareWatchingEye = "nightmare_watching_eye";
        public const string nightmarePortal = "nightmare_portal";
        public const string darkMatterParticles = "dark_matter_particles";
        public const string nightmareaperSplash = "nightmareaper_splash";
        public const string nightmareaperShadow = "nightmareaper_shadow";
        public const string sliceSpark = "slice_spark";
        public const string crushingWalls = "crushing_walls";
        public const string nightmareaperTimer = "nightmareaper_timer";
        public const string nightmareDarkness = "nightmare_darkness";
        public const string nightmareGlass = "nightmare_glass";
        public const string floatingText = "floating_text";
        public const string spikeParticles = "spike_particles";
        public const string diamondSpikeParticles = "diamond_spike_particles";
        public const string mindControlLines = "mind_control_lines";
        public const string mutantZombieWeapon = "mutant_zombie_weapon";
        public const string waterLightningParticles = "water_lightning_particles";
        public const string thunderCloud = "thunder_cloud";
        public const string magicBombExplosion = "magic_bomb_explosion";
        public const string seijaCameraFrame = "seija_camera_frame";
        public const string seijaFaintEffect = "seija_faint_effect";
        public const string witherSummoners = "wither_summoners";
        public const string castleTwilight = "castle_twilight";

        public const string thunderBoltHorizontal = "thunder_bolt_horizontal";
        public const string emberParticles = "ember_particles";
        public const string spawnerAppearEmbers = "spawner_appear_embers";
        public const string fireburn = "fireburn";
        public const string cursedFireburn = "cursed_fireburn";
        public const string cursedMeteor = "cursed_meteor";
        public const string cursedFireParticles = "cursed_fire_particles";
        public const string blackhole = "blackhole";
        public const string annihilationField = "annihilation_field";
        public const string desireLump = "desire_lump";
        public const string binaryParticles = "binary_particles";
        public const string redline = "redline";
        public const string izObserver = "iz_observer";
        public const string spiritUniverse = "spirit_universe";
        public const string zombieBlock = "zombie_block";
        public const string darkHole = "dark_hole";
        public const string amplifiedRoar = "amplified_roar";

        public const string ufoBackground = "ufo_background";
        public const string fireworkBlast = "firework_blast";
    }
    public static class VanillaEffectID
    {
        public static readonly NamespaceID miner = Get(VanillaEffectNames.miner);
        public static readonly NamespaceID mineDebris = Get(VanillaEffectNames.mineDebris);
        public static readonly NamespaceID brokenArmor = Get(VanillaEffectNames.brokenArmor);
        public static readonly NamespaceID fragment = Get(VanillaEffectNames.fragment);
        public static readonly NamespaceID starParticles = Get(VanillaEffectNames.starParticles);
        public static readonly NamespaceID gemEffect = Get(VanillaEffectNames.gemEffect);
        public static readonly NamespaceID smoke = Get(VanillaEffectNames.smoke);
        public static readonly NamespaceID thunderBolt = Get(VanillaEffectNames.thunderBolt);
        public static readonly NamespaceID evocationStar = Get(VanillaEffectNames.evocationStar);
        public static readonly NamespaceID explosion = Get(VanillaEffectNames.explosion);
        public static readonly NamespaceID rain = Get(VanillaEffectNames.rain);
        public static readonly NamespaceID shineRing = Get(VanillaEffectNames.shineRing);
        public static readonly NamespaceID stunStars = Get(VanillaEffectNames.stunStars);
        public static readonly NamespaceID stunningFlash = Get(VanillaEffectNames.stunningFlash);
        public static readonly NamespaceID soulfire = Get(VanillaEffectNames.soulfire);
        public static readonly NamespaceID soulfireBurn = Get(VanillaEffectNames.soulfireBurn);
        public static readonly NamespaceID soulfireBlast = Get(VanillaEffectNames.soulfireBlast);
        public static readonly NamespaceID mummyGas = Get(VanillaEffectNames.mummyGas);
        public static readonly NamespaceID burningGas = Get(VanillaEffectNames.burningGas);
        public static readonly NamespaceID healParticles = Get(VanillaEffectNames.healParticles);
        public static readonly NamespaceID boneParticles = Get(VanillaEffectNames.boneParticles);
        public static readonly NamespaceID bloodParticles = Get(VanillaEffectNames.bloodParticles);
        public static readonly NamespaceID smokeCluster = Get(VanillaEffectNames.smokeCluster);
        public static readonly NamespaceID electricArc = Get(VanillaEffectNames.electricArc);
        public static readonly NamespaceID gore = Get(VanillaEffectNames.goreParticles);
        public static readonly NamespaceID frankensteinJumpTrail = Get(VanillaEffectNames.frankensteinJumpTrail);
        public static readonly NamespaceID frankensteinHead = Get(VanillaEffectNames.frankensteinHead);
        public static readonly NamespaceID splashParticles = Get(VanillaEffectNames.splashParticles);
        public static readonly NamespaceID gearParticles = Get(VanillaEffectNames.gearParticles);
        public static readonly NamespaceID pow = Get(VanillaEffectNames.pow);
        public static readonly NamespaceID vortex = Get(VanillaEffectNames.vortex);
        public static readonly NamespaceID giantSpike = Get(VanillaEffectNames.giantSpike);
        public static readonly NamespaceID fireBreath = Get(VanillaEffectNames.fireBreath);
        public static readonly NamespaceID weaknessGas = Get(VanillaEffectNames.weaknessGas);
        public static readonly NamespaceID magneticLine = Get(VanillaEffectNames.magneticLine);
        public static readonly NamespaceID hoe = Get(VanillaEffectNames.hoe);
        public static readonly NamespaceID breakoutBoard = Get(VanillaEffectNames.breakoutBoard);
        public static readonly NamespaceID nightmareWatchingEye = Get(VanillaEffectNames.nightmareWatchingEye);
        public static readonly NamespaceID nightmarePortal = Get(VanillaEffectNames.nightmarePortal);
        public static readonly NamespaceID darkMatterParticles = Get(VanillaEffectNames.darkMatterParticles);
        public static readonly NamespaceID nightmareaperSplash = Get(VanillaEffectNames.nightmareaperSplash);
        public static readonly NamespaceID nightmareaperShadow = Get(VanillaEffectNames.nightmareaperShadow);
        public static readonly NamespaceID sliceSpark = Get(VanillaEffectNames.sliceSpark);
        public static readonly NamespaceID crushingWalls = Get(VanillaEffectNames.crushingWalls);
        public static readonly NamespaceID nightmareaperTimer = Get(VanillaEffectNames.nightmareaperTimer);
        public static readonly NamespaceID nightmareDarkness = Get(VanillaEffectNames.nightmareDarkness);
        public static readonly NamespaceID nightmareGlass = Get(VanillaEffectNames.nightmareGlass);
        public static readonly NamespaceID floatingText = Get(VanillaEffectNames.floatingText);
        public static readonly NamespaceID spikeParticles = Get(VanillaEffectNames.spikeParticles);
        public static readonly NamespaceID diamondSpikeParticles = Get(VanillaEffectNames.diamondSpikeParticles);
        public static readonly NamespaceID mindControlLines = Get(VanillaEffectNames.mindControlLines);
        public static readonly NamespaceID mutantZombieWeapon = Get(VanillaEffectNames.mutantZombieWeapon);
        public static readonly NamespaceID waterLightningParticles = Get(VanillaEffectNames.waterLightningParticles);
        public static readonly NamespaceID thunderCloud = Get(VanillaEffectNames.thunderCloud);
        public static readonly NamespaceID magicBombExplosion = Get(VanillaEffectNames.magicBombExplosion);
        public static readonly NamespaceID seijaCameraFrame = Get(VanillaEffectNames.seijaCameraFrame);
        public static readonly NamespaceID seijaFaintEffect = Get(VanillaEffectNames.seijaFaintEffect);
        public static readonly NamespaceID witherSummoners = Get(VanillaEffectNames.witherSummoners);
        public static readonly NamespaceID castleTwilight = Get(VanillaEffectNames.castleTwilight);

        public static readonly NamespaceID thunderBoltHorizontal = Get(VanillaEffectNames.thunderBoltHorizontal);
        public static readonly NamespaceID emberParticles = Get(VanillaEffectNames.emberParticles);
        public static readonly NamespaceID spawnerAppearEmbers = Get(VanillaEffectNames.spawnerAppearEmbers);
        public static readonly NamespaceID fireburn = Get(VanillaEffectNames.fireburn);
        public static readonly NamespaceID cursedFireburn = Get(VanillaEffectNames.cursedFireburn);
        public static readonly NamespaceID cursedMeteor = Get(VanillaEffectNames.cursedMeteor);
        public static readonly NamespaceID cursedFireParticles = Get(VanillaEffectNames.cursedFireParticles);
        public static readonly NamespaceID blackhole = Get(VanillaEffectNames.blackhole);
        public static readonly NamespaceID annihilationField = Get(VanillaEffectNames.annihilationField);
        public static readonly NamespaceID desireLump = Get(VanillaEffectNames.desireLump);
        public static readonly NamespaceID binaryParticles = Get(VanillaEffectNames.binaryParticles);
        public static readonly NamespaceID redline = Get(VanillaEffectNames.redline);
        public static readonly NamespaceID izObserver = Get(VanillaEffectNames.izObserver);
        public static readonly NamespaceID spiritUniverse = Get(VanillaEffectNames.spiritUniverse);
        public static readonly NamespaceID zombieBlock = Get(VanillaEffectNames.zombieBlock);
        public static readonly NamespaceID darkHole = Get(VanillaEffectNames.darkHole);
        public static readonly NamespaceID amplifiedRoar = Get(VanillaEffectNames.amplifiedRoar);

        public static readonly NamespaceID ufoBackground = Get(VanillaEffectNames.ufoBackground);
        public static readonly NamespaceID fireworkBlast = Get(VanillaEffectNames.fireworkBlast);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
