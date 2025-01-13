using System;
using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class BlueprintList : BlueprintSet
    {
        public void SetSlotCount(int count)
        {
            slots.updateList(count);
            blueprints.SetCount(count);
        }
        public override Blueprint CreateBlueprint()
        {
            return blueprints.CreateItem().GetComponent<Blueprint>();
        }
        public override void InsertBlueprint(int index, Blueprint blueprint)
        {
            if (!blueprint)
                return;
            if (index < 0 || index >= blueprints.Count)
                return;
            blueprints.Insert(index, blueprint.gameObject);
            blueprint.OnPointerEnter += OnBlueprintPointerEnterCallback;
            blueprint.OnPointerExit += OnBlueprintPointerExitCallback;
            blueprint.OnPointerDown += OnBlueprintPointerDownCallback;
        }
        public override bool RemoveBlueprint(Blueprint blueprint)
        {
            if (!blueprint)
                return false;
            if (!blueprints.Remove(blueprint.gameObject))
                return false;
            blueprint.OnPointerEnter -= OnBlueprintPointerEnterCallback;
            blueprint.OnPointerExit -= OnBlueprintPointerExitCallback;
            blueprint.OnPointerDown -= OnBlueprintPointerDownCallback;
            return true;
        }
        public override bool DestroyBlueprint(Blueprint blueprint)
        {
            if (!blueprint)
                return false;
            if (!blueprints.DestroyItem(blueprint.gameObject))
                return false;
            blueprint.OnPointerEnter -= OnBlueprintPointerEnterCallback;
            blueprint.OnPointerExit -= OnBlueprintPointerExitCallback;
            blueprint.OnPointerDown -= OnBlueprintPointerDownCallback;
            return true;
        }
        public override Blueprint GetBlueprintAt(int index)
        {
            return blueprints.getElement<Blueprint>(index);
        }
        public override int GetBlueprintIndex(Blueprint value)
        {
            return blueprints.indexOf(value);
        }
        public void ForceAlign(int index)
        {
            var element = GetBlueprintAt(index);
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
        public void AlignRemainBlueprints(int removeIndex)
        {
            for (int i = removeIndex; i < blueprints.Count; i++)
            {
                if (i >= blueprints.Count - 1)
                    continue;
                var nextBlueprint = GetBlueprintAt(i + 1);
                RemoveBlueprintAt(i + 1);
                InsertBlueprint(i, nextBlueprint);
            }
        }
        private void Update()
        {
            for (int i = 0; i < blueprints.Count; i++)
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
        private ElementArray blueprints;
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
