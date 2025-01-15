using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Level
{
    public class PoolColorSetter : MonoBehaviour
    {
        private void OnEnable()
        {
            if (!poolRenderer)
                return;
            var main = MainManager.Instance;
            var color = main.OptionsManager.HasBloodAndGore() ? normalColor : censoredColor;
            poolRenderer.color = color;
        }
        [SerializeField]
        private SpriteRenderer poolRenderer;
        [SerializeField]
        private Color normalColor;
        [SerializeField]
        private Color censoredColor;
    }
}
