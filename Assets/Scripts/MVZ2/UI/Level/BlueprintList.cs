using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class BlueprintList : BlueprintSet
    {
        public void SetSlotCount(int count)
        {
            slots.updateList(count);
        }
        public void ForceAlign(int index)
        {
            var element = blueprints.getElement(index);
            if (!element)
                return;
            element.transform.localPosition = GetBlueprintLocalPosition(index);
        }
        public Vector3 GetBlueprintPosition(int index)
        {
            var local = GetBlueprintLocalPosition(index);
            return blueprints.ListRoot.TransformPoint(local);
        }
        public Vector2 GetBlueprintLocalPosition(int index)
        {
            if (horizontal)
            {
                return offset + index * Vector2.right * cellSize.x;
            }
            return offset + index * Vector2.down * cellSize.y;
        }
        private void Update()
        {
            for (int i = 0; i < blueprints.Count; i++)
            {
                var element = blueprints.getElement(i);
                var localPos = element.transform.localPosition;
                localPos = Vector3.Lerp(localPos, GetBlueprintLocalPosition(i), alignSpeed);
                element.transform.localPosition = localPos;
            }
        }
        [SerializeField]
        private ElementList slots;
        [SerializeField]
        private Vector2 offset;
        [SerializeField]
        private Vector2 cellSize;
        [SerializeField]
        private float alignSpeed = 0.5f;
    }
}
