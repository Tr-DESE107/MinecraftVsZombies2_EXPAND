using System.Linq;
using MVZ2Logic.Games;

namespace MVZ2.Vanilla.Game
{
    public static class VanillaGameExt
    {
        public static bool IsRandomChina(this IGlobalSaveData saves)
        {
            var userName = saves.GetCurrentUserName();
            return saves.IsRandomChinaUserName(userName);
        }
        public static bool IsRandomChinaUserName(this IGlobalSaveData saves, string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return randomChinaNames.Any(n => n.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }
        public static readonly string[] randomChinaNames = new string[]
        {
            "RandomChina",
            "RandmChina",
        };
        public static bool IsDebugUser(this IGame game)
        {
            var userName = game.GetCurrentUserName();
            return game.IsDebugUserName(userName);
        }
        public static bool IsDebugUserName(this IGame game, string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return debugUserNames.Any(n => n.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }
        public static readonly string[] debugUserNames = new string[]
        {
            "debug",
        };
    }
}
