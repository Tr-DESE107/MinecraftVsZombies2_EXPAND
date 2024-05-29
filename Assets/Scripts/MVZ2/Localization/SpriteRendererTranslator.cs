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
            Component.sprite = lang.GetLocalizedSprite(spriteID, language) ?? MainManager.Instance.ResourceManager.GetSprite(spriteID);
        }
        [SerializeField]
        private NamespaceID spriteID;
    }
}
