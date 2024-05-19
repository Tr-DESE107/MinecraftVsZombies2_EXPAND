using UnityEngine;

namespace MVZ2
{
    public class ShadowController : MonoBehaviour
    {
        public void SetAlpha(float value)
        {
            Color col = shadowRenderer.color;
            shadowRenderer.color = new Color(col.r, col.g, col.b, value);
        }
        private void Awake()
        {
            shadowRenderer = GetComponent<SpriteRenderer>();
        }
        private SpriteRenderer shadowRenderer;
    }
}