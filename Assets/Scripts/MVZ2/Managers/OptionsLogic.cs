using System;
using System.Linq;
using MukioI18n;
using MVZ2.UI;
using PVZEngine;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace MVZ2
{
    using static MVZ2.UI.OptionsDialog;
    public class OptionsLogic : IDisposable
    {
        public OptionsLogic(OptionsDialog dialog)
        {
            this.dialog = dialog;
            dialog.OnButtonClick += OnButtonClickCallback;
            dialog.OnSliderValueChanged += OnSliderValueChangedCallback;
            dialog.OnDropdownValueChanged += OnDropdownValueChangedCallback;
            Main.ResolutionManager.OnResolutionChanged += OnResolutionChangedCallback;

            Language = Main.OptionsManager.GetLanguage();
            BloodAndGore = Main.OptionsManager.HasBloodAndGore();

            InitLanguageDropdown();
            InitResolutionDropdown();
            UpdateAllElements();
            dialog.SetPage(Page.Main);
        }
        public void Dispose()
        {
            Main.ResolutionManager.OnResolutionChanged -= OnResolutionChangedCallback;
        }
        private void OnButtonClickCallback(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Back:
                    OnClose?.Invoke();
                    break;
                case ButtonType.MoreOptions:
                    dialog.SetPage(Page.More);
                    break;
                case ButtonType.MoreBack:
                    dialog.SetPage(Page.Main);
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
                case ButtonType.BloodAndGore:
                    {
                        BloodAndGore = !BloodAndGore;
                        NeedsReload = true;
                        UpdateBloodAndGoreButton();
                    }
                    break;
            }
        }
        private void OnSliderValueChangedCallback(SliderType type, float value)
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
        private void OnDropdownValueChangedCallback(DropdownType type, int index)
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
                        Main.ResolutionManager.SetResolution(resolution);
                        UpdateResolutionDropdown();
                    }
                    break;
            }
        }
        #region 更新元素
        private string GetFloatText(float value)
        {
            return Main.LanguageManager._(OPTION_VALUE_PERCENT, Mathf.RoundToInt(value * 100));
        }
        private string GetValueText(bool value)
        {
            return Main.LanguageManager._(value ? OPTION_VALUE_YES : OPTION_VALUE_NO);
        }
        private string GetDifficultyText(NamespaceID id)
        {
            return Main.ResourceManager.GetDifficultyName(id);
        }
        private void UpdateSliderValue(float value, string optionKey, SliderType sliderType)
        {
            var valueText = GetFloatText(value);
            var text = Main.LanguageManager._(optionKey, valueText);
            dialog.SetSliderValue(sliderType, value);
            dialog.SetSliderText(sliderType, text);
        }
        private void UpdateButtonText(bool value, string optionKey, TextButtonType buttonType)
        {
            var valueText = GetValueText(value);
            var text = Main.LanguageManager._(optionKey, valueText);
            dialog.SetButtonText(buttonType, text);
        }
        private void UpdateMusicSlider()
        {
            var value = Main.OptionsManager.GetMusicVolume();
            UpdateSliderValue(value, OPTION_MUSIC, SliderType.Music);
        }
        private void UpdateSoundSlider()
        {
            var value = Main.OptionsManager.GetSoundVolume();
            UpdateSliderValue(value, OPTION_SOUND, SliderType.Sound);
        }
        private void UpdateSwapTriggerButton()
        {
            var value = Main.OptionsManager.IsTriggerSwapped();
            UpdateButtonText(value, OPTION_SWAP_TRIGGER, TextButtonType.SwapTrigger);
            dialog.SetButtonActive(ButtonType.SwapTrigger, Main.SaveManager.IsTriggerUnlocked());
        }
        private void UpdateFullscreenButton()
        {
            var value = Main.OptionsManager.IsFullscreen();
            UpdateButtonText(value, OPTION_FULLSCREEN, TextButtonType.Fullscreen);
            dialog.SetButtonActive(ButtonType.Fullscreen, !Main.IsMobile());
        }
        private void UpdateVibrationButton()
        {
            var value = Main.OptionsManager.IsVibration();
            UpdateButtonText(value, OPTION_VIBRATION, TextButtonType.Vibration);
            dialog.SetButtonActive(ButtonType.Vibration, Main.IsMobile());
        }
        private void UpdateDifficultyButton()
        {
            var value = Main.OptionsManager.GetDifficulty();
            var valueText = GetDifficultyText(value);
            var text = Main.LanguageManager._(OPTION_DIFFICULTY, valueText);
            dialog.SetButtonText(TextButtonType.Difficulty, text);
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
        private void InitLanguageDropdown()
        {
            var values = Main.LanguageManager.GetAllLanguages();
            languageValues = values;
            dialog.SetDropdownValues(DropdownType.Language, values.Select(v => Main.LanguageManager.GetLanguageName(v)).ToArray());
        }
        private void InitResolutionDropdown()
        {
            var resolutions = Main.ResolutionManager.GetResolutions();
            var currentResolution = Main.ResolutionManager.GetCurrentResolution();
            Resolution[] values;
            if (!resolutions.Any(r => r.width == currentResolution.width && r.height == currentResolution.height))
            {
                values = new Resolution[resolutions.Length + 1];
                for (int i = 0; i < resolutions.Length; i++)
                {
                    values[i] = resolutions[i];
                }
                values[values.Length - 1] = currentResolution;
            }
            else
            {
                values = resolutions;
            }
            resolutionValues = values;

            var texts = values.Select(v => Main.ResolutionManager.GetResolutionName(v));
            dialog.SetDropdownValues(DropdownType.Resolution, texts.ToArray());
        }
        private void UpdateLanguageDropdown()
        {
            var value = Language;
            var index = Array.IndexOf(languageValues, value);
            dialog.SetDropdownValue(DropdownType.Language, index);
        }
        private void UpdateResolutionDropdown()
        {
            var value = Main.ResolutionManager.GetCurrentResolution();
            var index = Array.FindIndex(resolutionValues, r => r.width == value.width && r.height == value.height);
            if (index < 0)
            {
                index = resolutionValues.Length - 1;
            }
            dialog.SetDropdownValue(DropdownType.Resolution, index);
        }
        private void UpdateBloodAndGoreButton()
        {
            var value = BloodAndGore;
            UpdateButtonText(value, OPTION_BLOOD_AND_GORE, TextButtonType.BloodAndGore);
        }
        private void UpdateAllElements()
        {
            UpdateMusicSlider();
            UpdateSoundSlider();
            UpdateSwapTriggerButton();
            UpdateFullscreenButton();
            UpdateVibrationButton();
            UpdateDifficultyButton();

            UpdateParticlesSlider();
            UpdateShakeSlider();
            UpdateLanguageDropdown();
            UpdateResolutionDropdown();
            UpdateBloodAndGoreButton();
        }
        #endregion
        private void OnResolutionChangedCallback(int width, int height)
        {
            InitResolutionDropdown();
            UpdateResolutionDropdown();
        }
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
        public const string OPTION_BLOOD_AND_GORE = "血与碎块：{0}";


        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_MUSIC = "音乐音量：{0}";
        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_SOUND = "音效音量：{0}";
        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_PARTICLE_AMOUNT = "粒子数量：{0}";
        [TranslateMsg("选项，{0}为量")]
        public const string OPTION_SHAKE_AMOUNT = "屏幕震动：{0}";
        [TranslateMsg("值，{0}为百分数")]
        public const string OPTION_VALUE_PERCENT = "{0}%";
        [TranslateMsg("值")]
        public const string OPTION_VALUE_YES = "是";
        [TranslateMsg("值")]
        public const string OPTION_VALUE_NO = "否";
        public bool NeedsReload { get; private set; }
        public bool BloodAndGore { get; private set; }
        public string Language { get; private set; }
        private MainManager Main => MainManager.Instance;
        private string[] languageValues;
        private Resolution[] resolutionValues;
        private OptionsDialog dialog;
    }
}
