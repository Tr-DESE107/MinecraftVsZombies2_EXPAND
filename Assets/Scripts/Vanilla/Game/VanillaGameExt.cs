using MVZ2.GameContent.Contraptions;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVZ2.Vanilla.Game
{
    public static class VanillaGameExt
    {
        public static bool IsRandomChina(this IGame game)
        {
            var userName = game.GetCurrentUserName();
            return game.IsRandomChinaUserName(userName);
        }
        public static bool IsRandomChinaUserName(this IGame game, string name)
        {
            return randomChinaNames.Any(n => n.ToLower() == name.ToLower());
        }
        public static readonly string[] randomChinaNames = new string[]
        {
            "RandomChina",
            "RandmChina",
        };
    }
}
