using System;
using MVZ2.UI;
using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public class BlueprintDisplayerStandalonePage : MonoBehaviour
    {
        public void UpdateItems(ChoosingBlueprintViewData[] viewDatas)
        {
            blueprintList.updateList(viewDatas.Length, (i, rect) =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                blueprint.UpdateView(viewDatas[i].blueprint);
                blueprint.SetDisabled(viewDatas[i].disabled || viewDatas[i].selected);
                blueprint.SetRecharge(viewDatas[i].selected ? 1 : 0);
            },
            rect =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                blueprint.OnPointerInteraction += OnBlueprintPointerInteractionCallback;
                blueprint.OnSelect += OnBlueprintSelectCallback;
            },
            rect =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                blueprint.OnPointerInteraction -= OnBlueprintPointerInteractionCallback;
                blueprint.OnSelect -= OnBlueprintSelectCallback;
            });
        }
        public Blueprint GetItem(int index)
        {
            return blueprintList.getElement<Blueprint>(index);
        }
        private void OnBlueprintPointerInteractionCallback(Blueprint blueprint, PointerEventData eventData, PointerInteraction interaction)
        {
            OnBlueprintPointerInteraction?.Invoke(this, blueprintList.indexOf(blueprint), eventData, interaction);
        }
        private void OnBlueprintSelectCallback(Blueprint blueprint)
        {
            OnBlueprintSelect?.Invoke(this, blueprintList.indexOf(blueprint));
        }
        public event Action<BlueprintDisplayerStandalonePage, int, PointerEventData, PointerInteraction> OnBlueprintPointerInteraction;
        public event Action<BlueprintDisplayerStandalonePage, int> OnBlueprintSelect;
        [SerializeField]
        ElementListUI blueprintList;
    }
}
