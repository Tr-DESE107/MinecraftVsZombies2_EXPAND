using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Level
{
    public static class VanillaLevelStates
    {
        public const int STATE_NOT_STARTED = 0;
        public const int STATE_STARTED = 1;
        public const int STATE_HUGE_WAVE_APPROACHING = 2;
        public const int STATE_FINAL_WAVE = 3;
        public const int STATE_AFTER_FINAL_WAVE = 4;
        public const int STATE_BOSS_FIGHT = 5;
        public const int STATE_BOSS_FIGHT_2 = 6;
        public const int STATE_AFTER_BOSS = 100;

        public const int STATE_IZ_NORMAL = 1;
        public const int STATE_IZ_NEXT = 2;
        public const int STATE_IZ_FINISHED = 3;
    }
}
