using System;
using MVZ2.UI;
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
            repickButton.gameObject.SetActive(viewData.canRepick);
            commandBlockSlot.SetCommandBlockActive(viewData.hasCommandBlock);
        }
        public void UpdateCommandBlockItem(ChoosingBlueprintViewData viewData)
        {
            commandBlockSlot.UpdateCommandBlockItem(viewData);
        }
        public void UpdateItems(ChoosingBlueprintViewData[] viewDatas)
        {
            displayer.UpdateItems(viewDatas);
        }
        public Blueprint GetItem(int index)
        {
            return displayer.GetItem(index);
        }
        public Blueprint GetCommandBlockBlueprintItem()
        {
            return commandBlockSlot.GetCommandBlockBlueprint();
        }
        private void Awake()
        {
            startButton.onClick.AddListener(() => OnStartButtonClick?.Invoke());
            viewLawnButton.onClick.AddListener(() => OnViewLawnButtonClick?.Invoke());
            repickButton.onClick.AddListener(() => OnRepickButtonClick?.Invoke());
            displayer.OnBlueprintPointerEnter += (index, data) => OnBlueprintPointerEnter?.Invoke(index, data);
            displayer.OnBlueprintPointerExit += (index, data) => OnBlueprintPointerExit?.Invoke(index, data);
            displayer.OnBlueprintSelect += (index, data) => OnBlueprintPointerDown?.Invoke(index, data);
            commandBlockSlot.OnPointerEnter += () => OnCommandBlockBlueprintPointerEnter?.Invoke();
            commandBlockSlot.OnPointerExit += () => OnCommandBlockBlueprintPointerExit?.Invoke();
            commandBlockSlot.OnClick += () => OnCommandBlockBlueprintClick?.Invoke();
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
        public event Action OnRepickButtonClick;
        public event Action OnCommandBlockBlueprintPointerEnter;
        public event Action OnCommandBlockBlueprintPointerExit;
        public event Action OnCommandBlockBlueprintClick;
        public event Action<int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintPointerDown;
        [SerializeField]
        Button startButton;
        [SerializeField]
        Button viewLawnButton;
        [SerializeField]
        Button repickButton;
        [SerializeField]
        BlueprintDisplayer displayer;
        [SerializeField]
        CommandBlockSlot commandBlockSlot;
    }
    public struct BlueprintChoosePanelViewData
    {
        public bool canViewLawn;
        public bool hasCommandBlock;
        public bool canRepick;
    }
}
