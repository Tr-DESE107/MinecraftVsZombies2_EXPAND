using PVZEngine;
using UnityEngine;

namespace MVZ2.Localization
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererTranslator : TranslateComponent<SpriteRenderer>
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
