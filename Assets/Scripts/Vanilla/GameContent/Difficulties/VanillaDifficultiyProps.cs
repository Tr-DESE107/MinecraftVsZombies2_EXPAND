using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.GameContent.Difficulties
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class VanillaDifficultyProps
    {
        public static VanillaDifficultyPropertyMeta<T> Get<T>(string name, T defaultValue = default)
        {
            return new VanillaDifficultyPropertyMeta<T>(name, defaultValue);
        }
        // Contraptions
        public static readonly VanillaDifficultyPropertyMeta<float> GUNPOWDER_DAMAGE_MULTIPLIER = Get<float>("gunpowderDamageMultiplier", 1);
        public static float GetGunpowderDamageMultiplier(this LevelEngine level) => level.GetProperty<float>(GUNPOWDER_DAMAGE_MULTIPLIER);

        // Enemies
        public static readonly VanillaDifficultyPropertyMeta<int> NAPSTABLOOK_PARALYSIS_TIME = Get<int>("napstablookParalysisTime", 45);
        public static readonly VanillaDifficultyPropertyMeta<float> GHAST_DAMAGE_MULTIPLIER = Get<float>("ghastDamageMultiplier", 1f);
        public static readonly VanillaDifficultyPropertyMeta<int> MOTHER_TERROR_EGG_COUNT = Get<int>("motherTerrorEggCount", 1);
        public static readonly VanillaDifficultyPropertyMeta<int> PARASITIZED_TERROR_COUNT = Get<int>("parasitizedTerrorCount", 3);
        public static readonly VanillaDifficultyPropertyMeta<float> REVERSE_SATELLITE_DAMAGE_MULTIPLIER = Get<float>("reverseSatelliteDamageMultiplier", 1f);
        public static readonly VanillaDifficultyPropertyMeta<int> SKELETON_HORSE_JUMP_TIMES = Get<int>("skeletonHorseJumpTimes", 1);
        public static readonly VanillaDifficultyPropertyMeta<int> WICKED_HERMIT_ZOMBIE_STUN_TIME = Get<int>("wickedHermitZombieStunTime", 150);
        public static int GetNapstablookParalysisTime(this LevelEngine level) => level.GetProperty<int>(NAPSTABLOOK_PARALYSIS_TIME);
        public static float GetGhastDamageMultiplier(this LevelEngine level) => level.GetProperty<float>(GHAST_DAMAGE_MULTIPLIER);
        public static int GetMotherTerrorEggCount(this LevelEngine level) => level.GetProperty<int>(MOTHER_TERROR_EGG_COUNT);
        public static int GetParasitizedTerrorCount(this LevelEngine level) => level.GetProperty<int>(PARASITIZED_TERROR_COUNT);
        public static float GetReverseSatelliteDamageMultiplier(this LevelEngine level) => level.GetProperty<float>(REVERSE_SATELLITE_DAMAGE_MULTIPLIER);
        public static int GetSkeletonHorseJumpTimes(this LevelEngine level) => level.GetProperty<int>(SKELETON_HORSE_JUMP_TIMES);
        public static int GetWickedHermitZombieStunTime(this LevelEngine level) => level.GetProperty<int>(WICKED_HERMIT_ZOMBIE_STUN_TIME);

        // Bosses
        public static readonly VanillaDifficultyPropertyMeta<bool> FRANKENSTEIN_INSTANT_STEEL = Get<bool>("frankensteinInstantSteel");
        public static readonly VanillaDifficultyPropertyMeta<bool> FRANKENSTEIN_NO_STEEL = Get<bool>("frankensteinNoSteel");
        public static readonly VanillaDifficultyPropertyMeta<float> FRANKENSTEIN_SPEED = Get<float>("frankensteinSpeed", 1f);
        public static readonly VanillaDifficultyPropertyMeta<bool> SLENDERMAN_MIND_SWAP_ZOMBIES = Get<bool>("slendermanMindSwapZombies");
        public static readonly VanillaDifficultyPropertyMeta<int> SLENDERMAN_FATE_CHOICE_COUNT = Get<int>("slendermanFateChoiceCount", 3);
        public static readonly VanillaDifficultyPropertyMeta<int> SLENDERMAN_MAX_FATE_TIMES = Get<int>("slendermanMaxFateTimes", 4);
        public static readonly VanillaDifficultyPropertyMeta<float> CRUSHING_WALLS_SPEED = Get<float>("crushingWallsSpeed", 4f);
        public static readonly VanillaDifficultyPropertyMeta<float> NIGHTMAREAPER_SPIN_DAMAGE = Get<float>("nightmareaperSpinDamage", 15f);
        public static readonly VanillaDifficultyPropertyMeta<int> NIGHTMAREAPER_TIMEOUT = Get<int>("nightmareaperTimeout", 2700);
        public static readonly VanillaDifficultyPropertyMeta<float> WITHER_REGENERATION = Get<float>("witherRegeneration", 1);
        public static readonly VanillaDifficultyPropertyMeta<bool> WITHER_SKULL_WITHERS_TARGET = Get<bool>("witherSkullWithersTarget");
        public static readonly VanillaDifficultyPropertyMeta<bool> THE_GIANT_IS_MALLEABLE = Get<bool>("theGiantIsMalleable");

        public static bool FrankensteinNoSteelPhase(this LevelEngine level) => level.GetProperty<bool>(FRANKENSTEIN_NO_STEEL);
        public static bool FrankensteinInstantSteelPhase(this LevelEngine level) => level.GetProperty<bool>(FRANKENSTEIN_INSTANT_STEEL);
        public static float GetFrankensteinSpeed(this LevelEngine level) => level.GetProperty<float>(FRANKENSTEIN_SPEED);
        public static bool SlendermanMindSwapZombies(this LevelEngine level) => level.GetProperty<bool>(SLENDERMAN_MIND_SWAP_ZOMBIES);
        public static int GetSlendermanFateChoiceCount(this LevelEngine level) => level.GetProperty<int>(SLENDERMAN_FATE_CHOICE_COUNT);
        public static int GetSlendermanMaxFateTimes(this LevelEngine level) => level.GetProperty<int>(SLENDERMAN_MAX_FATE_TIMES);
        public static float GetCrushingWallsSpeed(this LevelEngine level) => level.GetProperty<float>(CRUSHING_WALLS_SPEED);
        public static float GetNightmareaperSpinDamage(this LevelEngine level) => level.GetProperty<float>(NIGHTMAREAPER_SPIN_DAMAGE);
        public static int GetNightmareaperTimeout(this LevelEngine level) => level.GetProperty<int>(NIGHTMAREAPER_TIMEOUT);
        public static float GetWitherRegeneration(this LevelEngine level) => level.GetProperty<float>(WITHER_REGENERATION);
        public static bool WitherSkullWithersTarget(this LevelEngine level) => level.GetProperty<bool>(WITHER_SKULL_WITHERS_TARGET);
        public static bool TheGiantIsMalleable(this LevelEngine level) => level.GetProperty<bool>(THE_GIANT_IS_MALLEABLE);

        // Level
        public static readonly VanillaDifficultyPropertyMeta<float> STARSHARD_CARRIER_CHANCE_INCREAMENT = Get<float>("starshardCarrierChanceIncreament", 10f);
        public static readonly VanillaDifficultyPropertyMeta<float> REDSTONE_CARRIER_CHANCE_INCREAMENT = Get<float>("redstoneCarrierChanceIncreament", 10f);
        public static readonly VanillaDifficultyPropertyMeta<int> IZ_FURNACE_REDSTONE_COUNT = Get<int>("izFurnaceRedstoneCount", 8);
        public static float GetStarshardCarrierChanceIncreament(this LevelEngine level) => level.GetProperty<float>(STARSHARD_CARRIER_CHANCE_INCREAMENT);
        public static float GetRedstoneCarrierChanceIncreament(this LevelEngine level) => level.GetProperty<float>(REDSTONE_CARRIER_CHANCE_INCREAMENT);
        public static int GetIZFurnaceRedstoneCount(this LevelEngine level) => level.GetProperty<int>(IZ_FURNACE_REDSTONE_COUNT);
    }
}