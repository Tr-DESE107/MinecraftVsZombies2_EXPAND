using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class OptionsDialog : Dialog
    {
        public void SetPage(Page page)
        {
            mainPage.SetActive(page == Page.Main);
            morePage.SetActive(page == Page.More);
        }
        public void SetSliderValue(SliderType type, float value)
        {
            if (sliderDict.TryGetValue(type, out var slider))
            {
                slider.Slider.SetValueWithoutNotify(value);
            }
        }
        public void SetSliderRange(SliderType type, float min, float max, bool integer)
        {
            if (sliderDict.TryGetValue(type, out var slider))
            {
                slider.Slider.minValue = min;
                slider.Slider.maxValue = max;
                slider.Slider.wholeNumbers = integer;
            }
        }
        public void SetSliderText(SliderType type, string text)
        {
            if (sliderDict.TryGetValue(type, out var slider))
            {
                slider.Text.text = text;
            }
        }
        public void SetDropdownValue(DropdownType type, int index)
        {
            if (dropdownDict.TryGetValue(type, out var dropdown))
            {
                dropdown.SetValueWithoutNotify(index);
                dropdown.Hide();
            }
        }
        public void SetDropdownValues(DropdownType type, string[] texts)
        {
            if (dropdownDict.TryGetValue(type, out var dropdown))
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(texts.ToList());
            }
        }
        public void SetButtonText(TextButtonType type, string text)
        {
            if (textButtonDict.TryGetValue(type, out var button))
            {
                button.Text.text = text;
            }
        }
        public void SetButtonActive(ButtonType type, bool value)
        {
            if (buttonDict.TryGetValue(type, out var button))
            {
                button.gameObject.SetActive(value);
                UpdateMoreElementsLines();
            }
        }
        public void SetDropdownActive(DropdownType type, bool value)
        {
            if (dropdownPairDict.TryGetValue(type, out var dropdownPair))
            {
                dropdownPair.SetActive(value);
                UpdateMoreElementsLines();
            }
        }
        private void Awake()
        {
            sliderDict.Add(SliderType.Music, musicSlider);
            sliderDict.Add(SliderType.Sound, soundSlider);
            sliderDict.Add(SliderType.Particles, particlesSlider);
            sliderDict.Add(SliderType.FastForward, fastForwardSlider);
            sliderDict.Add(SliderType.Shake, shakeSlider);
            sliderDict.Add(SliderType.AnimationFrequency, animationFrequencySlider);

            dropdownDict.Add(DropdownType.Language, languageDropdown);
            dropdownDict.Add(DropdownType.Resolution, resolutionDropdown);

            dropdownPairDict.Add(DropdownType.Language, languageDropdownPair);
            dropdownPairDict.Add(DropdownType.Resolution, resolutionDropdownPair);

            textButtonDict.Add(TextButtonType.SwapTrigger, swapTriggerButton);
            textButtonDict.Add(TextButtonType.PauseOnFocusLost, pauseOnFocusLostButton);
            textButtonDict.Add(TextButtonType.Fullscreen, fullscreenButton);
            textButtonDict.Add(TextButtonType.Vibration, vibrationButton);
            textButtonDict.Add(TextButtonType.Difficulty, diffcultyButton);
            textButtonDict.Add(TextButtonType.LeaveLevel, leaveLevelButton);
            textButtonDict.Add(TextButtonType.BloodAndGore, bloodAndGoreButton);
            textButtonDict.Add(TextButtonType.SkipAllTalks, skipAllTalksButton);
            textButtonDict.Add(TextButtonType.ShowSponsorNames, showSponsorNamesButton);
            textButtonDict.Add(TextButtonType.ChooseWarnings, chooseWarningsButton);
            textButtonDict.Add(TextButtonType.HDR_LIGHTING, hdrButton);
            textButtonDict.Add(TextButtonType.CommandBlockMode, commandBlockModeButton);
            textButtonDict.Add(TextButtonType.ShowFPS, showFPSButton);
            textButtonDict.Add(TextButtonType.ShowHotkeys, showHotkeysButton);
            textButtonDict.Add(TextButtonType.Credits, creditsButton);
            textButtonDict.Add(TextButtonType.Keybinding, keybindingButton);

            buttonDict.Add(ButtonType.SwapTrigger, swapTriggerButton.Button);
            buttonDict.Add(ButtonType.PauseOnFocusLost, pauseOnFocusLostButton.Button);

            // 左上
            buttonDict.Add(ButtonType.Fullscreen, fullscreenButton.Button);
            buttonDict.Add(ButtonType.Vibration, vibrationButton.Button);

            // 右上
            buttonDict.Add(ButtonType.Difficulty, diffcultyButton.Button);
            buttonDict.Add(ButtonType.Restart, restartButton);

            // 左下
            buttonDict.Add(ButtonType.MoreOptions, moreOptionsButton);
            buttonDict.Add(ButtonType.LeaveLevel, leaveLevelButton.Button);

            // 右下
            buttonDict.Add(ButtonType.Back, backButton);

            buttonDict.Add(ButtonType.BloodAndGore, bloodAndGoreButton.Button);
            buttonDict.Add(ButtonType.SkipAllTalks, skipAllTalksButton.Button);
            buttonDict.Add(ButtonType.ShowSponsorNames, showSponsorNamesButton.Button);
            buttonDict.Add(ButtonType.ChooseWarnings, chooseWarningsButton.Button);
            buttonDict.Add(ButtonType.CommandBlockMode, commandBlockModeButton.Button);
            buttonDict.Add(ButtonType.ShowFPS, showFPSButton.Button);
            buttonDict.Add(ButtonType.HDRLighting, hdrButton.Button);
            buttonDict.Add(ButtonType.ShowHotkeys, showHotkeysButton.Button);
            buttonDict.Add(ButtonType.Credits, creditsButton.Button);
            buttonDict.Add(ButtonType.Keybinding, keybindingButton.Button);
            buttonDict.Add(ButtonType.MoreBack, moreBackButton);
            buttonDict.Add(ButtonType.ExportLogFiles, exportLogFilesButton);


            foreach (var pair in sliderDict)
            {
                var type = pair.Key;
                pair.Value.Slider.onValueChanged.AddListener(value => OnSliderValueChanged?.Invoke(type, value));
            }
            foreach (var pair in dropdownDict)
            {
                var type = pair.Key;
                pair.Value.onValueChanged.AddListener((value) => OnDropdownValueChanged?.Invoke(type, value));
            }
            foreach (var pair in buttonDict)
            {
                var type = pair.Key;
                pair.Value.onClick.AddListener(() => OnButtonClick?.Invoke(type));
            }
        }
        private void UpdateMoreElementsLines()
        {
            foreach (var line in moreElementLines)
            {
                bool hasActiveChild = false;
                for (int i = 0; i < line.childCount; i++)
                {
                    var child = line.GetChild(i);
                    if (child && child.gameObject.activeSelf)
                    {
                        hasActiveChild = true;
                        break;
                    }
                }
                line.gameObject.SetActive(hasActiveChild);
            }
        }
        public event Action<SliderType, float> OnSliderValueChanged;
        public event Action<DropdownType, int> OnDropdownValueChanged;
        public event Action<ButtonType> OnButtonClick;

        private Dictionary<SliderType, TextSlider> sliderDict = new Dictionary<SliderType, TextSlider>();
        private Dictionary<DropdownType, TMP_Dropdown> dropdownDict = new Dictionary<DropdownType, TMP_Dropdown>();
        private Dictionary<DropdownType, GameObject> dropdownPairDict = new Dictionary<DropdownType, GameObject>();
        private Dictionary<TextButtonType, TextButton> textButtonDict = new Dictionary<TextButtonType, TextButton>();
        private Dictionary<ButtonType, Button> buttonDict = new Dictionary<ButtonType, Button>();

        [Header("Pages")]
        [SerializeField]
        private GameObject mainPage;
        [SerializeField]
        private GameObject morePage;

        [Header("Main Elements")]
        [SerializeField]
        private TextSlider musicSlider;
        [SerializeField]
        private TextSlider soundSlider;
        [SerializeField]
        private TextSlider fastForwardSlider;

        [SerializeField]
        private TextButton swapTriggerButton;
        [SerializeField]
        private TextButton pauseOnFocusLostButton;

        [SerializeField]
        private TextButton diffcultyButton;
        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Button moreOptionsButton;
        [SerializeField]
        private TextButton leaveLevelButton;

        [SerializeField]
        private Button backButton;

        [Header("More Elements")]
        [SerializeField]
        private Button moreBackButton;
        [SerializeField]
        private Transform[] moreElementLines;

        [Header("General")]
        [SerializeField]
        private GameObject languageDropdownPair;
        [SerializeField]
        private TMP_Dropdown languageDropdown;
        [SerializeField]
        private TextButton vibrationButton;
        [SerializeField]
        private TextButton skipAllTalksButton;
        [SerializeField]
        private TextButton chooseWarningsButton;
        [SerializeField]
        private TextButton commandBlockModeButton;

        [Header("Display")]
        [SerializeField]
        private TextSlider particlesSlider;
        [SerializeField]
        private TextSlider shakeSlider;
        [SerializeField]
        private TextSlider animationFrequencySlider;
        [SerializeField]
        private GameObject resolutionDropdownPair;
        [SerializeField]
        private TMP_Dropdown resolutionDropdown;
        [SerializeField]
        private TextButton fullscreenButton;
        [SerializeField]
        private TextButton bloodAndGoreButton;
        [SerializeField]
        private TextButton showFPSButton;
        [SerializeField]
        private TextButton hdrButton;

        [Header("Controls")]
        [SerializeField]
        private TextButton showHotkeysButton;
        [SerializeField]
        private TextButton keybindingButton;

        [Header("Misc")]
        [SerializeField]
        private TextButton showSponsorNamesButton;
        [SerializeField]
        private TextButton creditsButton;
        [SerializeField]
        private Button exportLogFilesButton;
        public enum Page
        {
            Main,
            More
        }
        public enum SliderType
        {
            Music,
            Sound,
            FastForward,
            Particles,
            Shake,
            AnimationFrequency,
        }
        public enum DropdownType
        {
            Language,
            Resolution,
        }
        public enum TextButtonType
        {
            SwapTrigger,
            PauseOnFocusLost,
            Fullscreen,
            Vibration,
            Difficulty,
            LeaveLevel,
            BloodAndGore,
            SkipAllTalks,
            ShowSponsorNames,
            ChooseWarnings,
            CommandBlockMode,
            ShowFPS,
            HDR_LIGHTING,
            ShowHotkeys,
            Credits,
            Keybinding,
        }
        public enum ButtonType
        {
            SwapTrigger,
            PauseOnFocusLost,

            Fullscreen,
            Vibration,

            Difficulty,
            Restart,

            MoreOptions,
            LeaveLevel,

            Back,

            MoreBack,
            BloodAndGore,

            SkipAllTalks,
            ShowSponsorNames,
            ChooseWarnings,
            CommandBlockMode,
            ShowFPS,
            ShowHotkeys,
            HDRLighting,

            Credits,
            Keybinding,
            ExportLogFiles,
        }
    }

}
