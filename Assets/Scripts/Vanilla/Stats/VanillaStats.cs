﻿using PVZEngine;

namespace MVZ2.Vanilla.Stats
{
    public static class VanillaStats
    {
        public static readonly NamespaceID CATEGORY_CONTRAPTION_PLACE = Get("contraption_place");
        public static readonly NamespaceID CATEGORY_CONTRAPTION_DESTROY = Get("contraption_destroy");
        public static readonly NamespaceID CATEGORY_CONTRAPTION_EVOKE = Get("contraption_evoke");
        public static readonly NamespaceID CATEGORY_ENEMY_SPAWN = Get("enemy_spawn");
        public static readonly NamespaceID CATEGORY_ENEMY_NEUTRALIZE = Get("enemy_neutralize");
        public static readonly NamespaceID CATEGORY_ENEMY_GAME_OVER = Get("enemy_gameover");
        public static readonly NamespaceID CATEGORY_MAX_ENDLESS_FLAGS = Get("max_endless_flags");

        public static readonly NamespaceID CATEGORY_IZ_CONTRAPTION_DESTROY = Get("iz_contraption_destroy");
        public static readonly NamespaceID CATEGORY_IZ_ENEMY_PLACE = Get("iz_enemy_place");
        public static readonly NamespaceID CATEGORY_IZ_ENEMY_DEATH = Get("iz_enemy_death");
        public static readonly NamespaceID CATEGORY_IZ_OBSERVER_TRIGGER = Get("iz_observer_trigger");
        public static readonly NamespaceID CATEGORY_IZ_GAME_OVER = Get("iz_game_over");
        public static NamespaceID Get(string path)
        {
            return new NamespaceID(VanillaMod.spaceName, path);
        }
    }
}
