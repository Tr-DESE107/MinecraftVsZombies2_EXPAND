﻿using System.Collections.Generic;
using System.IO;
using System.Xml;
using MukioI18n;
using MVZ2.IO;
using MVZ2.Localization;
using MVZ2.Metas;
using MVZ2.TalkData;
using MVZ2.Vanilla;
using Newtonsoft.Json;
using PVZEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVZ2.Editor
{
    public class LocalizationMenu
    {
        public static void CompressLanguagePack()
        {
            UpdateLocalizedSpriteManifest();
            MoveMOFiles();

            var path = GetLanguagePackDirectory();
            var dirPath = Path.Combine(Application.dataPath, "Localization", "pack");
            var destPath = Path.Combine(path, "builtin.bytes");
            LanguageManager.CompressLanguagePack(dirPath, destPath);
            AssetDatabase.Refresh();
            Debug.Log("Langauge Pack Compressed.");
        }
        public static void UpdateAlmanacTranslations()
        {
            var potGenerator = new MukioPotGenerator("MinecraftVSZombies2", "Cuerzor");

            var spaceName = "mvz2";

            var almanacDocument = LoadMetaXmlDocument(spaceName, "almanac.xml");
            var entitiesDocument = LoadMetaXmlDocument(spaceName, "entities.xml");
            var talkcharacterDocument = LoadMetaXmlDocument(spaceName, "talkcharacters.xml");
            var artifactsDocument = LoadMetaXmlDocument(spaceName, "artifacts.xml");
            var blueprintsDocument = LoadMetaXmlDocument(spaceName, "blueprints.xml");

            var almanacEntryList = AlmanacMetaList.FromXmlNode(almanacDocument["almanac"], spaceName);
            var entitiesList = EntityMetaList.FromXmlNode(entitiesDocument["entities"], spaceName);
            var characterList = TalkCharacterMetaList.FromXmlNode(talkcharacterDocument["characters"], spaceName);
            var artifactsList = ArtifactMetaList.FromXmlNode(artifactsDocument["artifacts"], spaceName);
            var blueprintsList = BlueprintMetaList.FromXmlNode(blueprintsDocument["blueprints"], spaceName);

            var entitiesReference = "Entity meta file";
            var characterReference = "Character meta file";
            var artifactsReference = "Artifact meta file";
            var blueprintsReference = "Blueprints meta file";
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
            foreach (var meta in entitiesList.counters)
            {
                var id = new NamespaceID(spaceName, meta.ID);
                AddTranslation(potGenerator, meta.Name, entitiesReference, $"Name for entity counter {id}", VanillaStrings.CONTEXT_ENTITY_COUNTER_NAME);
            }
            foreach (var meta in entitiesList.metas)
            {
                var id = new NamespaceID(spaceName, meta.ID);
                AddTranslation(potGenerator, meta.Name, entitiesReference, $"Name for entity {id}", VanillaStrings.CONTEXT_ENTITY_NAME);
                AddTranslation(potGenerator, meta.Tooltip, entitiesReference, $"Tooltip for entity {id}", VanillaStrings.CONTEXT_ENTITY_TOOLTIP);
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
            foreach (var option in blueprintsList.Options)
            {
                AddTranslation(potGenerator, option.Name, blueprintsReference, $"Name for blueprint option {option.ID}", VanillaStrings.CONTEXT_OPTION_NAME);
            }
            potGenerator.WriteOut(GetPoTemplatePath("almanac.pot"));
            Debug.Log("Almanac Translations Updated.");
        }
        public static void UpdateAllTranslations()
        {
            UpdateGeneralTranslations();
            UpdateAlmanacTranslations();
            UpdateTalkTranslations();
        }
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
                AddTranslation(potGenerator, music.Source, musicsReference, $"Source for music {id}", VanillaStrings.CONTEXT_MUSIC_SOURCE);
                AddTranslation(potGenerator, music.Origin, musicsReference, $"Origin for music {id}", VanillaStrings.CONTEXT_MUSIC_ORIGIN);
                AddTranslation(potGenerator, music.Author, musicsReference, $"Author for music {id}", VanillaStrings.CONTEXT_MUSIC_AUTHOR);
                AddTranslation(potGenerator, music.Description, musicsReference, $"Description for music {id}", VanillaStrings.CONTEXT_MUSIC_DESCRIPTION);
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
            // 网格
            {
                var gridDocument = LoadMetaXmlDocument(spaceName, "grids.xml");
                var gridList = GridMetaList.FromXmlNode(gridDocument["grids"], spaceName);
                var gridReference = "Grid meta file";
                foreach (var error in gridList.errors)
                {
                    var id = new NamespaceID(spaceName, error.ID);
                    AddTranslation(potGenerator, error.Message, gridReference, $"Message for grid error {id}", VanillaStrings.CONTEXT_ADVICE);
                }
            }
            // 商店
            var storeDocument = LoadMetaXmlDocument(spaceName, "store.xml");
            var storeList = StoreMetaList.FromXmlNode(storeDocument["store"], spaceName);
            var storeReference = "Store meta file";
            foreach (var chatGroup in storeList.Chats)
            {
                foreach (var chat in chatGroup.Chats)
                {
                    AddTranslation(potGenerator, chat.Text, storeReference, $"Store chat from character {chatGroup.Character}", VanillaStrings.CONTEXT_STORE_TALK);
                }
            }
            foreach (var product in storeList.Products)
            {
                var id = new NamespaceID(spaceName, product.ID);
                foreach (var talk in product.Talks)
                {
                    AddTranslation(potGenerator, talk.Text, storeReference, $"Store item description of {id} by character {talk.Character}", VanillaStrings.CONTEXT_STORE_TALK);
                }
                foreach (var stage in product.Stages)
                {
                    AddTranslation(potGenerator, stage.Text, storeReference, $"Store item text of {id}");
                }
            }
            // 制作人员名单
            {
                var creditsDocument = LoadMetaXmlDocument(spaceName, "credits.xml");
                var creditsList = CreditMetaList.FromXmlNode(creditsDocument["credits"], spaceName);
                var creditsReference = "Credits meta file";
                foreach (var category in creditsList.categories)
                {
                    AddTranslation(potGenerator, category.Name, creditsReference, $"Name for credits category {category.Name}", VanillaStrings.CONTEXT_CREDITS_CATEGORY);
                    foreach (var entry in category.Entries)
                    {
                        AddTranslation(potGenerator, entry, creditsReference, $"Name for credit staff {category.Name}", VanillaStrings.CONTEXT_STAFF_NAME);
                    }
                }
            }

            potGenerator.WriteOut(GetPoTemplatePath("general.pot"));
            Debug.Log("Script Translations Updated.");
            EditorSceneManager.OpenScene(active);
        }
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
                            AddTranslation(potGenerator, sentence.speakerName, reference, $"Speaker Name for talk sentence {j} in talk group {group.id}'s section {i}", VanillaStrings.CONTEXT_CHARACTER_NAME);
                        }
                    }
                }
            }
            potGenerator.WriteOut(GetPoTemplatePath("talk.pot"));
            Debug.Log("Talk Translations Updated.");
        }
        public static void UpdateLocalizedSpriteManifest()
        {
            // 根据语言包内的已有贴图和项目内的已有贴图生成spriteManifest.json，并将其保存到Localization\pack\assets\mvz2\en-US中。
            var path = GetLanguagePackDirectory();
            var assetsDir = Path.Combine(Application.dataPath, "Localization", "pack", "assets");
            var databaseDir = Path.Combine("Assets", "GameContent", "Assets");
            var factories = new SpriteDataProviderFactories();
            factories.Init();
            foreach (var nspDir in Directory.EnumerateDirectories(assetsDir))
            {
                var nsp = Path.GetRelativePath(assetsDir, nspDir);
                foreach (var langDir in Directory.EnumerateDirectories(nspDir))
                {
                    List<LocalizedSprite> localizedSprites = new List<LocalizedSprite>();
                    List<LocalizedSpriteSheet> localizedSpritesheets = new List<LocalizedSpriteSheet>();

                    // 单片贴图。
                    var spritesDir = Path.Combine(langDir, "sprites");
                    foreach (var spritePath in Directory.EnumerateFiles(spritesDir, "*.png", SearchOption.AllDirectories))
                    {
                        var spriteRelativePath = Path.GetRelativePath(spritesDir, spritePath).Replace("\\", "/");
                        var originSpritePath = Path.Combine(databaseDir, nsp, "sprites", spriteRelativePath).Replace("\\", "/");

                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(originSpritePath);
                        if (!sprite)
                            continue;
                        var localizedSprite = new LocalizedSprite()
                        {
                            name = Path.ChangeExtension(spriteRelativePath, string.Empty).TrimEnd('.'),
                            texture = spriteRelativePath,
                            pivotX = sprite.pivot.x / sprite.rect.width,
                            pivotY = sprite.pivot.y / sprite.rect.height,
                        };
                        localizedSprites.Add(localizedSprite);
                    }

                    // 多片贴图。
                    var spritesheetsDir = Path.Combine(langDir, "spritesheets");
                    foreach (var spritePath in Directory.EnumerateFiles(spritesheetsDir, "*.png", SearchOption.AllDirectories))
                    {
                        var spriteRelativePath = Path.GetRelativePath(spritesheetsDir, spritePath).Replace("\\", "/");
                        var originSpritePath = Path.Combine(databaseDir, nsp, "spritesheets", spriteRelativePath).Replace("\\", "/");
                        var sprites = SpriteMenu.GetOrderedSpriteSheet(originSpritePath, factories);
                        var slices = new List<LocalizedSpriteSheetSlice>();
                        foreach (var sprite in sprites)
                        {
                            slices.Add(new LocalizedSpriteSheetSlice()
                            {
                                x = sprite.rect.x,
                                y = sprite.rect.y,
                                width = sprite.rect.width,
                                height = sprite.rect.height,
                                pivotX = sprite.pivot.x / sprite.rect.width,
                                pivotY = sprite.pivot.y / sprite.rect.height,
                            });
                        }
                        var localizedSpritesheet = new LocalizedSpriteSheet()
                        {
                            name = Path.ChangeExtension(spriteRelativePath, string.Empty).TrimEnd('.'),
                            texture = spriteRelativePath,
                            slices = slices.ToArray()
                        };
                        localizedSpritesheets.Add(localizedSpritesheet);
                    }
                    LocalizedSpriteManifest manifest = new LocalizedSpriteManifest()
                    {
                        sprites = localizedSprites.ToArray(),
                        spritesheets = localizedSpritesheets.ToArray()
                    };
                    var manifestJson = JsonConvert.SerializeObject(manifest, Newtonsoft.Json.Formatting.Indented);

                    var destPath = Path.Combine(langDir, LanguageManager.SPRITE_MANIFEST_FILENAME);
                    FileHelper.ValidateDirectory(destPath);
                    using var stream = File.Open(destPath, FileMode.Create);
                    using var writer = new StreamWriter(stream);
                    writer.Write(manifestJson);

                    Debug.Log($"语言包贴图清单已更新：{destPath}");
                }
            }

        }
        public static void UpdateLanguagePackDemo()
        {
            // 根据语言包内的已有贴图和项目内的已有贴图生成spriteManifest.json，并将其保存到Localization\pack\assets\mvz2\en-US中。
            UpdateLocalizedSpriteManifest();

            // 将spriteManifest.json复制到LanguagePack\templates中。
            Debug.Log("更新语言包贴图清单……");
            var templatesDir = Path.Combine(Application.dataPath, "LanguagePack", "templates");
            var sourceManifestPath = Path.Combine(Application.dataPath, "Localization", "pack", "assets", "mvz2", "en-US", LanguageManager.SPRITE_MANIFEST_FILENAME);
            if (File.Exists(sourceManifestPath))
            {
                var destPath = Path.Combine(templatesDir, LanguageManager.SPRITE_MANIFEST_FILENAME);
                FileHelper.ValidateDirectory(destPath);
                File.Copy(sourceManifestPath, destPath, true);
            }

            // 将*.pot文件复制到LanguagePack\templates\text_templates中。
            Debug.Log("更新语言包文本模板……");
            CopyDirectory(GetPoTemplateDirectory(), Path.Combine(templatesDir, "text_templates"), "*.pot");

            // 将英文的*.po文件复制到LanguagePack\templates\text_en中。
            Debug.Log("更新语言包英文文本翻译……");
            CopyDirectory(GetPoDirectory("en-US"), Path.Combine(templatesDir, "text_en"), "*.po");

            Debug.Log("语言包示例更新完成。");
        }
        private static void MoveMOFiles()
        {
            var sourceDir = Path.Combine(Application.dataPath, "Localization", "source");
            var targetDir = Path.Combine(Application.dataPath, "Localization", "pack", "assets", "mvz2");
            foreach (var file in Directory.GetFiles(sourceDir, "*.mo", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(sourceDir, file);
                var destFile = Path.Combine(targetDir, relativePath);
                File.Copy(file, destFile, true);
            }
        }
        private static void CopyDirectory(string source, string dest, string pattern)
        {
            if (!Directory.Exists(source))
                return;
            if (Directory.Exists(dest))
            {
                Directory.Delete(dest, true);
            }
            Directory.CreateDirectory(dest);
            foreach (var sourcePath in Directory.EnumerateFiles(source, pattern, SearchOption.AllDirectories))
            {
                if (Path.GetExtension(sourcePath) == ".meta")
                    continue;
                var relativePath = Path.GetRelativePath(source, sourcePath);
                var destPath = Path.Combine(dest, relativePath);
                FileHelper.ValidateDirectory(destPath);
                File.Copy(sourcePath, destPath, true);
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
        private static string GetPoTemplateDirectory()
        {
            return Path.Combine(Application.dataPath, "Localization", "source");
        }
        private static string GetPoDirectory(string language)
        {
            return Path.Combine(Application.dataPath, "Localization", "source", language);
        }
        private static string GetPoTemplatePath(string fileName)
        {
            return Path.Combine(GetPoTemplateDirectory(), fileName);
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
            foreach (var flavor in entry.GetAllFlavors())
            {
                AddTranslation(potGenerator, flavor, almanacReference, $"Flavor for {categoryName} {entry.id}", context);
            }
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
