using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public abstract class BlueprintSet : MonoBehaviour
    {
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
        public bool DestroyBlueprint(Blueprint blueprint)
        {
            if (!blueprint)
                return false;
            if (blueprints.DestroyItem(blueprint.gameObject))
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
        public void DestroyBlueprintAt(int index)
        {
            DestroyBlueprint(GetBlueprintAt(index));
        }
        public Blueprint GetBlueprintAt(int index)
        {
            return blueprints.getElement<Blueprint>(index);
        }
        public int GetBlueprintIndex(Blueprint value)
        {
            return blueprints.indexOf(value);
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
        protected ElementList blueprints;
        [SerializeField]
        protected bool horizontal;
    }
}
