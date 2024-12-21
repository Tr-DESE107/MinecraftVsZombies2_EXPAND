using System;
using MVZ2.UI;
using MVZ2Logic.Artifacts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class BlueprintChoosePanel : MonoBehaviour
    {
        public void UpdateElements(BlueprintChoosePanelViewData viewData)
        {
            viewLawnButton.gameObject.SetActive(viewData.canViewLawn);
            displayer.SetCommandBlockActive(viewData.hasCommandBlock);
        }
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
            startButton.onClick.AddListener(() => OnStartButtonClick?.Invoke());
            viewLawnButton.onClick.AddListener(() => OnViewLawnButtonClick?.Invoke());
            displayer.OnBlueprintPointerEnter += (index, data) => OnBlueprintPointerEnter?.Invoke(index, data);
            displayer.OnBlueprintPointerExit += (index, data) => OnBlueprintPointerExit?.Invoke(index, data);
            displayer.OnBlueprintPointerDown += (index, data) => OnBlueprintPointerDown?.Invoke(index, data);
            displayer.OnCommandBlockBlueprintClick += () => OnCommandBlockBlueprintClick?.Invoke();
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
        public event Action OnStartButtonClick;
        public event Action OnViewLawnButtonClick;
        public event Action OnCommandBlockBlueprintClick;
        public event Action<int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintPointerDown;
        [SerializeField]
        Button startButton;
        [SerializeField]
        Button viewLawnButton;
        [SerializeField]
        BlueprintDisplayer displayer;
    }
    public struct BlueprintChoosePanelViewData
    {
        public bool canViewLawn;
        public bool hasCommandBlock;
    }
}
