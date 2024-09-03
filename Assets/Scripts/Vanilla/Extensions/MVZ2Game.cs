using MVZ2.GameContent;
using PVZEngine.Game;

namespace MVZ2.Vanilla
{
    public static class MVZ2Game
    {
        public static int GetMoney(this Game game)
        {
            return game.GetProperty<int>(GameProps.MONEY);
        }
        public static void SetMoney(this Game game, int value)
        {
            game.SetProperty(GameProps.MONEY, value);
        }
        public static void AddMoney(this Game game, int value)
        {
            game.SetMoney(game.GetMoney() + value);
        }
        public static int GetBlueprintSlots(this Game game)
        {
            return game.GetProperty<int>(GameProps.BLUEPRINT_SLOTS);
        }
        public static void SetBlueprintSlots(this Game game, int value)
        {
            game.SetProperty(GameProps.BLUEPRINT_SLOTS, value);
        }
    }
}
