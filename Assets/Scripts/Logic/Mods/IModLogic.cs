using MVZ2Logic.Games;
using MVZ2Logic.Saves;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Callbacks;

namespace MVZ2Logic.Modding
{
    public interface IModLogic
    {
        string Namespace { get; }
        void Init(IGlobalGame game);
        void LateInit(IGlobalGame game);
        void PostReloadMods(IGlobalGame game);
        void PostGameInit();
        ITrigger[] GetTriggers();
        Definition[] GetDefinitions();
        ModSaveData CreateSaveData();
        ModSaveData LoadSaveData(string json);
    }
}
