using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public class BlueprintChoosePanelPage : MonoBehaviour
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
        public Blueprint GetItem(int index)
        {
            return blueprintList.getElement<Blueprint>(index);
        }
        private void OnBlueprintPointerEnterCallback(Blueprint blueprint, PointerEventData eventData)
        {
            OnBlueprintPointerEnter?.Invoke(this, blueprintList.indexOf(blueprint), eventData);
        }
        private void OnBlueprintPointerExitCallback(Blueprint blueprint, PointerEventData eventData)
        {
            OnBlueprintPointerExit?.Invoke(this, blueprintList.indexOf(blueprint), eventData);
        }
        private void OnBlueprintPointerDownCallback(Blueprint blueprint, PointerEventData eventData)
        {
            OnBlueprintPointerDown?.Invoke(this, blueprintList.indexOf(blueprint), eventData);
        }
        public event Action<BlueprintChoosePanelPage, int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<BlueprintChoosePanelPage, int, PointerEventData> OnBlueprintPointerExit;
        public event Action<BlueprintChoosePanelPage, int, PointerEventData> OnBlueprintPointerDown;
        [SerializeField]
        ElementListUI blueprintList;
    }
}
