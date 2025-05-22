using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Vanilla;
using MVZ2Logic;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Localization
{
    public partial class LanguageManager : MonoBehaviour, IGameLocalization
    {
        public string _(string text, params object[] args)
        {
            return GetLocalizedString(text, GetCurrentLanguage(), args);
        }
        public string _p(string context, string text, params object[] args)
        {
            return GetLocalizedStringParticular(context, text, GetCurrentLanguage(), args);
        }
        public string GetLocalizedString(string text, string language, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            if (TryGetLocalizedString(text, language, out var str, args))
                return str;
            return string.Format(text, args);
        }
        public bool TryGetLocalizedString(string text, string language, out string translated, params object[] args)
        {
            foreach (var languagePack in loadedLanguagePacks)
            {
                if (languagePack == null)
                    continue;
                if (languagePack.TryGetString(language, text, out var result, args))
                {
                    translated = result;
                    return true;
                }
            }
            translated = null;
            return false;
        }
        public string GetLocalizedStringParticular(string context, string text, string language, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            if (TryGetLocalizedStringParticular(context, text, language, out var str, args))
                return str;
            return string.Format(text, args);
        }
        public bool TryGetLocalizedStringParticular(string context, string text, string language, out string translated, params object[] args)
        {
            foreach (var languagePack in loadedLanguagePacks)
            {
                if (languagePack == null)
                    continue;
                if (languagePack.TryGetStringParticular(language, context, text, out var result, args))
                {
                    translated = result;
                    return true;
                }
            }
            translated = null;
            return false;
        }
        public string GetLanguageName(string language)
        {
            try
            {
                if (TryGetLocalizedStringParticular(VanillaStrings.CONTEXT_LANGUAGE_NAME, CURRENT_LANGUAGE_NAME, language, out var name))
                {
                    return name;
                }
                var cultureInfo = CultureInfo.GetCultureInfo(language);
                return $"{cultureInfo.NativeName}({language})";
            }
            catch (CultureNotFoundException)
            {
                return language;
            }
        }
        public Sprite GetCurrentLanguageSprite(Sprite sprite)
        {
            return GetLocalizedSprite(sprite, GetCurrentLanguage());
        }
        public Sprite GetCurrentLanguageSprite(SpriteReference spriteRef)
        {
            return GetLocalizedSprite(spriteRef, GetCurrentLanguage());
        }
        public Sprite GetLocalizedSprite(Sprite sprite, string language)
        {
            var spriteID = main.ResourceManager.GetSpriteReference(sprite);
            return GetLocalizedSprite(spriteID, language) ?? sprite;
        }
        public Sprite GetLocalizedSprite(SpriteReference spriteRef, string language)
        {
            if (spriteRef == null)
                return null;
            if (spriteRef.isSheet)
            {
                var sheet = GetLocalizedSpriteSheet(spriteRef.id, language);
                if (sheet == null || spriteRef.index < 0 || spriteRef.index >= sheet.Length)
                    return null;
                return sheet[spriteRef.index];
            }
            return GetLocalizedSprite(spriteRef.id, language);
        }
        public Sprite GetLocalizedSprite(NamespaceID spriteID, string language)
        {
            foreach (var languagePack in loadedLanguagePacks)
            {
                if (languagePack == null)
                    continue;
                if (languagePack.TryGetSprite(language, spriteID, out var localizedSpr))
                    return localizedSpr;
            }
            return null;
        }
        public Sprite[] GetLocalizedSpriteSheet(NamespaceID spriteID, string language)
        {
            foreach (var languagePack in loadedLanguagePacks)
            {
                if (languagePack == null)
                    continue;
                if (languagePack.TryGetSpriteSheet(language, spriteID, out var localizedSpr))
                    return localizedSpr;
            }
            return null;
        }
        public string GetCurrentLanguage()
        {
#if UNITY_EDITOR
            switch (debugLanguage)
            {
                case DebugLanguage.Chinese:
                    return CN;
                case DebugLanguage.English:
                    return EN;
            }
#endif
            return Main.OptionsManager.GetLanguage();
        }
        public void CallLanguageChanged(string lang)
        {
            OnLanguageChanged?.Invoke(lang);
        }
        public string[] GetAllLanguages()
        {
            return allLanguages.ToArray();
        }
        public void ValidateCurrentLanguage()
        {
            if (allLanguages.Contains(GetCurrentLanguage()))
                return;
            Main.OptionsManager.SetLanguage(allLanguages.FirstOrDefault());
        }
        string IGameLocalization.GetText(string textKey, params string[] args)
        {
            return _(textKey, args);
        }
        string IGameLocalization.GetTextParticular(string textKey, string context, params string[] args)
        {
            return _p(context, textKey, args);
        }
        public event Action<string> OnLanguageChanged;
        public MainManager Main => main;
        public const string CN = "zh-Hans";
        public const string EN = "en-US";
        public const string SOURCE_LANGUAGE = CN;

        [TranslateMsg("当前语言名称", VanillaStrings.CONTEXT_LANGUAGE_NAME)]
        public const string CURRENT_LANGUAGE_NAME = "中文";

        private List<string> allLanguages = new List<string>() { SOURCE_LANGUAGE };
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private DebugLanguage debugLanguage;
    }
    public enum DebugLanguage
    {
        Default,
        Chinese,
        English
    }
}
