using System.IO;
using System.IO.Compression;
using MukioI18n;
using MVZ2.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVZ2.Editor
{
    public class LocalizationMenu
    {
        [MenuItem("Custom/Assets/Localization/Update Translations")]
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
        [MenuItem("Custom/Assets/Localization/Compress Langauge Pack")]
        public static void CompressLanguagePack()
        {
            var path = GetLanguagePackDirectory();
            var dirPath = Path.Combine(Application.dataPath, "Localization", "pack");
            var destPath = Path.Combine(path, "builtin.bytes");
            CompressLanguagePack(dirPath, destPath);
            AssetDatabase.Refresh();
            Debug.Log("Langauge Pack Compressed.");
        }
        public static void CompressLanguagePack(string sourceDirectory, string destPath)
        {
            FileHelper.ValidateDirectory(destPath);
            var sourceDirInfo = new DirectoryInfo(sourceDirectory);
            var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
            using var stream = File.Open(destPath, FileMode.Create);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            foreach (var filePath in files)
            {
                if (Path.GetExtension(filePath) == ".meta")
                    continue;
                var entryName = Path.GetRelativePath(sourceDirectory, filePath);
                var entry = archive.CreateEntryFromFile(filePath, entryName);
            }
        }
        public static string GetLanguagePackDirectory()
        {
            return Path.Combine(Application.dataPath, "GameContent", "LanguagePacks");
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
