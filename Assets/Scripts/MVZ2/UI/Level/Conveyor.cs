using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class Conveyor : BlueprintSet
    {
        public void SetSlotCount(int count)
        {
            slotCount = count;
            structureList.updateList(count);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
        public void SetBlueprintNormalizedPosition(int index, float position)
        {
            Blueprint blueprint = GetBlueprintAt(index);
            if (blueprint == null)
                return;
            blueprint.transform.position = Vector3.LerpUnclamped(startPositionAnchor.position, endPositionAnchor.position, position / (slotCount - 1));
        }
        private int slotCount;
        [SerializeField]
        private ElementList structureList;
        [SerializeField]
        private Transform startPositionAnchor;
        [SerializeField]
        private Transform endPositionAnchor;
    }
}
