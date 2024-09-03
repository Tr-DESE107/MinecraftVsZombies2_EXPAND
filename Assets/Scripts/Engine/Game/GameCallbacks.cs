using PVZEngine.Base;
using PVZEngine.Callbacks;

namespace PVZEngine.Game
{
    public static class GameCallbacks
    {
        public readonly static CallbackActionList<ITalkSystem, string, string[]> TalkAction = new();
    }
}
