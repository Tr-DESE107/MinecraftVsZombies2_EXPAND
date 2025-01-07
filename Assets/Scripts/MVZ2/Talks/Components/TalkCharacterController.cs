using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Talk
{
    public class TalkCharacterController : MonoBehaviour
    {
        #region 公有方法
        public void UpdateCharacter(TalkCharacterViewData viewData)
        {
            Vector3 scale = Vector3.zero;
            switch (viewData.side)
            {
                case CharacterSide.Left:
                    scale = new Vector3(-1, 1, 1);
                    break;
                case CharacterSide.Right:
                    scale = new Vector3(1, 1, 1);
                    break;
            }
            SetScale(scale);
            SetCharacter(viewData.sprite);
            gameObject.name = viewData.name;

            var widthExtend = viewData.widthExtend;
            var anchoredPos = imageRectTransform.anchoredPosition;
            var sizeDelta = imageRectTransform.sizeDelta;
            anchoredPos.x -= (widthExtend.x - widthExtend.y) * 0.5f;
            sizeDelta.x += widthExtend.x + widthExtend.y;
            imageRectTransform.anchoredPosition = anchoredPos;
            imageRectTransform.sizeDelta = sizeDelta;
        }
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
            disappearing = value;
        }
        public void SetDisappearSpeed(float value)
        {
            disappearSpeed = value;
        }
        public void SetCharacter(Sprite spr)
        {
            image.sprite = spr;
            if (spr)
                imageRectTransform.pivot = spr.pivot / spr.rect.size;
        }
        public void ResetMotion()
        {
            blendValue = 0;
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
            if (disappearing)
            {
                disappearBlend = Mathf.Clamp01(disappearBlend + disappearSpeed * Time.deltaTime);
            }
            _animator.SetFloat("Blend", blendValue);
            _animator.SetFloat("DisappearBlend", disappearBlend);
            if ((leaving && blendValue <= 0.01f) || (disappearing && disappearBlend >= 1))
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region 属性字段
        private bool speaking;
        private bool leaving;
        private bool disappearing;
        private float disappearSpeed;
        private float disappearBlend;
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
        None,
        Left,
        Right,
        Self
    }
    public struct TalkCharacterViewData
    {
        public string name;
        public Sprite sprite;
        public Vector2 widthExtend;
        public CharacterSide side;
    }
}