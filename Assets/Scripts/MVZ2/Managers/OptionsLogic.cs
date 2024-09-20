using System;
using MukioI18n;
using MVZ2.UI;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    using static MVZ2.UI.OptionsDialog;
    public abstract class OptionsLogic : IDisposable
    {
        public OptionsLogic(OptionsDialog dialog)
        {
            this.dialog = dialog;

            dialog.OnButtonClick += OnButtonClickCallback;
            dialog.OnSliderValueChanged += OnSliderValueChangedCallback;
            dialog.OnDropdownValueChanged += OnDropdownValueChangedCallback;
        }
        public virtual void InitDialog()
        {
            UpdateAllElements();
        }
        public virtual void Dispose()
        {
            dialog.OnButtonClick -= OnButtonClickCallback;
            dialog.OnSliderValueChanged -= OnSliderValueChangedCallback;
            dialog.OnDropdownValueChanged -= OnDropdownValueChangedCallback;
        }
        protected virtual void OnButtonClickCallback(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Back:
                    OnClose?.Invoke();
                    break;
                case ButtonType.SwapTrigger:
                    {
                        Main.OptionsManager.SwitchSwapTrigger();
                        UpdateSwapTriggerButton();
                    }
                    break;
                case ButtonType.Fullscreen:
                    {
                        Main.OptionsManager.SwitchFullscreen();
                        UpdateFullscreenButton();
                    }
                    break;
                case ButtonType.Vibration:
                    {
                        Main.OptionsManager.SwitchVibration();
                        UpdateVibrationButton();
                    }
                    break;
                case ButtonType.Difficulty:
                    {
                        Main.OptionsManager.CycleDifficulty();
                        UpdateDifficultyButton();
                    }
                    break;
                case ButtonType.PauseOnFocusLost:
                    {
                        Main.OptionsManager.SwitchPauseOnFocusLost();
                        UpdatePauseOnFocusLostButton();
                    }
                    break;
            }
        }
        protected virtual void OnSliderValueChangedCallback(SliderType type, float value)
        {
            switch (type)
            {
                case SliderType.Music:
                    {
                        Main.OptionsManager.SetMusicVolume(value);
                        UpdateMusicSlider();
                    }
                    break;
                case SliderType.Sound:
                    {
                        Main.OptionsManager.SetSoundVolume(value);
                        UpdateSoundSlider();
                    }
                    break;
            }
        }
        protected virtual void OnDropdownValueChangedCallback(DropdownType type, int index)
        {
        }
        #region 更新元素
        protected string GetFloatText(float value)
        {
            return Main.LanguageManager._(OPTION_VALUE_PERCENT, Mathf.RoundToInt(value * 100));
        }
        protected string GetValueText(bool value)
        {
            return Main.LanguageManager._(value ? StringTable.YES : StringTable.NO);
        }
        protected string GetDifficultyText(NamespaceID id)
        {
            return Main.ResourceManager.GetDifficultyName(id);
        }
        protected void UpdateSliderValue(float value, string optionKey, SliderType sliderType)
        {
            var valueText = GetFloatText(value);
            var text = Main.LanguageManager._(optionKey, valueText);
            dialog.SetSliderValue(sliderType, value);
            dialog.SetSliderText(sliderType, text);
        }
        protected void UpdateButtonText(bool value, string optionKey, TextButtonType buttonType)
        {
            var valueText = GetValueText(value);
            var text = Main.LanguageManager._(optionKey, valueText);
            dialog.SetButtonText(buttonType, text);
        }
        protected void UpdateMusicSlider()
        {
            var value = Main.OptionsManager.GetMusicVolume();
            UpdateSliderValue(value, OPTION_MUSIC, SliderType.Music);
        }
        protected void UpdateSoundSlider()
        {
            var value = Main.OptionsManager.GetSoundVolume();
            UpdateSliderValue(value, OPTION_SOUND, SliderType.Sound);
        }
        protected void UpdateSwapTriggerButton()
        {
            var value = Main.OptionsManager.IsTriggerSwapped();
            UpdateButtonText(value, OPTION_SWAP_TRIGGER, TextButtonType.SwapTrigger);
        }
        protected void UpdateFullscreenButton()
        {
            var value = Main.OptionsManager.IsFullscreen();
            UpdateButtonText(value, OPTION_FULLSCREEN, TextButtonType.Fullscreen);
        }
        protected void UpdateVibrationButton()
        {
            var value = Main.OptionsManager.IsVibration();
            UpdateButtonText(value, OPTION_VIBRATION, TextButtonType.Vibration);
        }
        protected void UpdateDifficultyButton()
        {
            var value = Main.OptionsManager.GetDifficulty();
            var valueText = GetDifficultyText(value);
            var text = Main.LanguageManager._(OPTION_DIFFICULTY, valueText);
            dialog.SetButtonText(TextButtonType.Difficulty, text);
        }
        private void UpdatePauseOnFocusLostButton()
        {
            var value = Main.OptionsManager.GetPauseOnFocusLost();
            UpdateButtonText(value, OPTION_PAUSE_ON_FOCUS_LOST, TextButtonType.PauseOnFocusLost);
        }
        protected virtual void UpdateAllElements()
        {
            UpdateMusicSlider();
            UpdateSoundSlider();
            UpdateSwapTriggerButton();
            UpdateFullscreenButton();
            UpdateVibrationButton();
            UpdateDifficultyButton();
            UpdatePauseOnFocusLostButton();
            dialog.SetButtonActive(ButtonType.SwapTrigger, Main.SaveManager.IsTriggerUnlocked());
            dialog.SetButtonActive(ButtonType.Fullscreen, !Main.IsMobile());
            dialog.SetButtonActive(ButtonType.Vibration, Main.IsMobile());
        }
        #endregion

        public event Action OnClose;
        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_SWAP_TRIGGER = "交换触发：{0}";
        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_FULLSCREEN = "全屏：{0}";
        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_VIBRATION = "设备震动：{0}";
        [TranslateMsg("选项，{0}为难度")]
        public const string OPTION_DIFFICULTY = "难度：{0}";
        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_PAUSE_ON_FOCUS_LOST = "后台暂停：{0}";


        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_MUSIC = "音乐音量：{0}";
        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_SOUND = "音效音量：{0}";
        [TranslateMsg("值，{0}为百分数")]
        public const string OPTION_VALUE_PERCENT = "{0}%";
        protected MainManager Main => MainManager.Instance;
        protected OptionsDialog dialog;
    }
}
