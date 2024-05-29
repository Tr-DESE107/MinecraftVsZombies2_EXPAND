using System;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class LanguageManager : MonoBehaviour
    {
        public Sprite GetLocalizedSprite(NamespaceID spriteID, string language)
        {
            var localizedSpr = Main.ResourceManager.GetSprite(spriteID.spacename, $"{spriteID.path}.{language}");
            if (localizedSpr != null)
                return localizedSpr;
            return Main.ResourceManager.GetSprite(spriteID);
        }
        public string GetCurrentLanguage()
        {
            return currentLanaguge;
        }
        public string[] GetAllLanguages()
        {
            return allLanguages;
        }
        public event Action<string> OnLanguageChanged;
        public MainManager Main => main;
        public const string CN = "zh-Hans";
        public const string EN = "en-US";
        private string currentLanaguge = CN;
        private string[] allLanguages = new string[]
        {
            CN,
            EN,
        };
        [SerializeField]
        private MainManager main;
    }
}
