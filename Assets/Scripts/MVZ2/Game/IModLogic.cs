using MVZ2.Save;
using PVZEngine;

namespace MVZ2.Modding
{
    public interface IModLogic : IContentProvider
    {
        void PostGameInit();
        ModSaveData CreateSaveData();
        ModSaveData LoadSaveData(string json);
    }
}
