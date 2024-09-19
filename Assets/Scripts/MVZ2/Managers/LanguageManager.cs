using System;
using System.Collections.Generic;
using System.Globalization;
using MukioI18n;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public partial class LanguageManager : MonoBehaviour
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
            if (TryGetLocalizedString(text, language, out var str, args))
                return str;
            return string.Format(text, args);
        }
        public bool TryGetLocalizedString(string text, string language, out string translated, params object[] args)
        {
            var languagePacks = GetAllLanguagePacks();
            foreach (var languagePack in languagePacks)
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
            if (TryGetLocalizedStringParticular(context, text, language, out var str, args))
                return str;
            return string.Format(text, args);
        }
        public bool TryGetLocalizedStringParticular(string context, string text, string language, out string translated, params object[] args)
        {
            var languagePacks = GetAllLanguagePacks();
            foreach (var languagePack in languagePacks)
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
                var cultureInfo = CultureInfo.GetCultureInfo(language);
                return cultureInfo.NativeName;
            }
            catch (CultureNotFoundException)
            {
                return _p(StringTable.CONTEXT_LANGUAGE_NAME, language);
            }
        }
        public Sprite GetSprite(Sprite sprite)
        {
            return GetLocalizedSprite(sprite, GetCurrentLanguage());
        }
        public Sprite GetSprite(NamespaceID id)
        {
            return GetLocalizedSprite(id, GetCurrentLanguage()) ?? main.ResourceManager.GetSprite(id);
        }
        public Sprite GetSprite(SpriteReference spriteRef)
        {
            return GetLocalizedSprite(spriteRef, GetCurrentLanguage()) ?? main.ResourceManager.GetSprite(spriteRef);
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
            var languagePacks = GetAllLanguagePacks();
            foreach (var languagePack in languagePacks)
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
            var languagePacks = GetAllLanguagePacks();
            foreach (var languagePack in languagePacks)
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
        public string[] GetAllLanguages()
        {
            return allLanguages.ToArray();
        }
        public event Action<string> OnLanguageChanged;
        public MainManager Main => main;
        public const string CN = "zh-Hans";
        public const string EN = "en-US";

        private List<string> allLanguages = new List<string>() { CN };
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
