namespace PVZEngine
{
    public interface ITranslator
    {
        string GetText(string textKey);
        string GetTextParticular(string textKey, string context);
    }
}
