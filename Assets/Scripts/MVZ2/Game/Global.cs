using MVZ2.Games;
using MVZ2.Managers;

namespace MVZ2
{
    public static class Global
    {
        public static bool IsMobile()
        {
            return Main.IsMobile();
        }
        public static string BuiltinNamespace => Main.BuiltinNamespace;
        public static Game Game => Main.Game;
        private static MainManager Main => MainManager.Instance;
    }
}
