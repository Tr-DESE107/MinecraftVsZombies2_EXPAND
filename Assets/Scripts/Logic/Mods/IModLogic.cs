using MVZ2Logic.Saves;
using PVZEngine;

namespace MVZ2Logic.Modding
{
    public interface IModLogic : IGameContent
    {
        void Load();
        void Unload();
        void PostGameInit();
        ModSaveData CreateSaveData();
        ModSaveData LoadSaveData(string json);
    }
}
