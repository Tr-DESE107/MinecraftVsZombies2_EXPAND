using System.IO;
using System.IO.Compression;
using System.Xml;
using MukioI18n;
using MVZ2.IO;
using MVZ2.Metas;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
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

            // 场景
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
            // 代码
            TranslateMsgAttributeFinder.FindAll(potGenerator);

            // 统计
            var spaceName = "mvz2";
            var statsDocument = LoadMetaXmlDocument(spaceName, "stats.xml");
            var statsEntryList = StatMetaList.FromXmlNode(statsDocument["stats"], spaceName);
            var statsReference = "Stats meta file";
            foreach (var category in statsEntryList.categories)
            {
                var id = new NamespaceID(spaceName, category.ID);
                AddTranslation(potGenerator, category.Name, statsReference, $"Name for stat category {id}", VanillaStrings.CONTEXT_STAT_CATEGORY);
            }

            potGenerator.WriteOut(GetPoTemplatePath("general.pot"));
            Debug.Log("Script Translations Updated.");
            EditorSceneManager.OpenScene(active);
        }
        [MenuItem("Custom/Assets/Localization/Update Almanac Translations")]
        public static void UpdateAlmanacTranslations()
        {
            var potGenerator = new MukioPotGenerator("MinecraftVSZombies2", "Cuerzor");

            var spaceName = "mvz2";

            var almanacDocument = LoadMetaXmlDocument(spaceName, "almanac.xml");
            var entitiesDocument = LoadMetaXmlDocument(spaceName, "entities.xml");
            var talkcharacterDocument = LoadMetaXmlDocument(spaceName, "talkcharacters.xml");
            var artifactsDocument = LoadMetaXmlDocument(spaceName, "artifacts.xml");

            var almanacEntryList = AlmanacMetaList.FromXmlNode(almanacDocument["almanac"], spaceName);
            var entitiesList = EntityMetaList.FromXmlNode(entitiesDocument["entities"], spaceName);
            var characterList = TalkCharacterMetaList.FromXmlNode(talkcharacterDocument["characters"], spaceName);
            var artifactsList = ArtifactMetaList.FromXmlNode(artifactsDocument["artifacts"], spaceName);

            var entitiesReference = "Entity meta file";
            var characterReference = "Character meta file";
            var artifactsReference = "Artifact meta file";
            foreach (var category in almanacEntryList.categories)
            {
                var categoryName = category.name;
                foreach (var group in category.groups)
                {
                    AddTranslation(potGenerator, group.name, almanacReference, $"Name for almanac group {group.id}", VanillaStrings.CONTEXT_ALMANAC_GROUP_NAME);
                    foreach (var entry in group.entries)
                    {
                        AddAlmanacEntryTranslation(potGenerator, entry, categoryName);
                    }
                }
                foreach (var entry in category.entries)
                {
                    AddAlmanacEntryTranslation(potGenerator, entry, categoryName);
                }
            }
            foreach (var meta in entitiesList.metas)
            {
                var id = new NamespaceID(spaceName, meta.ID);
                AddTranslation(potGenerator, meta.Name, entitiesReference, $"Header for entity {id}", VanillaStrings.CONTEXT_ENTITY_NAME);
                AddTranslation(potGenerator, meta.Tooltip, entitiesReference, $"Properties for entity {id}", VanillaStrings.CONTEXT_ENTITY_TOOLTIP);
                AddTranslation(potGenerator, meta.DeathMessage, entitiesReference, $"Death message for entity {id}", VanillaStrings.CONTEXT_DEATH_MESSAGE);
            }
            foreach (var meta in characterList.metas)
            {
                var id = new NamespaceID(spaceName, meta.id);
                AddTranslation(potGenerator, meta.name, characterReference, $"Name for character {id}", $"character.name");
            }
            foreach (var meta in artifactsList.metas)
            {
                var id = new NamespaceID(spaceName, meta.ID);
                AddTranslation(potGenerator, meta.Name, artifactsReference, $"Name for artifact {id}", VanillaStrings.CONTEXT_ARTIFACT_NAME);
                AddTranslation(potGenerator, meta.Tooltip, artifactsReference, $"Tooltip for artifact {id}", VanillaStrings.CONTEXT_ARTIFACT_TOOLTIP);
            }
            potGenerator.WriteOut(GetPoTemplatePath("almanac.pot"));
            Debug.Log("Almanac Translations Updated.");
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

        private static string GetMetaDirectory(string nsp)
        {
            return Path.Combine(Application.dataPath, "GameContent", "Assets", nsp, "metas");
        }
        private static XmlDocument LoadMetaXmlDocument(string nsp, string path)
        {
            var metaDirectory = GetMetaDirectory(nsp);
            var absPath = Path.Combine(metaDirectory, path);
            using FileStream stream = File.Open(absPath, FileMode.Open);
            return stream.ReadXmlDocument();
        }
        private static void AddAlmanacEntryTranslation(MukioPotGenerator potGenerator, AlmanacMetaEntry entry, string categoryName)
        {
            AddTranslation(potGenerator, entry.name, almanacReference, $"Name for {categoryName} {entry.id}", VanillaStrings.GetAlmanacNameContext(categoryName));
            var context = VanillaStrings.GetAlmanacDescriptionContext(categoryName);
            AddTranslation(potGenerator, entry.header, almanacReference, $"Header for {categoryName} {entry.id}", context);
            AddTranslation(potGenerator, entry.properties, almanacReference, $"Properties for {categoryName} {entry.id}", context);
            AddTranslation(potGenerator, entry.flavor, almanacReference, $"Flavor for {categoryName} {entry.id}", context);
        }
        private static void AddTranslation(MukioPotGenerator potGenerator, string text, string reference = null, string comment = null, string context = null)
        {
            if (string.IsNullOrEmpty(text))
                return;
            potGenerator.AddString(new PotTranslate(text, reference, comment, context));
        }
        private static string almanacReference = "Almanac meta file";
    }
}
