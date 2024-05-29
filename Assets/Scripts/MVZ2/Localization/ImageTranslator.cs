using PVZEngine;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Localization
{
    [RequireComponent(typeof(Image))]
    public class ImageTranslator : TranslateComponent<Image>
    {
        protected override void Translate(string language)
        {
            base.Translate(language);
            if (isSheet)
            {
                var sheet = lang.GetLocalizedSpriteSheet(spriteID, language) ?? MainManager.Instance.ResourceManager.GetSpriteSheet(spriteID);
                if (sheet == null)
                    return;
                Component.sprite = sheet[sheetIndex];
            }
            else
            {
                Component.sprite = lang.GetLocalizedSprite(spriteID, language) ?? MainManager.Instance.ResourceManager.GetSprite(spriteID);
            }
        }
        [SerializeField]
        private NamespaceID spriteID;
        [SerializeField]
        private bool isSheet;
        [SerializeField]
        private int sheetIndex;
    }
}
