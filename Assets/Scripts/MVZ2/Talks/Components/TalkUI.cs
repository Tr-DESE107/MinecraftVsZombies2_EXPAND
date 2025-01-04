using System;
using System.Collections.Generic;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Talk
{
    public class TalkUI : MonoBehaviour
    {
        #region 公有方法

        #region 角色
        public void CreateCharacter(TalkCharacterViewData viewData)
        {
            var chr = Instantiate(characterTemplate, characterRoot).GetComponent<TalkCharacterController>();
            chr.gameObject.SetActive(true);
            chr.UpdateCharacter(viewData);
            characters.Add(chr);
        }
        public void RemoveCharacterAt(int index)
        {
            characters.RemoveAt(index);
        }
        public void LeaveCharacterAt(int index)
        {
            var chr = GetCharacter(index);
            RemoveCharacterAt(index);
            chr.SetLeaving(true);
        }
        public void DestroyCharacterAt(int index)
        {
            var chr = GetCharacter(index);
            RemoveCharacterAt(index);
            Destroy(chr.gameObject);
        }
        public void SetCharacterSprite(int index, Sprite sprite)
        {
            var character = GetCharacter(index);
            character.SetCharacter(sprite);
        }
        public void SetCharacterSpeaking(int index, bool value)
        {
            var character = GetCharacter(index);
            character.SetSpeaking(value);
        }
        public void CharacterDisappear(int index, float disappearSpeed)
        {
            var character = GetCharacter(index);
            character.SetDisappear(true);
            character.SetDisappearSpeed(disappearSpeed);
        }
        public void StopCharacterDisappear(int index)
        {
            var character = GetCharacter(index);
            character.SetDisappear(false);
            character.SetDisappearSpeed(1);
        }
        public void ClearCharacters()
        {
            foreach (var chr in characters)
            {
                Destroy(chr.gameObject);
            }
            characters.Clear();
        }
        #endregion


        #region 对话气泡
        public void SetSpeechBubbleShowing(bool value)
        {
            speechBubble.SetShowing(value);
        }
        public void SetSpeechBubbleText(string text)
        {
            speechBubble.SetText(text);
        }
        public void SetSpeechBubbleDirection(SpeechBubbleDirection direction)
        {
            speechBubble.SetDirection(direction);
        }
        public void ForceReshowSpeechBubble()
        {
            speechBubble.ForceReshow();
        }
        #endregion
        public void SetSkipButtonActive(bool value)
        {
            skipButton.gameObject.SetActive(value);
        }
        public void SetBlockerActive(bool value)
        {
            blockerObject.SetActive(value);
        }
        public void SetRaycastReceiverActive(bool value)
        {
            raycastReceiver.gameObject.SetActive(value);
        }

        #region 前景图
        public void SetForegroundSprite(Sprite sprite)
        {
            foregroundImage.sprite = sprite;
        }
        public void SetForegroundAlpha(float value)
        {
            foregroundFader.Value = value;
        }
        public void StartForegroundFade(float target, float duration)
        {
            foregroundFader.StartFade(target, duration);
        }
        #endregion

        #region 前景色
        public void SetForecolor(Color value)
        {
            forecolorFader.Value = value;
        }
        public void StartForecolorFade(Color target, float duration)
        {
            forecolorFader.StartFade(target, duration);
        }
        #endregion

        #region 展示物品
        public void ShowTalkItem(Sprite sprite)
        {
            talkItem.ForceShow();
            talkItem.SetShowing(true);
            talkItem.SetSprite(sprite);
        }
        public void HideTalkItem()
        {
            talkItem.SetShowing(false);
        }
        #endregion

        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            characterTemplate.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            blockerObject.SetActive(false);
            raycastReceiver.gameObject.SetActive(false);
            SetSkipButtonActive(false);

            skipButton.onClick.AddListener(() => OnSkipClick?.Invoke());
            raycastReceiver.OnPointerDown += (eventData) => OnClick?.Invoke();
            foregroundFader.OnValueChanged += OnForegroundAlphaChangedCallback;
            forecolorFader.OnValueChanged += OnForegroundColorChangedCallback;
        }
        #endregion

        #region 事件回调
        private void OnForegroundAlphaChangedCallback(float value)
        {
            foregroundCanvasGroup.alpha = value;
        }
        private void OnForegroundColorChangedCallback(Color value)
        {
            var color = foregroundImage.color;
            color.r = value.r;
            color.g = value.g;
            color.b = value.b;
            foregroundImage.color = color;
        }
        #endregion

        private TalkCharacterController GetCharacter(int index)
        {
            return characters[index];
        }

        #endregion

        #region 事件
        public event Action OnClick;
        public event Action OnSkipClick;
        #endregion 动作

        #region 属性字段
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private GameObject characterTemplate;
        [SerializeField]
        private TalkItem talkItem;
        [SerializeField]
        private Transform characterRoot;
        [SerializeField]
        private SpeechBubble speechBubble;
        [SerializeField]
        private Button skipButton;
        [SerializeField]
        private RaycastReceiver raycastReceiver;
        [SerializeField]
        private GameObject blockerObject;
        [SerializeField]
        private Image foregroundImage;
        [SerializeField]
        private CanvasGroup foregroundCanvasGroup;
        [SerializeField]
        private FloatFader foregroundFader;
        [SerializeField]
        private ColorFader forecolorFader;
        private List<TalkCharacterController> characters = new List<TalkCharacterController>();
        #endregion 属性
    }
}