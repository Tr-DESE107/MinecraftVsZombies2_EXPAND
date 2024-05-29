using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MukioI18n;
using MVZ2.Localization;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVZ2.Editor
{
    public class LocalizationMenu
    {
        [MenuItem("Custom/Localization/Update Translations")]
        public static void UpdateTranslations()
        {
            var potGenerator = new MukioPotGenerator("MinecraftVSZombies2", "Cuerzor");
            var active = SceneManager.GetActiveScene().path;

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) 
                    continue;

                var sc = EditorSceneManager.OpenScene(scene.path);
                if (sc == null) 
                    continue;

                var objects = sc.GetRootGameObjects();
                foreach (var item in objects)
                {
                    SearchObject(item.transform, potGenerator);
                }
            }

            TranslateMsgAttributeFinder.FindAll(potGenerator);

            potGenerator.WriteOut(GetPoTemplatePath("mvz2.pot"));
            Debug.Log("Script Translations Updated.");
            EditorSceneManager.OpenScene(active);
        }
        [MenuItem("Custom/Localization/Compress Langauge Pack")]
        public static void CompressLanguagePack()
        {
            var path = LanguageManager.GetLanguagePackDirectory();
            var dirPath = Path.Combine(path, "builtin");
            var destPath = Path.Combine(path, "builtin.pack");
            LanguageManager.CompressLanguagePack(dirPath, destPath);
            Debug.Log("Langauge Pack Compressed.");
        }
        static void SearchObject(Transform tr, MukioPotGenerator pot)
        {
            var cp = tr.GetComponent<ITranslateComponent>();
            AddTranslate(pot, cp);

            for (int i = 0; i < tr.childCount; i++)
            {
                SearchObject(tr.GetChild(i), pot);
            }
        }
        private static void AddTranslate(MukioPotGenerator pot, ITranslateComponent cp)
        {
            if (cp == null)
                return;
            if (cp.Key != null)
            {
                PotTranslate translate = new PotTranslate(cp.Key, cp.Path, cp.Comment, cp.Context);
                pot.AddString(translate);
            }
            if (cp.Keys != null)
            {
                foreach (var key in cp.Keys)
                {
                    PotTranslate translate = new PotTranslate(key, cp.Path, cp.Comment, cp.Context);
                    pot.AddString(translate);
                }
            }
        }
        private static string GetPoTemplatePath(string fileName)
        {
            return Path.Combine(Application.dataPath, "Localization", fileName);
        }
    }
}
