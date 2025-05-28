using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class CommandBlockChoosePanel : MonoBehaviour
    {
        public void UpdateItems(ChoosingBlueprintViewData[] viewDatas)
        {
            displayer.UpdateItems(viewDatas);
        }
        public Blueprint GetItem(int index)
        {
            return displayer.GetItem(index);
        }
        private void Awake()
        {
            cancelButton.onClick.AddListener(() => OnCancelButtonClick?.Invoke());
            displayer.OnBlueprintPointerEnter += (index, data) => OnBlueprintPointerEnter?.Invoke(index, data);
            displayer.OnBlueprintPointerExit += (index, data) => OnBlueprintPointerExit?.Invoke(index, data);
            displayer.OnBlueprintSelect += (index, data) => OnBlueprintPointerDown?.Invoke(index, data);
        }
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
            OnBlueprintPointerDown?.Invoke(index, eventData);
        }
        public event Action OnCancelButtonClick;
        public event Action<int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintPointerDown;
        [SerializeField]
        Button cancelButton;
        [SerializeField]
        BlueprintDisplayer displayer;
    }
}
