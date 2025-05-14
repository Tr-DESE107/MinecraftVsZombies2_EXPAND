using MVZ2.UI;
using TMPro;
using UnityEngine;

namespace MVZ2.Models
{
    public class BlueprintSprite : MonoBehaviour
    {
        public void UpdateView(BlueprintViewData viewData)
        {
            costText.text = viewData.cost;
            triggerCostRoot.SetActive(viewData.triggerActive);
            foreach (var preset in normalPresets)
            {
                preset.SetActive(viewData.preset == BlueprintPreset.Normal);
            }
            foreach (var preset in upgradePresets)
            {
                preset.SetActive(viewData.preset == BlueprintPreset.Upgrade);
            }
            foreach (var preset in commandBlockPresets)
            {
                preset.SetActive(viewData.preset == BlueprintPreset.CommandBlock);
            }
            var icon = viewData.icon;
            iconRenderer.enabled = icon && !viewData.iconGrayscale;
            iconRenderer.sprite = icon;
            iconCommandBlockRenderer.enabled = icon && viewData.iconGrayscale;
            iconCommandBlockRenderer.sprite = icon;
            UpdateIcon();
        }
        protected void UpdateIcon()
        {
            var iconSprite = iconRenderer.sprite;
            if (!iconSprite)
                return;
            var spriteScale = new Vector2(iconSprite.rect.width / iconSpriteSize.x, iconSprite.rect.height / iconSpriteSize.y);
            Vector3 iconScale;
            if (lockAspect)
            {
                var maxScale = Mathf.Max(spriteScale.x, spriteScale.y);
                iconScale = Vector3.one * (1 / maxScale);
            }
            else
            {
                iconScale = new Vector3(1 / spriteScale.x, 1 / spriteScale.y);
            }
            iconRoot.localScale = iconScale;
        }
        [SerializeField]
        protected Transform iconRoot;
        [SerializeField]
        protected SpriteRenderer iconRenderer;
        [SerializeField]
        protected SpriteRenderer iconCommandBlockRenderer;
        [SerializeField]
        protected GameObject triggerCostRoot;
        [SerializeField]
        protected TextMeshPro costText;
        [SerializeField]
        protected GameObject[] normalPresets;
        [SerializeField]
        protected GameObject[] upgradePresets;
        [SerializeField]
        protected GameObject[] commandBlockPresets;
        [SerializeField]
        protected Vector2 iconSpriteSize;
        [SerializeField]
        protected bool lockAspect;
    }
}
