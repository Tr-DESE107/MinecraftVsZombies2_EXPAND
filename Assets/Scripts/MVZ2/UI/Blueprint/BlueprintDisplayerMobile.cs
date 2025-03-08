using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class BlueprintDisplayerMobile : BlueprintDisplayer
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
                page.OnPointerClick += OnBlueprintPointerClickCallback;
            },
            rect =>
            {
                var page = rect.GetComponent<Blueprint>();
                page.OnPointerEnter -= OnBlueprintPointerEnterCallback;
                page.OnPointerExit -= OnBlueprintPointerExitCallback;
                page.OnPointerClick -= OnBlueprintPointerClickCallback;
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
        private void OnBlueprintPointerClickCallback(Blueprint blueprint, PointerEventData eventData)
        {
            CallBlueprintPointerDown(blueprintList.indexOf(blueprint), eventData);
        }
        [Header("Mobile")]
        [SerializeField]
        ElementListUI blueprintList;
    }
}
