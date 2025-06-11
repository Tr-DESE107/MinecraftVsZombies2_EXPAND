using System;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Options
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
            UpdateMusicSlider();
            UpdateSoundSlider();
            UpdateFastforwardSlider();
            UpdateSwapTriggerButton();
            UpdateFullscreenButton(Main.OptionsManager.IsFullscreen());
            UpdateVibrationButton();
            UpdateDifficultyButton();
            UpdatePauseOnFocusLostButton();
            dialog.SetButtonActive(ButtonType.SwapTrigger, Main.SaveManager.IsTriggerUnlocked());
            dialog.SetButtonActive(ButtonType.Fullscreen, !Main.IsMobile());
            dialog.SetButtonActive(ButtonType.Vibration, Main.IsMobile());
            dialog.SetButtonActive(ButtonType.CommandBlockMode, Main.SaveManager.IsCommandBlockUnlocked());
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
                        bool fullscreen = Main.OptionsManager.IsFullscreen();
                        Main.OptionsManager.SetFullscreen(!fullscreen);
                        UpdateFullscreenButton(!fullscreen);
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
                case ButtonType.ExportLogFiles:
                    {
                        Main.DebugManager.ExportLogFiles();
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
                case SliderType.FastForward:
                    {
                        var multi = ValueToFastForwardMultiplier(value);
                        Main.OptionsManager.SetFastForwardMultiplier(multi);
                        UpdateFastforwardSlider();
                    }
                    break;
            }
        }
        protected virtual void OnDropdownValueChangedCallback(DropdownType type, int index)
        {
        }
        #region 更新元素
        protected string GetValueText(bool value)
        {
            return Main.LanguageManager._(value ? VanillaStrings.YES : VanillaStrings.NO);
        }
        protected string GetValueTextOnOff(bool value)
        {
            return Main.LanguageManager._(value ? VanillaStrings.ON : VanillaStrings.OFF);
        }
        protected string GetValueTextCommandBlockMode(int value)
        {
            switch (value)
            {
                case CommandBlockModes.PREVIOUS:
                    return Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_BLOCK_MODE, COMMAND_BLOCK_MODE_PREVIOUS);
            }
            return Main.LanguageManager._p(VanillaStrings.CONTEXT_COMMAND_BLOCK_MODE, COMMAND_BLOCK_MODE_MANUAL);
        }
        protected string GetDifficultyText(NamespaceID id)
        {
            return Main.ResourceManager.GetDifficultyName(id);
        }
        protected void UpdateSliderValue(float value, string optionKey, SliderType sliderType)
        {
            var valueText = Main.GetFloatPercentageText(value);
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
        protected void UpdateFastforwardSlider()
        {
            var multi = Main.OptionsManager.GetFastForwardMultiplier();
            var value = FastForwardMultiplierToValue(multi);

            var valueText = Main.GetFloatPercentageText(multi);
            var text = Main.LanguageManager._(OPTION_FASTFORWARD_MULTIPLIER, valueText);
            dialog.SetSliderRange(SliderType.FastForward, FASTFORWARD_SLIDER_START, FASTFORWARD_SLIDER_END, true);
            dialog.SetSliderValue(SliderType.FastForward, value);
            dialog.SetSliderText(SliderType.FastForward, text);
        }
        protected void UpdateSwapTriggerButton()
        {
            var value = Main.OptionsManager.IsTriggerSwapped();
            UpdateButtonText(value, OPTION_SWAP_TRIGGER, TextButtonType.SwapTrigger);
        }
        protected void UpdateFullscreenButton(bool value)
        {
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
        protected void UpdateSkipAllTalksButton()
        {
            var value = Main.OptionsManager.SkipAllTalks();
            UpdateButtonText(value, OPTION_SKIP_ALL_TALKS, TextButtonType.SkipAllTalks);
        }
        protected void UpdateShowSponsorNamesButton()
        {
            var value = Main.OptionsManager.ShowSponsorNames();
            UpdateButtonText(value, OPTION_SHOW_SPONSOR_NAMES, TextButtonType.ShowSponsorNames);
        }
        protected void UpdateChooseWarningsButton()
        {
            var value = Main.OptionsManager.AreBlueprintChooseWarningsDisabled();
            var valueText = GetValueTextOnOff(!value);
            var text = Main.LanguageManager._(OPTION_CHOOSE_WARNINGS, valueText);
            dialog.SetButtonText(TextButtonType.ChooseWarnings, text);
        }
        protected void UpdateCommandBlockModeButton()
        {
            var value = Main.OptionsManager.GetCommandBlockMode();
            var valueText = GetValueTextCommandBlockMode(value);
            var text = Main.LanguageManager._(OPTION_COMMAND_BLOCK_MODE, valueText);
            dialog.SetButtonText(TextButtonType.CommandBlockMode, text);
        }
        protected float ValueToFastForwardMultiplier(float value)
        {
            return FASTFORWARD_MULTIPLIER_START + FASTFORWARD_STEP * value;
        }
        protected float FastForwardMultiplierToValue(float multi)
        {
            return Mathf.RoundToInt((multi - FASTFORWARD_MULTIPLIER_START) / FASTFORWARD_STEP);
        }
        #endregion

        public event Action OnClose;

        public const int FASTFORWARD_STEP_COUNT = 20;
        public const float FASTFORWARD_SLIDER_START = 1;
        public const float FASTFORWARD_SLIDER_END = FASTFORWARD_STEP_COUNT;
        public const float FASTFORWARD_MULTIPLIER_START = 1;
        public const float FASTFORWARD_MULTIPLIER_END = 3;
        public const float FASTFORWARD_MULTIPLIER_RANGE = FASTFORWARD_MULTIPLIER_END - FASTFORWARD_MULTIPLIER_START;
        public const float FASTFORWARD_STEP = FASTFORWARD_MULTIPLIER_RANGE / FASTFORWARD_STEP_COUNT;


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
        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_FASTFORWARD_MULTIPLIER = "加速倍率：{0}";

        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_BLOOD_AND_GORE = "血与碎块：{0}";

        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_SKIP_ALL_TALKS = "跳过对话：{0}";
        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_SHOW_SPONSOR_NAMES = "赞助者名称：{0}";
        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_CHOOSE_WARNINGS = "选卡警告：{0}";
        [TranslateMsg("选项，{0}为模式")]
        public const string OPTION_COMMAND_BLOCK_MODE = "命令方块：{0}";

        [TranslateMsg("命令方块模式", VanillaStrings.CONTEXT_COMMAND_BLOCK_MODE)]
        public const string COMMAND_BLOCK_MODE_MANUAL = "手选";
        [TranslateMsg("命令方块模式", VanillaStrings.CONTEXT_COMMAND_BLOCK_MODE)]
        public const string COMMAND_BLOCK_MODE_PREVIOUS = "前位";


        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_PARTICLE_AMOUNT = "粒子数量：{0}";
        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_SHAKE_AMOUNT = "屏幕震动：{0}";
        protected MainManager Main => MainManager.Instance;
        protected OptionsDialog dialog;
    }
}
