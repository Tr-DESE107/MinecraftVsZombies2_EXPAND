using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public class BlueprintList : LevelUIUnit
    {
        public void SetBlueprintCount(int count)
        {
            blueprints.updateList(count, null,
            rect =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                blueprint.OnPointerEnter += OnBlueprintPointerEnterCallback;
                blueprint.OnPointerExit += OnBlueprintPointerExitCallback;
                blueprint.OnPointerDown += OnBlueprintPointerDownCallback;
            },
            rect =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                blueprint.OnPointerEnter -= OnBlueprintPointerEnterCallback;
                blueprint.OnPointerExit -= OnBlueprintPointerExitCallback;
                blueprint.OnPointerDown -= OnBlueprintPointerDownCallback;
            });
        }
        public Blueprint GetBlueprintAt(int index)
        {
            return blueprints.getElement<Blueprint>(index);
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
    }
}
