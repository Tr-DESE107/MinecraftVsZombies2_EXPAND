﻿using MVZ2Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
                page.OnPointerInteraction += OnPointerInteractionCallback;
                page.OnSelect += OnSelectCallback;
            },
            rect =>
            {
                var page = rect.GetComponent<Blueprint>();
                page.OnPointerInteraction -= OnPointerInteractionCallback;
                page.OnSelect -= OnSelectCallback;
            });
        }
        public override Blueprint GetItem(int index)
        {
            return blueprintList.getElement<Blueprint>(index);
        }
        private void OnPointerInteractionCallback(Blueprint blueprint, PointerEventData eventData, PointerInteraction interaction)
        {
            switch (interaction)
            {
                case PointerInteraction.BeginDrag:
                    scrollRect.OnBeginDrag(eventData);
                    return;
                case PointerInteraction.Drag:
                    scrollRect.OnDrag(eventData);
                    return;
                case PointerInteraction.EndDrag:
                    scrollRect.OnEndDrag(eventData);
                    return;
            }
            var index = blueprintList.indexOf(blueprint);
            CallBlueprintPointerInteraction(index, eventData, interaction);
        }
        private void OnSelectCallback(Blueprint blueprint)
        {
            var index = blueprintList.indexOf(blueprint);
            CallBlueprintSelect(index);
        }
        [Header("Mobile")]
        [SerializeField]
        ScrollRect scrollRect;
        [SerializeField]
        ElementListUI blueprintList;
    }
}
