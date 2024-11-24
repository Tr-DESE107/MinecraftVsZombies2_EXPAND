using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using MVZ2Logic.Saves;
using PVZEngine;

namespace MVZ2.Games
{
    public partial class Game
    {
        public string GetText(string textKey, params string[] args)
        {
            return localization.GetText(textKey, args);
        }

        public string GetTextParticular(string textKey, string context, params string[] args)
        {
            return localization.GetTextParticular(textKey, context, args);
        }
        private IGameLocalization localization;
    }
}