using UnityEngine;

namespace MVZ2.ChapterTransition
{
    public class ChapterTransition : MonoBehaviour
    {
        public void SetWheelRootRotation(float rotation)
        {
            wheelRoot.eulerAngles = new Vector3(0, 0, rotation);
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
        private Transform wheelRoot;
        [SerializeField]
        private SpriteRenderer titleRenderer;
    }
}
