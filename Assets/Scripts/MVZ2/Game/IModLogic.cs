using MVZ2.Save;

namespace PVZEngine.Game
{
    public interface IModLogic : IContentProvider
    {
        void Init();
        ModSaveData CreateSaveData();
        ModSaveData LoadSaveData(string json);
    }
}
