using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class LocalSpriteRectSetter : MonoBehaviour
    {
        private void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            propertyBlock = new MaterialPropertyBlock();

            UpdateRect();
        }
        private void OnDisable()
        {
            ResetRect();

            spriteRenderer = null;
            propertyBlock = null;
            lastSprite = null;
        }
        private void LateUpdate()
        {
            Sprite sprite = spriteRenderer.sprite;
            if (sprite == lastSprite)
                return;
            lastSprite = sprite;
            UpdateRect();
        }
        private void UpdateRect()
        {
            Sprite sprite = spriteRenderer.sprite;
            if (!sprite)
            {
                ResetRect();
                return;
            }
            Vector4 result = new Vector4(
            sprite.textureRect.min.x / sprite.texture.width,
            sprite.textureRect.min.y / sprite.texture.height,
            sprite.textureRect.max.x / sprite.texture.width,
            sprite.textureRect.max.y / sprite.texture.height);

            SetRect(result);
        }
        private void ResetRect()
        {
            SetRect(new Vector4(0, 0, 1, 1));
        }
        private void SetRect(Vector4 vector)
        {
            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector("_LocalRect", vector);
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
        private SpriteRenderer spriteRenderer;
        private Sprite lastSprite;
        private MaterialPropertyBlock propertyBlock;
    }
}
