namespace PVZEngine.Base
{
    public interface ICachedDefinition
    {
        void CacheContents(IGameContent content);
        void ClearCaches();
    }
}
