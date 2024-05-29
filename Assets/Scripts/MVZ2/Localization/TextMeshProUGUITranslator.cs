using TMPro;
using UnityEngine;

namespace MVZ2.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProUGUITranslator : TranslateComponent<TextMeshProUGUI>
    {
        protected override string GetKeyInner()
        {
            return Component.text;
        }
        protected override void Translate(string language)
        {
            base.Translate(language);
            if (!string.IsNullOrEmpty(Context))
                Component.text = lang._p(Context, Key);
            else
                Component.text = lang._(Key);
        }
    }
}
