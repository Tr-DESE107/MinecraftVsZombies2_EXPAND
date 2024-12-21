using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class ArtifactSelectItem : MonoBehaviour, ITooltipTarget
    {
        public void UpdateItem(ArtifactSelectItemViewData viewData)
        {
            iconImage.sprite = viewData.icon;
            iconImage.enabled = iconImage.sprite;
            button.interactable = !viewData.disabled;
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        public event Action<ArtifactSelectItem> OnClick;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private Button button;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;

        TooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;
    }
    public struct ArtifactSelectItemViewData
    {
        public Sprite icon;
        public bool disabled;
    }
}
