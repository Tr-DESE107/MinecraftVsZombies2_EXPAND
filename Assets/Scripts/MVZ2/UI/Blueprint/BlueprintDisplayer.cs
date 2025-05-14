using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public abstract class BlueprintDisplayer : MonoBehaviour
    {
        public abstract void UpdateItems(ChoosingBlueprintViewData[] viewDatas);
        public abstract Blueprint GetItem(int index);
        protected void CallBlueprintPointerEnter(int index, PointerEventData eventData)
        {
            OnBlueprintPointerEnter?.Invoke(index, eventData);
        }
        protected void CallBlueprintPointerExit(int index, PointerEventData eventData)
        {
            OnBlueprintPointerExit?.Invoke(index, eventData);
        }
        protected void CallBlueprintPointerDown(int index, PointerEventData eventData)
        {
            OnBlueprintSelect?.Invoke(index, eventData);
        }
        public event Action<int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintSelect;
    }
    public struct ChoosingBlueprintViewData
    {
        public BlueprintViewData blueprint;
        public bool disabled;
        public bool selected;

        public static readonly ChoosingBlueprintViewData Empty = new ChoosingBlueprintViewData()
        {
            blueprint = BlueprintViewData.Empty
        };
    }
}
