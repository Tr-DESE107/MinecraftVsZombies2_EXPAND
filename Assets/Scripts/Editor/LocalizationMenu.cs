using System.IO;
using System.IO.Compression;
using System.Xml;
using MukioI18n;
using MVZ2.IO;
using MVZ2.Metas;
using MVZ2.TalkData;
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
        [MenuItem("Custom/Assets/Localization/Update All Translations")]
        public static void UpdateAllTranslations()
        {
            UpdateGeneralTranslations();
            UpdateAlmanacTranslations();
            UpdateTalkTranslations();
        }
        [MenuItem("Custom/Assets/Localization/Update General Translations")]
        public static void UpdateGeneralTranslations()
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
            // 关卡
            var stageDocument = LoadMetaXmlDocument(spaceName, "stages.xml");
            var stageList = StageMetaList.FromXmlNode(stageDocument["stages"], spaceName);
            var stageReference = "Stage meta file";
            foreach (var meta in stageList.metas)
            {
                var id = new NamespaceID(spaceName, meta.ID);
                AddTranslation(potGenerator, meta.Name, stageReference, $"Name for stage {id}", VanillaStrings.CONTEXT_LEVEL_NAME);
            }
            // 统计
            var statsDocument = LoadMetaXmlDocument(spaceName, "stats.xml");
            var statsEntryList = StatMetaList.FromXmlNode(statsDocument["stats"], spaceName);
            var statsReference = "Stats meta file";
            foreach (var category in statsEntryList.categories)
            {
                var id = new NamespaceID(spaceName, category.ID);
                AddTranslation(potGenerator, category.Name, statsReference, $"Name for stat category {id}", VanillaStrings.CONTEXT_STAT_CATEGORY);
            }
            // 成就
            var achievementsDocument = LoadMetaXmlDocument(spaceName, "achievements.xml");
            var achievementList = AchievementMetaList.FromXmlNode(achievementsDocument["achievements"], spaceName);
            var achievementsReference = "Achievements meta file";
            foreach (var achievement in achievementList.metas)
            {
                var id = new NamespaceID(spaceName, achievement.ID);
                AddTranslation(potGenerator, achievement.Name, achievementsReference, $"Name for achievement {id}", VanillaStrings.CONTEXT_ACHIEVEMENT);
                AddTranslation(potGenerator, achievement.Description, achievementsReference, $"Description for achievement {id}", VanillaStrings.CONTEXT_ACHIEVEMENT);
            }
            // 音乐
            var musicsDocument = LoadMetaXmlDocument(spaceName, "musics.xml");
            var musicsList = MusicMetaList.FromXmlNode(musicsDocument["musics"], spaceName);
            var musicsReference = "Music meta file";
            foreach (var music in musicsList.metas)
            {
                var id = new NamespaceID(spaceName, music.ID);
                AddTranslation(potGenerator, music.Name, musicsReference, $"Name for music {id}", VanillaStrings.CONTEXT_MUSIC_NAME);
            }
            // 档案
            var archiveDocument = LoadMetaXmlDocument(spaceName, "archive.xml");
            var archiveList = ArchiveMetaList.FromXmlNode(archiveDocument["archive"], spaceName);
            var archiveReference = "Archive meta file";
            foreach (var tag in archiveList.Tags)
            {
                var id = new NamespaceID(spaceName, tag.ID);
                AddTranslation(potGenerator, tag.Name, archiveReference, $"Name for archive tag {id}", VanillaStrings.CONTEXT_ARCHIVE_TAG_NAME);
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
                AddTranslation(potGenerator, meta.name, characterReference, $"Name for character {id}", VanillaStrings.CONTEXT_CHARACTER_NAME);
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
        [MenuItem("Custom/Assets/Localization/Update Talk Translations")]
        public static void UpdateTalkTranslations()
        {
            var potGenerator = new MukioPotGenerator("MinecraftVSZombies2", "Cuerzor");

            var spaceName = "mvz2";

            var talkDir = Path.Combine(GetMetaDirectory(spaceName), "talks");
            foreach (var filePath in Directory.GetFiles(talkDir, "*.xml", SearchOption.TopDirectoryOnly))
            {
                using FileStream stream = File.Open(filePath, FileMode.Open);
                var document = stream.ReadXmlDocument();
                var metaList = TalkMeta.FromXmlDocument(document, spaceName);
                var reference = "Talk meta file";
                foreach (var group in metaList.groups)
                {
                    var groupName = group.archive.name;
                    var groupID = new NamespaceID(spaceName, group.id);
                    var groupContext = VanillaStrings.GetTalkTextContext(groupID);
                    AddTranslation(potGenerator, groupName, reference, $"Archive name for talk group {group.id}", VanillaStrings.CONTEXT_ARCHIVE);
                    for (int i = 0; i < group.sections.Count; i++)
                    {
                        var section = group.sections[i];
                        AddTranslation(potGenerator, section.archiveText, reference, $"Archive text for talk group {group.id}'s section {i}", VanillaStrings.CONTEXT_ARCHIVE);
                        for (int j = 0; j < section.sentences.Count; j++)
                        {
                            var sentence = section.sentences[j];
                            AddTranslation(potGenerator, sentence.text, reference, $"Text for talk sentence {j} in talk group {group.id}'s section {i}", groupContext);
                            AddTranslation(potGenerator, sentence.description, reference, $"Description for talk sentence {j} in talk group {group.id}'s section {i}", VanillaStrings.CONTEXT_ARCHIVE); 
                        }
                    }
                }
            }
            potGenerator.WriteOut(GetPoTemplatePath("talk.pot"));
            Debug.Log("Talk Translations Updated.");
        }
        [MenuItem("Custom/Assets/Localization/Compress Langauge Pack")]
        public static void CompressLanguagePack()
        {
            MoveMOFiles();

            var path = GetLanguagePackDirectory();
            var dirPath = Path.Combine(Application.dataPath, "Localization", "pack");
            var destPath = Path.Combine(path, "builtin.bytes");
            CompressLanguagePack(dirPath, destPath);
            AssetDatabase.Refresh();
            Debug.Log("Langauge Pack Compressed.");
        }
        private static void MoveMOFiles()
        {
            var sourceDir = Path.Combine(Application.dataPath, "Localization", "sources");
            var targetDir = Path.Combine(Application.dataPath, "Localization", "pack", "assets", "mvz2");
            foreach (var file in Directory.GetFiles(sourceDir, "*.mo", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(sourceDir, file);
                var destFile = Path.Combine(targetDir, relativePath);
                File.Copy(file, destFile, true);
            }
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
