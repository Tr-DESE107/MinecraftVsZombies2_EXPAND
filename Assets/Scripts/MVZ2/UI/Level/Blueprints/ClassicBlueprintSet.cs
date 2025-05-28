﻿using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public abstract class ClassicBlueprintSet : BlueprintSet
    {
        public virtual void SetSlotCount(int count)
        {
            slots.updateList(count);
        }
        public Vector2 GetBlueprintLocalPosition(int index)
        {
            if (horizontal)
            {
                return offset + index * Vector2.right * cellSize.x;
            }
            return offset + index * Vector2.down * cellSize.y;
        }
        public abstract int GetBlueprintCount();
        private void Update()
        {
            for (int i = 0; i < GetBlueprintCount(); i++)
            {
                var element = GetBlueprintAt(i);
                if (!element)
                    continue;
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
