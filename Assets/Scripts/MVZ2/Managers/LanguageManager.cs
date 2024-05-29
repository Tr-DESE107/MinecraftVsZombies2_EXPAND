using System;
using System.Collections.Generic;
using System.IO;
using MVZ2.Localization;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class LanguageManager : MonoBehaviour
    {
        public void LoadAllLanguagePacks()
        {
            var directory = GetLanguagePackDirectory();
            var packs = Directory.GetFiles(directory, "*.pack", SearchOption.AllDirectories);
            foreach (var pack in packs)
            {
                var loaded = LanguagePack.Read(pack);
                if (loaded == null)
                    continue;
                languagePacks.Add(loaded);
                foreach (var lang in loaded.GetLanguages())
                {
                    if (!allLanguages.Contains(lang))
                    {
                        allLanguages.Add(lang);
                    }
                }
            }
        }
        public static string GetLanguagePackDirectory()
        {
            if (Application.isEditor)
            {
                var path = Application.dataPath;
                path = Path.GetDirectoryName(path);
                return Path.Combine(path, "ExternalData", "languages");
            }
            return Path.Combine(Application.streamingAssetsPath, "languages");
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
        public LanguagePack[] GetAllLanguagePacks()
        {
            return languagePacks.ToArray();
        }
        public string GetCurrentLanguage()
        {
            return currentLanaguge;
        }
        public string[] GetAllLanguages()
        {
            return allLanguages.ToArray();
        }
        public event Action<string> OnLanguageChanged;
        public MainManager Main => main;
        public const string CN = "zh-Hans";
        public const string EN = "en-US";
        private string currentLanaguge = CN;
        private List<string> allLanguages = new List<string>() { CN };
        private List<LanguagePack> languagePacks = new List<LanguagePack>();
        [SerializeField]
        private MainManager main;
    }
}
