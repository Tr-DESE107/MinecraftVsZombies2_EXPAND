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
            _animator.SetBool("Speaking", value);
        }
        public void SetLeaving(bool value)
        {
            _animator.SetBool("Leaving", value);
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
            //image.SetNativeSize();
            //(transform as RectTransform).pivot = spr.pivot;
        }
        public void SetStartPosition(Position position)
        {
            positionTransition.setStartTransform(GetTransformByPosition(position));
        }
        public void SetTargetPosition(Position position)
        {
            positionTransition.setTargetTransform(GetTransformByPosition(position));
        }
        #endregion
        private Transform GetTransformByPosition(Position position)
        {
            switch (position)
            {
                case Position.Start:
                    return startTransform;
                case Position.Idle:
                    return idleTransform;
                case Position.Speak:
                    return speakTransform;
            }
            return null;
        }

        #region 属性字段
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Image image;

        [Header("Animator")]
        [SerializeField]
        private PositionTransition positionTransition;
        [SerializeField]
        private Transform startTransform;
        [SerializeField]
        private Transform idleTransform;
        [SerializeField]
        private Transform speakTransform;
        #endregion
        public enum Position
        {
            Start,
            Idle,
            Speak
        }

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