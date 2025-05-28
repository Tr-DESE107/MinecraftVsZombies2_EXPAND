﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Stages
{
    public static class VanillaStageNames
    {
        public const string debug = "debug";
        public const string tutorial = "tutorial";
        public const string starshardTutorial = "starshard_tutorial";
        public const string triggerTutorial = "trigger_tutorial";

        public const string prologue = "prologue";

        public const string halloween1 = "halloween_1";
        public const string halloween2 = "halloween_2";
        public const string halloween3 = "halloween_3";
        public const string halloween4 = "halloween_4";
        public const string halloween5 = "halloween_5";
        public const string halloween6 = "halloween_6";
        public const string halloween7 = "halloween_7";
        public const string halloween8 = "halloween_8";
        public const string halloween9 = "halloween_9";
        public const string halloween10 = "halloween_10";
        public const string halloween11 = "halloween_11";
        public const string halloweenEndless = "halloween_endless";

        public const string dream1 = "dream_1";
        public const string dream2 = "dream_2";
        public const string dream3 = "dream_3";
        public const string dream4 = "dream_4";
        public const string dream5 = "dream_5";
        public const string dream6 = "dream_6";
        public const string dream7 = "dream_7";
        public const string dream8 = "dream_8";
        public const string dream9 = "dream_9";
        public const string dream10 = "dream_10";
        public const string dream11 = "dream_11";
        public const string dreamEndless = "dream_endless";

        public const string castle1 = "castle_1";
        public const string castle2 = "castle_2";
        public const string castle3 = "castle_3";
        public const string castle4 = "castle_4";
        public const string castle5 = "castle_5";
        public const string castle6 = "castle_6";
        public const string castle7 = "castle_7";
        public const string castle8 = "castle_8";
        public const string castle9 = "castle_9";
        public const string castle10 = "castle_10";
        public const string castle11 = "castle_11";
        public const string castleEndless = "castle_endless";

        public const string mausoleum1 = "mausoleum_1";
        public const string mausoleum2 = "mausoleum_2";
        public const string mausoleum3 = "mausoleum_3";
        public const string mausoleum4 = "mausoleum_4";
        public const string mausoleum5 = "mausoleum_5";
        public const string mausoleum6 = "mausoleum_6";
        public const string mausoleum7 = "mausoleum_7";
        public const string mausoleum8 = "mausoleum_8";
        public const string mausoleum9 = "mausoleum_9";
        public const string mausoleum10 = "mausoleum_10";
        public const string mausoleum11 = "mausoleum_11";
        public const string mausoleumEndless = "mausoleum_endless";

        // Minigames
        public const string whackAGhost = "whack_a_ghost";
        public const string breakout = "breakout";
        public const string bigTroubleAndLittleZombie = "big_trouble_and_little_zombie";

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
        public const string puzzleIZombieEndless = "puzzle_i_zombie_endless";
    }
    public static class VanillaStageID
    {
        public static readonly NamespaceID debug = Get(VanillaStageNames.debug);
        public static readonly NamespaceID prologue = Get(VanillaStageNames.prologue);
        public static readonly NamespaceID tutorial = Get(VanillaStageNames.tutorial);
        public static readonly NamespaceID starshardTutorial = Get(VanillaStageNames.starshardTutorial);
        public static readonly NamespaceID triggerTutorial = Get(VanillaStageNames.triggerTutorial);
        public static readonly NamespaceID halloween1 = Get(VanillaStageNames.halloween1);
        public static readonly NamespaceID halloween2 = Get(VanillaStageNames.halloween2);
        public static readonly NamespaceID halloween7 = Get(VanillaStageNames.halloween7);
        public static readonly NamespaceID halloween10 = Get(VanillaStageNames.halloween10);
        public static readonly NamespaceID halloween11 = Get(VanillaStageNames.halloween11);
        public static readonly NamespaceID dream1 = Get(VanillaStageNames.dream1);
        public static readonly NamespaceID dream11 = Get(VanillaStageNames.dream11);
        public static readonly NamespaceID castle1 = Get(VanillaStageNames.castle1);
        public static readonly NamespaceID mausoleum1 = Get(VanillaStageNames.mausoleum1);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
