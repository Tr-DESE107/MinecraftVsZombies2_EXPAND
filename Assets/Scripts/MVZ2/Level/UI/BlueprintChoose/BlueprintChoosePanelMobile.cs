using System.Linq;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class BlueprintChoosePanelMobile : BlueprintChoosePanel
    {
        public override void UpdateItems(ChoosingBlueprintViewData[] viewDatas)
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
                var page = rect.GetComponent<Blueprint>();
                page.OnPointerEnter += OnBlueprintPointerEnterCallback;
                page.OnPointerExit += OnBlueprintPointerExitCallback;
                page.OnPointerDown += OnBlueprintPointerDownCallback;
            },
            rect =>
            {
                var page = rect.GetComponent<Blueprint>();
                page.OnPointerEnter -= OnBlueprintPointerEnterCallback;
                page.OnPointerExit -= OnBlueprintPointerExitCallback;
                page.OnPointerDown -= OnBlueprintPointerDownCallback;
            });
        }
        public override Blueprint GetItem(int index)
        {
            return blueprintList.getElement<Blueprint>(index);
        }
        private void OnBlueprintPointerEnterCallback(Blueprint blueprint, PointerEventData eventData)
        {
            CallBlueprintPointerEnter(blueprintList.indexOf(blueprint), eventData);
        }
        private void OnBlueprintPointerExitCallback(Blueprint blueprint, PointerEventData eventData)
        {
            CallBlueprintPointerExit(blueprintList.indexOf(blueprint), eventData);
        }
        private void OnBlueprintPointerDownCallback(Blueprint blueprint, PointerEventData eventData)
        {
            CallBlueprintPointerDown(blueprintList.indexOf(blueprint), eventData);
        }
        [Header("Mobile")]
        [SerializeField]
        ElementListUI blueprintList;
    }
}
