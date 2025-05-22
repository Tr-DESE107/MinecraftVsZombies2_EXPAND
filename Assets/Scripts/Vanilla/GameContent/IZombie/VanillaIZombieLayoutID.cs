using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Stages
{
    public static class VanillaIZombieLayoutNames
    {
        public const string dispenserPunchton4 = "dispenser_punchton4";
        public const string highAndLow4 = "high_and_low4";
        public const string redAlert5 = "red_alert5";

        // Puzzles
        public const string puzzleIZombie = "puzzle_i_zombie";
        public const string puzzleISkeleton = "puzzle_i_skeleton";
        public const string puzzleCanYouPassIt = "puzzle_can_you_pass_it";
        public const string puzzleAbsoluteDefense = "puzzle_absolute_defense";
        public const string puzzleDeadBalloon = "puzzle_dead_balloon";
        public const string puzzleUnbreakable = "puzzle_unbreakable";
        public const string puzzleMineclear = "puzzle_mineclear";
    }
    public static class VanillaIZombieLayoutID
    {
        public static readonly NamespaceID dispenserPunchton4 = Get(VanillaIZombieLayoutNames.dispenserPunchton4);
        public static readonly NamespaceID highAndLow4 = Get(VanillaIZombieLayoutNames.highAndLow4);
        public static readonly NamespaceID redAlert5 = Get(VanillaIZombieLayoutNames.redAlert5);

        // Puzzles
        public static readonly NamespaceID puzzleIZombie = Get(VanillaIZombieLayoutNames.puzzleIZombie);
        public static readonly NamespaceID puzzleISkeleton = Get(VanillaIZombieLayoutNames.puzzleISkeleton);
        public static readonly NamespaceID puzzleCanYouPassIt = Get(VanillaIZombieLayoutNames.puzzleCanYouPassIt);
        public static readonly NamespaceID puzzleAbsoluteDefense = Get(VanillaIZombieLayoutNames.puzzleAbsoluteDefense);
        public static readonly NamespaceID puzzleDeadBalloon = Get(VanillaIZombieLayoutNames.puzzleDeadBalloon);
        public static readonly NamespaceID puzzleUnbreakable = Get(VanillaIZombieLayoutNames.puzzleUnbreakable);
        public static readonly NamespaceID puzzleMineclear = Get(VanillaIZombieLayoutNames.puzzleMineclear);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
