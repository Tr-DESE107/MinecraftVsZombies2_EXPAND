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
        public string GetTextPlural(string textKey, string textPlural, long n, params string[] args)
        {
            return localization.GetTextPlural(textKey, textPlural, n, args);
        }
        public string GetTextPluralParticular(string textKey, string textPlural, long n, string context, params string[] args)
        {
            return localization.GetTextPluralParticular(textKey, textPlural, n, context, args);
        }
        private IGameLocalization localization;
    }
}