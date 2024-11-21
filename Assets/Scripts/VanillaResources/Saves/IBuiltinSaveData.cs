using PVZEngine;

namespace MVZ2Logic.Saves
{
    public interface IBuiltinSaveData : ISaveData
    {
        NamespaceID LastMapID { get; set; }
        NamespaceID MapTalkID { get; set; }
        int GetMoney();
        void SetMoney(int value);
        int GetBlueprintSlots();
        void SetBlueprintSlots(int slots);
        int GetStarshardSlots();
        void SetStarshardSlots(int slots);

    }
}
