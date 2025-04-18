using UnityEngine;

namespace MVZ2.ChapterTransition
{
    [ExecuteAlways]
    public class ChapterTransitionBlurer : MonoBehaviour
    {
        private void Update()
        {
            SetTitleBlur(titleBlur);
        }
        public void SetTitleBlur(float blur)
        {
            propertyBlock.Clear();
            titleRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_Blur", blur);
            titleRenderer.SetPropertyBlock(propertyBlock);
        }
        private MaterialPropertyBlock propertyBlock
        {
            get
            {
                if (_propertyBlock == null)
                    _propertyBlock = new MaterialPropertyBlock();
                return _propertyBlock;
            }
        }
        private MaterialPropertyBlock _propertyBlock;
        [SerializeField]
        private SpriteRenderer titleRenderer;
        [SerializeField]
        [Range(0, 1)]
        private float titleBlur;
    }
}
