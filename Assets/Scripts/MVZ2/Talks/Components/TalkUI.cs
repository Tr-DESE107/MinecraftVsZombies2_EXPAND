using System;
using System.Collections.Generic;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public void SetCharacterWidthExtend(int index, Vector2 extend)
        {
            var character = GetCharacter(index);
            character.SetWidthExtend(extend);
        }
        public void SetCharacterSpeaking(int index, bool value)
        {
            var character = GetCharacter(index);
            character.SetSpeaking(value);
        }
        public void SetCharacterToTheFirstLayer(int index)
        {
            var character = GetCharacter(index);
            character.transform.SetAsLastSibling();
        }
        public void CharacterDisappear(int index, float disappearSpeed)
        {
            var character = GetCharacter(index);
            character.SetDisappear(true);
            character.SetDisappearSpeed(disappearSpeed);
            RemoveCharacterAt(index);
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
        public float GetForegroundAlpha()
        {
            return foregroundFader.Value;
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

        #region 背景图
        public void SetBackgroundSprite(Sprite sprite)
        {
            backgroundImage.sprite = sprite;
        }
        public void SetBackgroundAlpha(float value)
        {
            backgroundFader.Value = value;
        }
        public float GetBackgroundAlpha()
        {
            return backgroundFader.Value;
        }
        public void StartBackgroundFade(float target, float duration)
        {
            backgroundFader.StartFade(target, duration);
        }
        #endregion

        #region 背景色
        public void SetBackcolor(Color value)
        {
            backcolorFader.Value = value;
        }
        public void StartBackcolorFade(Color target, float duration)
        {
            backcolorFader.StartFade(target, duration);
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

        public void SetShake(Vector3 shake)
        {
            foreach (var root in shakeRoots)
            {
                if (!root)
                    continue;
                root.localPosition = shake;
            }
        }

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
            raycastReceiver.OnPointerDown += OnRaycastReceiverPointerDownCallback;
            foregroundFader.OnValueChanged += OnForegroundAlphaChangedCallback;
            forecolorFader.OnValueChanged += OnForegroundColorChangedCallback;
            backgroundFader.OnValueChanged += OnBackgroundAlphaChangedCallback;
            backcolorFader.OnValueChanged += OnBackgroundColorChangedCallback;
        }
        #endregion

        #region 事件回调
        private void OnRaycastReceiverPointerDownCallback(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
        private void OnForegroundAlphaChangedCallback(float value)
        {
            foregroundCanvasGroup.alpha = value;
        }
        private void OnForegroundColorChangedCallback(Color value)
        {
            var color = forecolorImage.color;
            color.r = value.r;
            color.g = value.g;
            color.b = value.b;
            color.a = value.a;
            forecolorImage.color = color;
        }
        private void OnBackgroundAlphaChangedCallback(float value)
        {
            backgroundCanvasGroup.alpha = value;
        }
        private void OnBackgroundColorChangedCallback(Color value)
        {
            var color = backcolorImage.color;
            color.r = value.r;
            color.g = value.g;
            color.b = value.b;
            color.a = value.a;
            backcolorImage.color = color;
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

        [Header("Foreground")]
        [SerializeField]
        private CanvasGroup foregroundCanvasGroup;
        [SerializeField]
        private Image foregroundImage;
        [SerializeField]
        private FloatFader foregroundFader;
        [SerializeField]
        private Image forecolorImage;
        [SerializeField]
        private ColorFader forecolorFader;

        [Header("Background")]
        [SerializeField]
        private CanvasGroup backgroundCanvasGroup;
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private FloatFader backgroundFader;
        [SerializeField]
        private Image backcolorImage;
        [SerializeField]
        private ColorFader backcolorFader;
        [SerializeField]
        private Transform[] shakeRoots;
        private List<TalkCharacterController> characters = new List<TalkCharacterController>();
        #endregion 属性
    }
}