using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public class BlueprintList : MonoBehaviour
    {
        public void SetSlotCount(int count)
        {
            slots.updateList(count);
        }
        public Blueprint CreateBlueprint()
        {
            return blueprints.CreateItem().GetComponent<Blueprint>();
        }
        public void AddBlueprint(Blueprint blueprint)
        {
            InsertBlueprint(blueprints.Count, blueprint);
        }
        public void InsertBlueprint(int index, Blueprint blueprint)
        {
            if (!blueprint)
                return;
            blueprints.Insert(index, blueprint.gameObject);
            blueprint.OnPointerEnter += OnBlueprintPointerEnterCallback;
            blueprint.OnPointerExit += OnBlueprintPointerExitCallback;
            blueprint.OnPointerDown += OnBlueprintPointerDownCallback;
        }
        public void ForceAlign(int index)
        {
            var element = blueprints.getElement(index);
            if (!element)
                return;
            element.transform.localPosition = GetBlueprintLocalPosition(index);
        }
        public bool RemoveBlueprint(Blueprint blueprint)
        {
            if (!blueprint)
                return false;
            if (blueprints.Remove(blueprint.gameObject))
            {
                blueprint.OnPointerEnter -= OnBlueprintPointerEnterCallback;
                blueprint.OnPointerExit -= OnBlueprintPointerExitCallback;
                blueprint.OnPointerDown -= OnBlueprintPointerDownCallback;
                return true;
            }
            return false;
        }
        public void RemoveBlueprintAt(int index)
        {
            RemoveBlueprint(GetBlueprintAt(index));
        }
        public Blueprint GetBlueprintAt(int index)
        {
            return blueprints.getElement<Blueprint>(index);
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
        private void OnBlueprintPointerEnterCallback(Blueprint blueprint, PointerEventData data)
        {
            OnBlueprintPointerEnter?.Invoke(blueprints.indexOf(blueprint), data);
        }
        private void OnBlueprintPointerExitCallback(Blueprint blueprint, PointerEventData data)
        {
            OnBlueprintPointerExit?.Invoke(blueprints.indexOf(blueprint), data);
        }
        private void OnBlueprintPointerDownCallback(Blueprint blueprint, PointerEventData data)
        {
            OnBlueprintPointerDown?.Invoke(blueprints.indexOf(blueprint), data);
        }
        public event Action<int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintPointerDown;
        [SerializeField]
        private ElementList blueprints;
        [SerializeField]
        private ElementList slots;
        [SerializeField]
        private Vector2 offset;
        [SerializeField]
        private Vector2 cellSize;
        [SerializeField]
        private bool horizontal;
        [SerializeField]
        private float alignSpeed = 0.5f;
    }
}
