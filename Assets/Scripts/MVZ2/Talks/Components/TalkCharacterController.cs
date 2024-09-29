using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Talk
{
    public class TalkCharacterController : MonoBehaviour
    {
        #region 公有方法
        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
        public void SetSpeaking(bool value)
        {
            speaking = value;
        }
        public void SetLeaving(bool value)
        {
            leaving = value;
        }
        public void SetDisappear(bool value)
        {
            _animator.SetBool("Disappear", value);
        }
        public void SetDisappearSpeed(float value)
        {
            _animator.SetFloat("DisappearSpeed", value);
        }
        public bool IsLeft()
        {
            return transform.localScale.x < 0;
        }
        public void SetCharacter(Sprite spr)
        {
            image.sprite = spr;
            image.SetNativeSize();
            imageRectTransform.pivot = spr.pivot / spr.rect.size;
        }
        private void Update()
        {
            if (leaving)
            {
                blendValue = Mathf.Lerp(blendValue, 0, idleBlendFactor);
            }
            else if (speaking)
            {
                blendValue = Mathf.Lerp(blendValue, 1, idleBlendFactor);
            }
            else
            {
                blendValue = Mathf.Lerp(blendValue, idleBlendValue, idleBlendFactor);
            }
            _animator.SetFloat("Blend", blendValue);
        }
        #endregion

        #region 属性字段
        private bool speaking;
        private bool leaving;
        private float blendValue = 0;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Image image;
        [SerializeField]
        private RectTransform imageRectTransform;
        [SerializeField]
        private float idleBlendFactor = 0.2f;
        [SerializeField]
        private float idleBlendValue = 0.8f;
        #endregion

    }

    public enum CharacterSide
    {
        Left,
        Right
    }

    public enum CharacterState
    {
        None,
        Enter,
        Silence,
        Speaking,
        Leaving,
        Disappear,
    }
}