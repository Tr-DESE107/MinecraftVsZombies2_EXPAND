using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public abstract class BlueprintSet : MonoBehaviour
    {
        public abstract Blueprint CreateBlueprint();
        public abstract void InsertBlueprint(int index, Blueprint blueprint);
        public abstract bool RemoveBlueprint(Blueprint blueprint);
        public abstract bool DestroyBlueprint(Blueprint blueprint);
        public void RemoveBlueprintAt(int index)
        {
            RemoveBlueprint(GetBlueprintAt(index));
        }
        public void DestroyBlueprintAt(int index)
        {
            DestroyBlueprint(GetBlueprintAt(index));
        }
        public abstract Blueprint GetBlueprintAt(int index);
        public abstract int GetBlueprintIndex(Blueprint value);
        protected void OnBlueprintPointerEnterCallback(Blueprint blueprint, PointerEventData data)
        {
            OnBlueprintPointerEnter?.Invoke(GetBlueprintIndex(blueprint), data);
        }
        protected void OnBlueprintPointerExitCallback(Blueprint blueprint, PointerEventData data)
        {
            OnBlueprintPointerExit?.Invoke(GetBlueprintIndex(blueprint), data);
        }
        protected void OnBlueprintPointerDownCallback(Blueprint blueprint, PointerEventData data)
        {
            OnBlueprintPointerDown?.Invoke(GetBlueprintIndex(blueprint), data);
        }
        public event Action<int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintPointerDown;
        [SerializeField]
        protected bool horizontal;
    }
}
