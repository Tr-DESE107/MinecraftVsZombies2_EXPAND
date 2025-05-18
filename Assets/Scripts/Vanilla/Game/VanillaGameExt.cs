using System.Linq;
using MVZ2Logic.Games;

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
            if (string.IsNullOrEmpty(name))
                return false;
            return randomChinaNames.Any(n => n.ToLower() == name.ToLower());
        }
        public static readonly string[] randomChinaNames = new string[]
        {
            "RandomChina",
            "RandmChina",
        };
    }
}
