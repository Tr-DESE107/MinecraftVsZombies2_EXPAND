﻿using MVZ2.Vanilla;
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
        public const string puzzleFireInTheHole = "puzzle_fire_in_the_hole";
        public const string puzzleAllYourObservesAreBelongToUs = "puzzle_all_your_observes_are_belong_to_us";

        public const string iZombieDebug = "i_zombie_debug";

        public const string izeComposite = "ize_composite";
        public const string izeControl = "ize_control";
        public const string izeInstakill = "ize_instakill";
        public const string izeDispensers = "ize_dispensers";
        public const string izeExplosives = "ize_explosives";
        public const string izeSpikes = "ize_spikes";
        public const string izeFire = "ize_fire";
        public const string izeAwards = "ize_awards";
        public const string izeError = "ize_error";
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
        public static readonly NamespaceID puzzleFireInTheHole = Get(VanillaIZombieLayoutNames.puzzleFireInTheHole);
        public static readonly NamespaceID puzzleAllYourObservesAreBelongToUs = Get(VanillaIZombieLayoutNames.puzzleAllYourObservesAreBelongToUs);

        public static readonly NamespaceID iZombieDebug = Get(VanillaIZombieLayoutNames.iZombieDebug);

        public static readonly NamespaceID izeComposite = Get(VanillaIZombieLayoutNames.izeComposite);
        public static readonly NamespaceID izeControl = Get(VanillaIZombieLayoutNames.izeControl);
        public static readonly NamespaceID izeInstakill = Get(VanillaIZombieLayoutNames.izeInstakill);
        public static readonly NamespaceID izeDispensers = Get(VanillaIZombieLayoutNames.izeDispensers);
        public static readonly NamespaceID izeExplosives = Get(VanillaIZombieLayoutNames.izeExplosives);
        public static readonly NamespaceID izeSpikes = Get(VanillaIZombieLayoutNames.izeSpikes);
        public static readonly NamespaceID izeFire = Get(VanillaIZombieLayoutNames.izeFire);
        public static readonly NamespaceID izeAwards = Get(VanillaIZombieLayoutNames.izeAwards);
        public static readonly NamespaceID izeError = Get(VanillaIZombieLayoutNames.izeError);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
