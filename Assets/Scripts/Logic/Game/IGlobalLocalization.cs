namespace MVZ2Logic.Games
{
    public interface IGlobalLocalization
    {
        string GetText(string textKey, params string[] args);
        string GetTextParticular(string textKey, string context, params string[] args);
        string GetTextPlural(string textKey, string textPlural, long n, params string[] args);
        string GetTextPluralParticular(string textKey, string textPlural, long n, string context, params string[] args);
    }
}
