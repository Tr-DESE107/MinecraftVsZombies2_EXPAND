using UnityEngine;

namespace MVZ2.Level.UI
{
    public class HeldItem : MonoBehaviour
    {
        public void SetIcon(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
        [SerializeField]
        private SpriteRenderer spriteRenderer;
    }
}
