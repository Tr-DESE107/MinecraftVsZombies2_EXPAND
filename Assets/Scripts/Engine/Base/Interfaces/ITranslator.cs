namespace PVZEngine
{
    public interface ITranslator
    {
        string GetText(string textKey, params string[] args);
        string GetTextParticular(string textKey, string context, params string[] args);
    }
}
