using System;
using System.Globalization;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class OptionsManager : MonoBehaviour
    {
        public void InitOptions()
        {
            options = new Options();
        }
        public void LoadOptions()
        {
            options.difficulty = NamespaceID.Parse(GetPlayerPrefsString(PREFS_DIFFICULTY, GetDefaultDifficulty()), Main.BuiltinNamespace);
            options.swapTrigger = GetPlayerPrefsBool(PREFS_SWAP_TRIGGER, false);
            options.vibration = GetPlayerPrefsBool(PREFS_VIBRATION, false);
            options.bloodAndGore = GetPlayerPrefsBool(PREFS_BLOOD_AND_GORE, true);
            options.pauseOnFocusLost = GetPlayerPrefsBool(PREFS_PAUSE_ON_FOCUS_LOST, true);

            options.musicVolume = GetPlayerPrefsFloat(PREFS_MUSIC_VOLUME, 1);
            options.soundVolume = GetPlayerPrefsFloat(PREFS_SOUND_VOLUME, 1);
            options.particleAmount = GetPlayerPrefsFloat(PREFS_PARTICLE_AMOUNT, 1);
            options.shakeAmount = GetPlayerPrefsFloat(PREFS_SHAKE_AMOUNT, 1);

            options.language = GetPlayerPrefsString(PREFS_LANGUAGE, GetEnvironmentLanguage());

            UpdateMusicVolume();
            UpdateSoundVolume();
        }

        #region 语言
        public string GetLanguage()
        {
            return options.language;
        }
        public void SetLanguage(string language)
        {
            options.language = language;
            PlayerPrefs.SetString(PREFS_LANGUAGE, language);
        }
        #endregion

        #region 难度
        public NamespaceID GetDifficulty()
        {
            return options.difficulty;
        }
        public void SetDifficulty(NamespaceID difficulty)
        {
            options.difficulty = difficulty;
            PlayerPrefs.SetString(PREFS_DIFFICULTY, difficulty.ToString());
        }
        public void CycleDifficulty()
        {
            var difficulties = Main.ResourceManager.GetAllDifficulties();
            var index = Array.IndexOf(difficulties, GetDifficulty());
            index++;
            index %= difficulties.Length;
            SetDifficulty(difficulties[index]);
        }
        #endregion

        #region 交换触发
        public bool IsTriggerSwapped()
        {
            return options.swapTrigger;
        }
        public void SetSwapTrigger(bool value)
        {
            options.swapTrigger = value;
            PlayerPrefs.SetInt(PREFS_SWAP_TRIGGER, BoolToInt(value));
        }
        public void SwitchSwapTrigger()
        {
            SetSwapTrigger(!IsTriggerSwapped());
        }
        #endregion

        #region 全屏
        public bool IsFullscreen()
        {
            return Screen.fullScreen;
        }
        public void SetFullscreen(bool value)
        {
            Screen.fullScreen = value;
        }
        public void SwitchFullscreen()
        {
            SetFullscreen(!IsFullscreen());
        }
        #endregion

        #region 震动
        public bool IsVibration()
        {
            return options.vibration;
        }
        public void SetVibration(bool value)
        {
            options.vibration = value;
            PlayerPrefs.SetInt(PREFS_VIBRATION, BoolToInt(value));
        }
        public void SwitchVibration()
        {
            SetVibration(!IsVibration());
        }
        #endregion

        #region 血与碎块
        public bool HasBloodAndGore()
        {
            return options.bloodAndGore;
        }
        public void SetBloodAndGore(bool value)
        {
            options.bloodAndGore = value;
            PlayerPrefs.SetInt(PREFS_BLOOD_AND_GORE, BoolToInt(value));
        }
        public void SwitchBloodAndGore()
        {
            SetBloodAndGore(!HasBloodAndGore());
        }
        #endregion

        #region 焦点丢失暂停
        public bool GetPauseOnFocusLost()
        {
            return options.pauseOnFocusLost;
        }
        public void SetPauseOnFocusLost(bool value)
        {
            options.pauseOnFocusLost = value;
            PlayerPrefs.SetInt(PREFS_PAUSE_ON_FOCUS_LOST, BoolToInt(value));
        }
        public void SwitchPauseOnFocusLost()
        {
            SetPauseOnFocusLost(!GetPauseOnFocusLost());
        }
        #endregion


        #region 音乐音量
        public float GetMusicVolume()
        {
            return options.musicVolume;
        }
        public void SetMusicVolume(float value)
        {
            options.musicVolume = value;
            PlayerPrefs.SetFloat(PREFS_MUSIC_VOLUME, value);
            UpdateMusicVolume();
        }
        public void UpdateMusicVolume()
        {
            Main.MusicManager.SetGlobalVolume(GetMusicVolume());
        }
        #endregion

        #region 音效音量
        public float GetSoundVolume()
        {
            return options.soundVolume;
        }
        public void SetSoundVolume(float value)
        {
            options.soundVolume = value;
            PlayerPrefs.SetFloat(PREFS_SOUND_VOLUME, value);
            UpdateSoundVolume();
        }
        public void UpdateSoundVolume()
        {
            Main.SoundManager.SetGlobalVolume(GetSoundVolume());
        }
        #endregion

        #region 粒子数量
        public float GetParticleAmount()
        {
            return options.particleAmount;
        }
        public void SetParticleAmount(float value)
        {
            options.particleAmount = value;
            PlayerPrefs.SetFloat(PREFS_PARTICLE_AMOUNT, value);
        }
        #endregion

        #region 震动幅度
        public float GetShakeAmount()
        {
            return options.shakeAmount;
        }
        public void SetShakeAmount(float value)
        {
            options.shakeAmount = value;
            PlayerPrefs.SetFloat(PREFS_SHAKE_AMOUNT, value);
        }
        #endregion
        private static bool GetPlayerPrefsBool(string key, bool defaultValue)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, BoolToInt(defaultValue));
            }
            return IntToBool(PlayerPrefs.GetInt(key));
        }
        private static int GetPlayerPrefsInt(string key, int defaultValue)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, defaultValue);
            }
            return PlayerPrefs.GetInt(key);
        }
        private static string GetPlayerPrefsString(string key, string defaultValue)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetString(key, defaultValue);
            }
            return PlayerPrefs.GetString(key);
        }
        private static float GetPlayerPrefsFloat(string key, float defaultValue)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetFloat(key, defaultValue);
            }
            return PlayerPrefs.GetFloat(key);
        }
        private static bool IntToBool(int value)
        {
            return value > 0;
        }
        private static int BoolToInt(bool value)
        {
            return value ? 1 : 0;
        }
        private string GetDefaultDifficulty()
        {
            return new NamespaceID(Main.BuiltinNamespace, "normal").ToString();
        }
        private string GetEnvironmentLanguage()
        {
            var culture = CultureInfo.CurrentCulture;
            var allLanguages = Main.LanguageManager.GetAllLanguages();
            foreach (var language in allLanguages)
            {
                if (culture.Name == language)
                    return language;
                var langCulture = new CultureInfo(language);
                if (culture.Parent == langCulture.Parent)
                    return language;
            }
            return LanguageManager.CN;
        }
        public MainManager Main => MainManager.Instance;
        public const string PREFS_LANGUAGE = "Language";
        public const string PREFS_DIFFICULTY = "Difficulty";
        public const string PREFS_SWAP_TRIGGER = "SwapTrigger";
        public const string PREFS_VIBRATION = "Vibration";
        public const string PREFS_BLOOD_AND_GORE = "BloodAndGore";
        public const string PREFS_PAUSE_ON_FOCUS_LOST = "PauseOnFocusLost";

        public const string PREFS_MUSIC_VOLUME = "MusicVolume";
        public const string PREFS_SOUND_VOLUME = "SoundVolume";
        public const string PREFS_PARTICLE_AMOUNT = "ParticleAmount";
        public const string PREFS_SHAKE_AMOUNT = "ShakeAmount";

        private Options options;
    }
    public class Options
    {
        public bool swapTrigger;
        public bool vibration;
        public NamespaceID difficulty;
        public bool bloodAndGore;
        public bool pauseOnFocusLost;

        public float musicVolume;
        public float soundVolume;
        public float particleAmount;
        public float shakeAmount;

        public string language;
    }
}
