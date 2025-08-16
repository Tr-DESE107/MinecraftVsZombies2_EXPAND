using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class ArtifactSlot : MonoBehaviour, ITooltipTarget, IPointerEnterHandler, IPointerExitHandler
    {
        public void ResetView()
        {
            SetSprite(null);
        }
        public void UpdateView(ArtifactViewData viewData)
        {
            SetSprite(viewData.sprite);
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        private void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
            image.enabled = sprite;
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this);
        }
        public event Action<ArtifactSlot> OnClick;
        public event Action<ArtifactSlot> OnPointerEnter;
        public event Action<ArtifactSlot> OnPointerExit;
        [SerializeField]
        Image image;
        [SerializeField]
        Button button;
        [SerializeField]
        TooltipAnchor tooltipAnchor;
        ITooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;
    }
    public struct ArtifactViewData
    {
        public Sprite sprite;
    }
}
