using PVZEngine;
using UnityEngine;

namespace MVZ2Logic.Entities
{
    public interface IShapeMeta
    {
        Vector3 GetArmorPosition(NamespaceID slotID, NamespaceID armorID);
        Vector3 GetArmorScale(NamespaceID slotID, NamespaceID armorID);
        Vector3 GetArmorModelOffset(NamespaceID slotID, NamespaceID armorID);
        string GetArmorModelAnchor(NamespaceID slotID, NamespaceID armorID);
    }
}
