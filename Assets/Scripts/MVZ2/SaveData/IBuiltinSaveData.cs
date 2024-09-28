using PVZEngine;

namespace MVZ2.Save
{
    public interface IBuiltinSaveData : ISaveData
    {
        NamespaceID LastMapID { get; set; }
        NamespaceID MapTalkID { get; set; }
        int GetMoney();
        void SetMoney(int value);
        int GetBlueprintSlots();
        void SetBlueprintSlots(int slots);

    }
}
