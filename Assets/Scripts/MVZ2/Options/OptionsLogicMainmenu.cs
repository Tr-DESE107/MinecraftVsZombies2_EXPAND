﻿using System;
using System.Linq;
using MVZ2.Cameras;
using MVZ2.Mainmenu;
using MVZ2.UI;
using MVZ2Logic;
using UnityEngine;

namespace MVZ2.Options
{
    using static MVZ2.UI.OptionsDialog;
    public class OptionsLogicMainmenu : OptionsLogic
    {
        public OptionsLogicMainmenu(OptionsDialog dialog, MainmenuController controller) : base(dialog)
        {
            ResolutionManager.OnResolutionChanged += OnResolutionChangedCallback;

            Language = Main.OptionsManager.GetLanguage();
            BloodAndGore = Main.OptionsManager.HasBloodAndGore();
            mainmenu = controller;
        }
        public override void InitDialog()
        {
            UpdateParticlesSlider();
            UpdateShakeSlider();

            InitLanguageDropdown();
            UpdateLanguageDropdown();
            InitResolutionDropdown();
            UpdateResolutionDropdown();

            UpdateBloodAndGoreButton();
            UpdateSkipAllTalksButton();
            UpdateShowSponsorNamesButton();
            UpdateChooseWarningsButton();
            UpdateCommandBlockModeButton();

            base.InitDialog();

            dialog.SetDropdownActive(DropdownType.Resolution, !Global.IsMobile());
            dialog.SetButtonActive(ButtonType.LeaveLevel, false);
            dialog.SetButtonActive(ButtonType.Restart, false);
            dialog.SetButtonActive(ButtonType.Keybinding, !Global.IsMobile());

            dialog.SetPage(Page.Main);
        }
        public override void Dispose()
        {
            base.Dispose();
            ResolutionManager.OnResolutionChanged -= OnResolutionChangedCallback;
        }
        protected override void OnButtonClickCallback(ButtonType type)
        {
            base.OnButtonClickCallback(type);
            switch (type)
            {
                case ButtonType.MoreOptions:
                    dialog.SetPage(Page.More);
                    break;
                case ButtonType.MoreBack:
                    dialog.SetPage(Page.Main);
                    break;
                case ButtonType.Credits:
                    {
                        mainmenu.ShowCredits();
                    }
                    break;
                case ButtonType.BloodAndGore:
                    {
                        BloodAndGore = !BloodAndGore;
                        NeedsReload = true;
                        UpdateBloodAndGoreButton();
                    }
                    break;
                case ButtonType.SkipAllTalks:
                    {
                        Main.OptionsManager.SwitchSkipAllTalks();
                        UpdateSkipAllTalksButton();
                    }
                    break;
                case ButtonType.ShowSponsorNames:
                    {
                        Main.OptionsManager.SwitchShowSponsorNames();
                        UpdateShowSponsorNamesButton();
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
                case ButtonType.Keybinding:
                    {
                        mainmenu.ShowKeybinding();
                    }
                    break;
            }
        }
        protected override void OnSliderValueChangedCallback(SliderType type, float value)
        {
            base.OnSliderValueChangedCallback(type, value);
            switch (type)
            {
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
        protected override void OnDropdownValueChangedCallback(DropdownType type, int index)
        {
            base.OnDropdownValueChangedCallback(type, index);
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
        private void UpdateLanguageDropdown()
        {
            var value = Language;
            var index = Array.IndexOf(languageValues, value);
            dialog.SetDropdownValue(DropdownType.Language, index);
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
        private void UpdateBloodAndGoreButton()
        {
            var value = BloodAndGore;
            UpdateButtonText(value, OPTION_BLOOD_AND_GORE, TextButtonType.BloodAndGore);
        }
        #endregion
        public bool NeedsReload { get; private set; }
        public bool BloodAndGore { get; private set; }
        public string Language { get; private set; }
        private MainmenuController mainmenu;
        private string[] languageValues;
        private Vector2Int[] resolutionValues;
    }
}
