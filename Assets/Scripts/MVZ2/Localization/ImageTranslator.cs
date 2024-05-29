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
            Component.sprite = lang.GetLocalizedSprite(spriteID, language) ?? MainManager.Instance.ResourceManager.GetSprite(spriteID);
        }
        [SerializeField]
        private NamespaceID spriteID;
    }
}
