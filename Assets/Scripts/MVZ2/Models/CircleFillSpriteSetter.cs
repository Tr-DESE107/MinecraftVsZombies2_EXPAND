using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CircleFillSpriteSetter : MonoBehaviour
    {
        private void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            propertyBlock = new MaterialPropertyBlock();
        }
        private void OnDisable()
        {
            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_CircleFill", 0);
            spriteRenderer.SetPropertyBlock(propertyBlock);

            spriteRenderer = null;
            propertyBlock = null;
        }
        private void Update()
        {
            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_CircleFill", fill);
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
        [Range(0, 1)]
        public float fill;
        private SpriteRenderer spriteRenderer;
        private MaterialPropertyBlock propertyBlock;
    }
}
