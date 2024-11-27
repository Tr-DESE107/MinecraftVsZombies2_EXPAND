using MVZ2.UI;
using TMPro;
using UnityEngine;

namespace MVZ2.Models
{
    public class BlueprintSprite : MonoBehaviour
    {
        public void UpdateView(BlueprintViewData viewData)
        {
            iconRenderer.sprite = viewData.icon;
            costText.text = viewData.cost;
            triggerCostRoot.SetActive(viewData.triggerActive);
            UpdateIcon();
        }
        protected void UpdateIcon()
        {
            var iconSprite = iconRenderer.sprite;
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
            iconRenderer.transform.localScale = iconScale;
        }
        [SerializeField]
        protected SpriteRenderer iconRenderer;
        [SerializeField]
        protected GameObject triggerCostRoot;
        [SerializeField]
        protected TextMeshPro costText;
        [SerializeField]
        protected Vector2 iconSpriteSize;
        [SerializeField]
        protected bool lockAspect;
    }
}
