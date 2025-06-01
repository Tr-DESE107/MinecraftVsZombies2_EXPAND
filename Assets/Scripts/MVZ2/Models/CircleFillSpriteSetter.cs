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
            UpdateCircleFill();
        }
        private void OnDisable()
        {
            ResetCircleFill();

            spriteRenderer = null;
            propertyBlock = null;
        }
        private void LateUpdate()
        {
            UpdateCircleFill();
        }
        private void ResetCircleFill()
        {
            SetCircleFill(0);
        }
        private void UpdateCircleFill()
        {
            SetCircleFill(fill);
        }
        private void SetCircleFill(float fill)
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
