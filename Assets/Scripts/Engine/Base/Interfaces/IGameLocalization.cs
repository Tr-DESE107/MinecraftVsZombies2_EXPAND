namespace PVZEngine
{
    public interface IGameLocalization
    {
        string GetText(string textKey, params string[] args);
        string GetTextParticular(string textKey, string context, params string[] args);
    }
}
