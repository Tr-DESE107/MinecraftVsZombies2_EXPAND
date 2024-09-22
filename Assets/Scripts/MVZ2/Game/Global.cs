using MVZ2.Save;
using PVZEngine;
using PVZEngine.Game;

namespace MVZ2
{
    public static class Global
    {
        public static bool IsMobile()
        {
            return Main.IsMobile();
        }
        private static MainManager Main => MainManager.Instance;
        public static Game Game => Main.Game;
    }
}
