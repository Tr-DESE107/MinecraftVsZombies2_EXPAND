using System.CodeDom;
using System.IO;
using System.IO.Compression;
using System.Xml;
using MukioI18n;
using MVZ2.IO;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Modding;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.Editor
{
    public class LocalizationMenu
    {
        [MenuItem("Custom/Assets/Localization/Update Translations")]
        public static void UpdateTranslations()
        {
            var potGenerator = new MukioPotGenerator("MinecraftVSZombies2", "Cuerzor");
            var active = SceneManager.GetActiveScene().path;

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            foreach (var group in settings.groups)
            {
                foreach (var entry in group.entries)
                {
                    if (!entry.IsScene)
                        continue;
                    var scenePath = entry.AssetPath;

                    var sc = EditorSceneManager.OpenScene(scenePath);
                    if (sc == null)
                        continue;

                    var objects = sc.GetRootGameObjects();
                    foreach (var item in objects)
                    {
                        SearchObject(item.transform, potGenerator);
                    }
                }
            }

            TranslateMsgAttributeFinder.FindAll(potGenerator);

            potGenerator.WriteOut(GetPoTemplatePath("general.pot"));
            Debug.Log("Script Translations Updated.");
            EditorSceneManager.OpenScene(active);
        }
        [MenuItem("Custom/Assets/Localization/Update Almanac Translations")]
        public static void UpdateAlmanacTranslations()
        {
            var potGenerator = new MukioPotGenerator("MinecraftVSZombies2", "Cuerzor");

            var spaceName = "mvz2";
            var metaDirectory = Path.Combine(Application.dataPath, "GameContent", "Assets", spaceName, "metas");
            var almanacPath = Path.Combine(metaDirectory, "almanac.xml");
            var entitiesPath = Path.Combine(metaDirectory, "entities.xml");
            var charactersPath = Path.Combine(metaDirectory, "talkcharacters.xml");

            using FileStream almanacStream = File.Open(almanacPath, FileMode.Open);
            using FileStream entitiesStream = File.Open(entitiesPath, FileMode.Open);
            using FileStream talkcharactersStream = File.Open(charactersPath, FileMode.Open);

            var almanacDocument = almanacStream.ReadXmlDocument();
            var entitiesDocument = entitiesStream.ReadXmlDocument();
            var talkcharacterDocument = talkcharactersStream.ReadXmlDocument();

            var almanacEntryList = AlmanacMetaList.FromXmlNode(almanacDocument["almanac"], spaceName);
            var entitiesList = EntityMetaList.FromXmlNode(entitiesDocument["entities"], spaceName);
            var characterList = TalkCharacterMetaList.FromXmlNode(talkcharacterDocument["characters"], spaceName);

            var almanacReference = "Almanac meta file";
            var entitiesReference = "Entity meta file";
            var characterReference = "Character meta file";
            foreach (var pair in almanacEntryList.entries)
            {
                var category = pair.Key;
                var entries = pair.Value;
                foreach (var entry in entries)
                {
                    AddTranslation(entry.name, almanacReference, $"Name for {category} {entry.id}", $"{category}.name");
                    var context = VanillaStrings.GetAlmanacDescriptionContext(category);
                    AddTranslation(entry.header, almanacReference, $"Header for {category} {entry.id}", context);
                    AddTranslation(entry.properties, almanacReference, $"Properties for {category} {entry.id}", context);
                    AddTranslation(entry.flavor, almanacReference, $"Flavor for {category} {entry.id}", context);
                }
            }
            foreach (var meta in entitiesList.metas)
            {
                var id = new NamespaceID(spaceName, meta.ID);
                AddTranslation(meta.Name, entitiesReference, $"Header for entity {id}", $"entity.name");
                AddTranslation(meta.Tooltip, entitiesReference, $"Properties for entity {id}", $"entity.tooltip");
                AddTranslation(meta.DeathMessage, entitiesReference, $"Death message for entity {id}", $"death_message");
            }
            foreach (var meta in characterList.metas)
            {
                var id = new NamespaceID(spaceName, meta.id);
                AddTranslation(meta.name, characterReference, $"Name for character {id}", $"character.name");
            }
            potGenerator.WriteOut(GetPoTemplatePath("almanac.pot"));
            Debug.Log("Almanac Translations Updated.");

            void AddTranslation(string text, string reference = null, string comment = null, string context = null)
            {
                if (string.IsNullOrEmpty(text))
                    return;
                potGenerator.AddString(new PotTranslate(text, reference, comment, context));
            }
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
            return Path.Combine(Application.dataPath, "Localization", "templates", fileName);
        }
    }
}
