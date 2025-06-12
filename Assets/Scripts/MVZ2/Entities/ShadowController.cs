using UnityEngine;

namespace MVZ2.Entities
{
    public class ShadowController : MonoBehaviour
    {
        public void SetAlpha(float value)
        {
            Color col = shadowRenderer.color;
            shadowRenderer.color = new Color(col.r, col.g, col.b, value);
        }
        [SerializeField]
        private SpriteRenderer shadowRenderer;
    }
}