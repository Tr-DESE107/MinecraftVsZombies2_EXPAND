using System;
using MVZ2.UI;
using MVZ2Logic;
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
        protected void OnBlueprintPointerInteractionCallback(Blueprint blueprint, PointerEventData data, PointerInteraction interaction)
        {
            OnBlueprintPointerInteraction?.Invoke(GetBlueprintIndex(blueprint), data, interaction);
        }
        public event Action<int, PointerEventData, PointerInteraction> OnBlueprintPointerInteraction;
        [SerializeField]
        protected bool horizontal;
    }
}
