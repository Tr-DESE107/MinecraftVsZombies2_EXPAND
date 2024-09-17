using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            options.difficulty = new NamespaceID(Main.BuiltinNamespace, "normal");
            if (!PlayerPrefs.HasKey("Language"))
            {
                PlayerPrefs.SetString("Language", GetEnvironmentLanguage());
            }
            options.language = PlayerPrefs.GetString("Language");
        }
        public string GetLanguage()
        {
            return options.language;
        }
        public void SetLanguage(string language)
        {
            options.language = language;
        }
        public NamespaceID GetDifficulty()
        {
            return options.difficulty;
        }
        public void SetDifficulty(NamespaceID difficulty)
        {
            options.difficulty = difficulty;
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
        private Options options;
    }
    public class Options
    {
        public NamespaceID difficulty;
        public string language;
    }
}
