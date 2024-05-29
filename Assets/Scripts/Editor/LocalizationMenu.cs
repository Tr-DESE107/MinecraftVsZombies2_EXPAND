using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MukioI18n;
using MVZ2.Localization;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace MVZ2.Editor
{
    public class LocalizationMenu
    {
        [MenuItem("Custom/Localization/Update Script Translations")]
        public static void UpdateScriptTranslations()
        {
            var potGenerator = new MukioPotGenerator("MinecraftVSZombies2", "Cuerzor");
            TranslateMsgAttributeFinder.FindAll(potGenerator);
            potGenerator.WriteOut(GetPoTemplatePath());
            Debug.Log("Script Translations Updated.");
        }
        [MenuItem("Custom/Localization/Compress Langauge Pack")]
        public static void CompressLanguagePack()
        {
            var path = LanguageManager.GetLanguagePackDirectory();
            var dirPath = Path.Combine(path, "builtin");
            var destPath = Path.Combine(path, "builtin.pack");
            LanguagePack.Compress(dirPath, destPath);
            Debug.Log("Langauge Pack Compressed.");
        }
        private static string GetPoTemplatePath()
        {
            return Path.Combine(Application.dataPath, "Localization", "mvz2.pot");
        }
    }
}
