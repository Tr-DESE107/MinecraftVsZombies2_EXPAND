using MVZ2.Save;

namespace PVZEngine.Game
{
    public interface IModLogic : IContentProvider
    {
        void Init(Game game);
        ModSaveData CreateSaveData();
        ModSaveData LoadSaveData(string json);
    }
}
