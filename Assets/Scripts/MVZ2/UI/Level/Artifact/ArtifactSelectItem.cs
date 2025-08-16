using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class ArtifactSelectItem : MonoBehaviour, ITooltipTarget, IPointerEnterHandler, IPointerExitHandler
    {
        public void UpdateItem(ArtifactSelectItemViewData viewData)
        {
            iconImage.sprite = viewData.icon;
            iconImage.enabled = iconImage.sprite;
            selectedObj.SetActive(viewData.selected);
            button.interactable = !viewData.disabled;
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this);
        }
        public event Action<ArtifactSelectItem> OnClick;
        public event Action<ArtifactSelectItem> OnPointerEnter;
        public event Action<ArtifactSelectItem> OnPointerExit;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private GameObject selectedObj;
        [SerializeField]
        private Button button;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;

        ITooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;
    }
    public struct ArtifactSelectItemViewData
    {
        public Sprite icon;
        public bool selected;
        public bool disabled;
    }
}
