﻿using System;
using System.Linq;
using MukioI18n;
using MVZ2.Cameras;
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
            ResolutionManager.OnResolutionChanged += OnResolutionChangedCallback;

            this.dialog = dialog;

            dialog.OnButtonClick += OnButtonClickCallback;
            dialog.OnSliderValueChanged += OnSliderValueChangedCallback;
            dialog.OnDropdownValueChanged += OnDropdownValueChangedCallback;

            Language = Main.OptionsManager.GetLanguage();
            BloodAndGore = Main.OptionsManager.HasBloodAndGore();
        }
        public virtual void InitDialog()
        {
            // Main
            UpdateMusicSlider();
            UpdateSoundSlider();
            UpdateFastforwardSlider();
            UpdateSwapTriggerButton();
            UpdatePauseOnFocusLostButton();
            UpdateDifficultyButton();

            // More
            // General
            InitLanguageDropdown();
            UpdateLanguageDropdown();
            UpdateVibrationButton();
            UpdateSkipAllTalksButton();
            UpdateChooseWarningsButton();
            UpdateCommandBlockModeButton();

            // Display
            UpdateFullscreenButton(Main.OptionsManager.IsFullscreen());
            UpdateAnimationFrequencySlider();
            InitResolutionDropdown();
            UpdateResolutionDropdown();
            UpdateParticlesSlider();
            UpdateShakeSlider();
            UpdateBloodAndGoreButton();
            UpdateShowFPSButton();

            // Controls
            UpdateShowHotkeysButton();

            // Misc
            UpdateShowSponsorNamesButton();

            dialog.SetButtonActive(ButtonType.SwapTrigger, Main.SaveManager.IsTriggerUnlocked());
            dialog.SetButtonActive(ButtonType.CommandBlockMode, Main.SaveManager.IsCommandBlockUnlocked());

            dialog.SetButtonActive(ButtonType.Vibration, Main.IsMobile());

            dialog.SetButtonActive(ButtonType.Fullscreen, !Main.IsMobile());
            dialog.SetButtonActive(ButtonType.Keybinding, !Main.IsMobile());
            dialog.SetButtonActive(ButtonType.ShowHotkeys, !Main.IsMobile());
            dialog.SetDropdownActive(DropdownType.Resolution, !Main.IsMobile());

            dialog.SetPage(Page.Main);
        }
        public virtual void Dispose()
        {
            if (NeedsReload)
            {
                Main.OptionsManager.SetLanguage(Language);
                Main.OptionsManager.SetBloodAndGore(BloodAndGore);
            }
            dialog.OnButtonClick -= OnButtonClickCallback;
            dialog.OnSliderValueChanged -= OnSliderValueChangedCallback;
            dialog.OnDropdownValueChanged -= OnDropdownValueChangedCallback;
            ResolutionManager.OnResolutionChanged -= OnResolutionChangedCallback;
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
                case ButtonType.MoreOptions:
                    dialog.SetPage(Page.More);
                    break;
                case ButtonType.MoreBack:
                    dialog.SetPage(Page.Main);
                    break;

                // General.
                case ButtonType.Vibration:
                    {
                        Main.OptionsManager.SwitchVibration();
                        UpdateVibrationButton();
                    }
                    break;
                case ButtonType.SkipAllTalks:
                    {
                        Main.OptionsManager.SwitchSkipAllTalks();
                        UpdateSkipAllTalksButton();
                    }
                    break;
                case ButtonType.ChooseWarnings:
                    {
                        Main.OptionsManager.SwitchBlueprintChooseWarningsDisabled();
                        UpdateChooseWarningsButton();
                    }
                    break;
                case ButtonType.CommandBlockMode:
                    {
                        Main.OptionsManager.CycleCommandBlockMode();
                        UpdateCommandBlockModeButton();
                    }
                    break;

                // Display.
                case ButtonType.Fullscreen:
                    {
                        bool fullscreen = Main.OptionsManager.IsFullscreen();
                        Main.OptionsManager.SetFullscreen(!fullscreen);
                        UpdateFullscreenButton(!fullscreen);
                    }
                    break;
                case ButtonType.BloodAndGore:
                    {
                        BloodAndGore = !BloodAndGore;
                        NeedsReload = true;
                        UpdateBloodAndGoreButton();
                    }
                    break;
                case ButtonType.ShowFPS:
                    {
                        Main.OptionsManager.CycleFPSMode();
                        UpdateShowFPSButton();
                    }
                    break;

                // Controls.
                case ButtonType.ShowHotkeys:
                    {
                        Main.OptionsManager.SwitchShowHotkeyIndicators();
                        UpdateShowHotkeysButton();
                    }
                    break;
                case ButtonType.Keybinding:
                    {
                        Main.Scene.ShowKeybinding();
                    }
                    break;

                // Misc.
                case ButtonType.Credits:
                    {
                        Main.Scene.ShowCredits();
                    }
                    break;
                case ButtonType.ExportLogFiles:
                    {
                        Main.DebugManager.ExportLogFiles();
                    }
                    break;
                case ButtonType.ShowSponsorNames:
                    {
                        Main.OptionsManager.SwitchShowSponsorNames();
                        UpdateShowSponsorNamesButton();
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
                case SliderType.AnimationFrequency:
                    {
                        var multi = ValueToAnimationFrequency(value);
                        Main.OptionsManager.SetAnimationFrequency(multi);
                        UpdateAnimationFrequencySlider();
                    }
                    break;
                case SliderType.Particles:
                    {
                        Main.OptionsManager.SetParticleAmount(value);
                        UpdateParticlesSlider();
                    }
                    break;
                case SliderType.Shake:
                    {
                        Main.OptionsManager.SetShakeAmount(value);
                        UpdateShakeSlider();
                    }
                    break;
            }
        }
        protected virtual void OnDropdownValueChangedCallback(DropdownType type, int index)
        {
            switch (type)
            {
                case DropdownType.Language:
                    {
                        Language = languageValues[index];
                        NeedsReload = true;
                        UpdateLanguageDropdown();
                    }
                    break;
                case DropdownType.Resolution:
                    {
                        var resolution = resolutionValues[index];
                        Main.ResolutionManager.SetResolution(resolution.x, resolution.y);
                        UpdateResolutionDropdown();
                    }
                    break;
            }
        }
        private void OnResolutionChangedCallback(int width, int height)
        {
            InitResolutionDropdown();
            UpdateResolutionDropdown();
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
        protected string GetValueTextFPSMode(int value)
        {
            var key = FPS_MODE_DISABLED;
            switch (value)
            {
                case FPSModes.TOP_LEFT:
                    key = FPS_MODE_TOP_LEFT;
                    break;
                case FPSModes.TOP_RIGHT:
                    key = FPS_MODE_TOP_RIGHT;
                    break;
                case FPSModes.BOTTOM_LEFT:
                    key = FPS_MODE_BOTTOM_LEFT;
                    break;
                case FPSModes.BOTTOM_RIGHT:
                    key = FPS_MODE_BOTTOM_RIGHT;
                    break;
            }
            return Main.LanguageManager._p(VanillaStrings.CONTEXT_FPS_MODE, key);
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

        #region 主界面
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
        protected void UpdateAnimationFrequencySlider()
        {
            var multi = Main.OptionsManager.GetAnimationFrequency();
            var value = AnimationFrequencyToValue(multi);

            var valueText = Main.GetFloatPercentageText(multi);
            var text = Main.LanguageManager._(OPTION_ANIMATION_FREQUENCY, valueText);
            dialog.SetSliderRange(SliderType.AnimationFrequency, ANIMATION_FREQUENCY_SLIDER_START, ANIMATION_FREQUENCY_SLIDER_END, false);
            dialog.SetSliderValue(SliderType.AnimationFrequency, value);
            dialog.SetSliderText(SliderType.AnimationFrequency, text);
        }
        protected float ValueToFastForwardMultiplier(float value)
        {
            return FASTFORWARD_MULTIPLIER_START + FASTFORWARD_STEP * value;
        }
        protected float FastForwardMultiplierToValue(float multi)
        {
            return Mathf.RoundToInt((multi - FASTFORWARD_MULTIPLIER_START) / FASTFORWARD_STEP);
        }
        private float ValueToAnimationFrequency(float value)
        {
            return value;
        }
        protected float AnimationFrequencyToValue(float frequency)
        {
            return frequency;
        }
        protected void UpdateSwapTriggerButton()
        {
            var value = Main.OptionsManager.IsTriggerSwapped();
            UpdateButtonText(value, OPTION_SWAP_TRIGGER, TextButtonType.SwapTrigger);
        }
        private void UpdatePauseOnFocusLostButton()
        {
            var value = Main.OptionsManager.GetPauseOnFocusLost();
            UpdateButtonText(value, OPTION_PAUSE_ON_FOCUS_LOST, TextButtonType.PauseOnFocusLost);
        }
        protected void UpdateDifficultyButton()
        {
            var value = Main.OptionsManager.GetDifficulty();
            var valueText = GetDifficultyText(value);
            var text = Main.LanguageManager._(OPTION_DIFFICULTY, valueText);
            dialog.SetButtonText(TextButtonType.Difficulty, text);
        }
        #endregion

        #region 常规设置
        private void InitLanguageDropdown()
        {
            var values = Main.LanguageManager.GetAllLanguages();
            languageValues = values;
            dialog.SetDropdownValues(DropdownType.Language, values.Select(v => Main.LanguageManager.GetLanguageName(v)).ToArray());
        }
        private void UpdateLanguageDropdown()
        {
            var value = Language;
            var index = Array.IndexOf(languageValues, value);
            dialog.SetDropdownValue(DropdownType.Language, index);
        }
        protected void UpdateVibrationButton()
        {
            var value = Main.OptionsManager.IsVibration();
            UpdateButtonText(value, OPTION_VIBRATION, TextButtonType.Vibration);
        }
        protected void UpdateSkipAllTalksButton()
        {
            var value = Main.OptionsManager.SkipAllTalks();
            UpdateButtonText(value, OPTION_SKIP_ALL_TALKS, TextButtonType.SkipAllTalks);
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
        #endregion

        #region 显示设置
        private void InitResolutionDropdown()
        {
            var resolutions = Main.ResolutionManager.GetResolutions().Select(r => new Vector2Int(r.width, r.height)).Distinct().ToArray();
            var currentResolution = Main.ResolutionManager.GetCurrentResolution();
            Vector2Int[] values;
            if (!resolutions.Any(r => r.x == currentResolution.width && r.y == currentResolution.height))
            {
                values = new Vector2Int[resolutions.Length + 1];
                for (int i = 0; i < resolutions.Length; i++)
                {
                    values[i] = resolutions[i];
                }
                values[values.Length - 1] = new Vector2Int(currentResolution.width, currentResolution.height);
            }
            else
            {
                values = resolutions;
            }
            resolutionValues = values;

            var texts = values.Select(v => Main.ResolutionManager.GetResolutionName(v.x, v.y));
            dialog.SetDropdownValues(DropdownType.Resolution, texts.ToArray());
        }
        private void UpdateResolutionDropdown()
        {
            var value = Main.ResolutionManager.GetCurrentResolution();
            var index = Array.FindIndex(resolutionValues, r => r.x == value.width && r.y == value.height);
            if (index < 0)
            {
                index = resolutionValues.Length - 1;
            }
            dialog.SetDropdownValue(DropdownType.Resolution, index);
        }
        protected void UpdateFullscreenButton(bool value)
        {
            UpdateButtonText(value, OPTION_FULLSCREEN, TextButtonType.Fullscreen);
        }
        private void UpdateParticlesSlider()
        {
            var value = Main.OptionsManager.GetParticleAmount();
            UpdateSliderValue(value, OPTION_PARTICLE_AMOUNT, SliderType.Particles);
        }
        private void UpdateShakeSlider()
        {
            var value = Main.OptionsManager.GetShakeAmount();
            UpdateSliderValue(value, OPTION_SHAKE_AMOUNT, SliderType.Shake);
        }
        protected void UpdateShowFPSButton()
        {
            var value = Main.OptionsManager.GetFPSMode();
            var valueText = GetValueTextFPSMode(value);
            var text = Main.LanguageManager._(OPTION_FPS_MODE, valueText);
            dialog.SetButtonText(TextButtonType.ShowFPS, text);
        }
        private void UpdateBloodAndGoreButton()
        {
            var value = BloodAndGore;
            UpdateButtonText(value, OPTION_BLOOD_AND_GORE, TextButtonType.BloodAndGore);
        }
        #endregion

        #region 控制
        protected void UpdateShowHotkeysButton()
        {
            var value = Main.OptionsManager.ShowHotkeyIndicators();
            UpdateButtonText(value, OPTION_SHOW_HOTKEYS, TextButtonType.ShowHotkeys);
        }
        #endregion

        #region 杂项
        protected void UpdateShowSponsorNamesButton()
        {
            var value = Main.OptionsManager.ShowSponsorNames();
            UpdateButtonText(value, OPTION_SHOW_SPONSOR_NAMES, TextButtonType.ShowSponsorNames);
        }
        #endregion

        #endregion

        public event Action OnClose;

        public const int FASTFORWARD_STEP_COUNT = 20;
        public const float FASTFORWARD_SLIDER_START = 1;
        public const float FASTFORWARD_SLIDER_END = FASTFORWARD_STEP_COUNT;
        public const float FASTFORWARD_MULTIPLIER_START = 1;
        public const float FASTFORWARD_MULTIPLIER_END = 3;
        public const float FASTFORWARD_MULTIPLIER_RANGE = FASTFORWARD_MULTIPLIER_END - FASTFORWARD_MULTIPLIER_START;
        public const float FASTFORWARD_STEP = FASTFORWARD_MULTIPLIER_RANGE / FASTFORWARD_STEP_COUNT;

        public const float ANIMATION_FREQUENCY_SLIDER_START = 0.3f;
        public const float ANIMATION_FREQUENCY_SLIDER_END = 1;
        public const float ANIMATION_FREQUENCY_START = 0.3f;
        public const float ANIMATION_FREQUENCY_END = 1;


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
        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_ANIMATION_FREQUENCY = "动画频率：{0}";

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
        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_FPS_MODE = "显示帧率：{0}";
        [TranslateMsg("选项，{0}为是否开启")]
        public const string OPTION_SHOW_HOTKEYS = "显示快捷键：{0}";

        [TranslateMsg("命令方块模式", VanillaStrings.CONTEXT_COMMAND_BLOCK_MODE)]
        public const string COMMAND_BLOCK_MODE_MANUAL = "手选";
        [TranslateMsg("命令方块模式", VanillaStrings.CONTEXT_COMMAND_BLOCK_MODE)]
        public const string COMMAND_BLOCK_MODE_PREVIOUS = "前位";

        [TranslateMsg("FPS模式", VanillaStrings.CONTEXT_FPS_MODE)]
        public const string FPS_MODE_DISABLED = "关闭";
        [TranslateMsg("FPS模式", VanillaStrings.CONTEXT_FPS_MODE)]
        public const string FPS_MODE_TOP_LEFT = "左上";
        [TranslateMsg("FPS模式", VanillaStrings.CONTEXT_FPS_MODE)]
        public const string FPS_MODE_TOP_RIGHT = "右上";
        [TranslateMsg("FPS模式", VanillaStrings.CONTEXT_FPS_MODE)]
        public const string FPS_MODE_BOTTOM_LEFT = "左下";
        [TranslateMsg("FPS模式", VanillaStrings.CONTEXT_FPS_MODE)]
        public const string FPS_MODE_BOTTOM_RIGHT = "右下";

        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_PARTICLE_AMOUNT = "粒子数量：{0}";
        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_SHAKE_AMOUNT = "屏幕震动：{0}";
        protected MainManager Main => MainManager.Instance;
        public bool NeedsReload { get; private set; }
        public bool BloodAndGore { get; private set; }
        public string Language { get; private set; }
        protected OptionsDialog dialog;
        private string[] languageValues;
        private Vector2Int[] resolutionValues;
    }
}
