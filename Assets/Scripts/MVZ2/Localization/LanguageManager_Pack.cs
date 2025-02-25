using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using MVZ2.IO;
using MVZ2.Sprites;
using Newtonsoft.Json;
using NGettext;
using PVZEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVZ2.Localization
{
    public delegate bool TryGetTranslation<in TKey, TResult>(LanguagePack pack, TKey key, out TResult result);
    public partial class LanguageManager
    {
        public async Task LoadAllLanguagePacks()
        {
            var op = Addressables.LoadAssetsAsync<TextAsset>("LanguagePack", textAsset =>
            {
                var bytes = textAsset.bytes;
                var loaded = ReadLanguagePackZip(bytes);
                if (loaded == null)
                    return;
                languagePacks.Add(loaded);
            });
            await op.Task;

            // 加载外部语言包。
            LoadExternalLanguagePacks();

            // TODO：更新启用的语言包列表。
            LoadEnabledLanguagePackList();
            EvaluateEnabledLanguagePacks();
        }
        #region 外部语言包
        private string GetExternalLanguagePackDirectory()
        {
            return Path.Combine(Application.persistentDataPath, externalLangaugePackDir);
        }
        private void LoadExternalLanguagePacks()
        {
            var dir = GetExternalLanguagePackDirectory();
            if (!Directory.Exists(dir))
                return;
            // lang语言包
            foreach (var zip in Directory.EnumerateFiles(dir, "*.lang", SearchOption.TopDirectoryOnly))
            {
                var loaded = ReadLanguagePackZip(zip);
                if (loaded == null)
                    continue;
                languagePacks.Add(loaded);
            }
            // Zip语言包
            foreach (var zip in Directory.EnumerateFiles(dir, "*.zip", SearchOption.TopDirectoryOnly))
            {
                var loaded = ReadLanguagePackZip(zip);
                if (loaded == null)
                    continue;
                languagePacks.Add(loaded);
            }
            // 文件夹语言包
            foreach (var langDir in Directory.EnumerateDirectories(dir))
            {
                var loaded = ReadLanguagePackDirectory(langDir);
                if (loaded == null)
                    continue;
                languagePacks.Add(loaded);
            }
        }
        #endregion
        public void LoadEnabledLanguagePackList()
        {
            for (int i = languagePacks.Count - 1; i >= 0; i--)
            {
                var languagePack = languagePacks[i];
                enabledLanguagePacks.Add(languagePack);
            }
        }
        public void EvaluateEnabledLanguagePacks()
        {
            allLanguages.Clear();
            foreach (var languagePack in enabledLanguagePacks)
            {
                foreach (var lang in languagePack.GetLanguages())
                {
                    if (!allLanguages.Contains(lang))
                    {
                        allLanguages.Add(lang);
                    }
                }
            }
        }
        public LanguagePack[] GetAllLanguagePacks()
        {
            return languagePacks.ToArray();
        }

        #region 加载Zip语言包
        public LanguagePack ReadLanguagePackZip(string path)
        {
            using var stream = File.Open(path, FileMode.Open);
            return ReadLanguagePackZip(stream);
        }
        public LanguagePack ReadLanguagePackZip(byte[] bytes)
        {
            using var memory = new MemoryStream(bytes);
            return ReadLanguagePackZip(memory);
        }
        public LanguagePack ReadLanguagePackZip(Stream stream)
        {
            try
            {
                using var archive = new ZipArchive(stream);

                var metadataEntry = archive.GetEntry(METADATA_FILENAME);
                if (metadataEntry == null)
                    return null;

                var json = metadataEntry.ReadString(Encoding.UTF8);
                LanguagePackMetadata metadata = JsonConvert.DeserializeObject<LanguagePackMetadata>(json);
                var languagePack = new LanguagePack(metadata);
                foreach (var entry in archive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    if (string.IsNullOrEmpty(Path.GetExtension(entry.FullName)))
                        continue;

                    var fullPath = entry.FullName.Replace("\\", "/");
                    var splitedPaths = fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    if (splitedPaths.Length < 3 || splitedPaths[0] != "assets")
                        continue;
                    var nsp = splitedPaths[1];
                    var lang = splitedPaths[2].Replace('_', '-');
                    if (splitedPaths.Length == 4)
                    {
                        var asset = languagePack.GetOrCreateLanguageAsset(lang);
                        string filename = splitedPaths[3];
                        if (Path.GetExtension(filename) == ".mo")
                        {
                            var filenameWithoutExt = Path.GetFileNameWithoutExtension(filename);
                            var catalog = entry.ReadCatalog(lang);
                            asset.catalogs.Add(filenameWithoutExt, catalog);
                        }
                        else if (filename == SPRITE_MANIFEST_FILENAME)
                        {
                            var manifestJson = entry.ReadString(Encoding.UTF8);
                            var manifest = JsonConvert.DeserializeObject<LocalizedSpriteManifest>(manifestJson);
                            foreach (var localizedSprite in manifest.sprites)
                            {
                                var resID = new NamespaceID(nsp, localizedSprite.name);
                                var texturePath = Path.Combine(splitedPaths[0], splitedPaths[1], splitedPaths[2], "sprites", localizedSprite.texture).Replace("/", "\\");
                                var textureEntry = archive.GetEntry(texturePath);
                                var sprite = ReadEntryToSprite(resID, textureEntry, localizedSprite);
                                asset.Sprites.Add(resID, sprite);
                            }
                            foreach (var localizedSpritesheet in manifest.spritesheets)
                            {
                                var resID = new NamespaceID(nsp, localizedSpritesheet.name);
                                var texturePath = Path.Combine(splitedPaths[0], splitedPaths[1], splitedPaths[2], "spritesheets", localizedSpritesheet.texture).Replace("/", "\\");
                                var textureEntry = archive.GetEntry(texturePath);
                                var spritesheet = ReadEntryToSpriteSheet(resID, textureEntry, localizedSpritesheet);
                                asset.SpriteSheets.Add(resID, spritesheet);
                            }
                        }
                    }
                }
                return languagePack;
            }
            catch (Exception e)
            {
                Debug.LogError($"读取语言包Zip失败：{e}");
                return null;
            }
        }
        #endregion

        #region 加载文件夹语言包
        public LanguagePack ReadLanguagePackDirectory(string path)
        {
            var metadataPath = Path.Combine(path, METADATA_FILENAME);
            if (!File.Exists(metadataPath))
                return null;
            try
            {
                using var metadataStream = File.Open(metadataPath, FileMode.Open);
                using var metadataReader = new StreamReader(metadataStream);
                var json = metadataReader.ReadToEnd();
                LanguagePackMetadata metadata = JsonConvert.DeserializeObject<LanguagePackMetadata>(json);

                var languagePack = new LanguagePack(metadata);
                var assetsDir = Path.Combine(path, "assets");
                foreach (var nspDir in Directory.EnumerateDirectories(assetsDir))
                {
                    var nsp = Path.GetRelativePath(assetsDir, nspDir);
                    foreach (var langDir in Directory.EnumerateDirectories(nspDir))
                    {
                        var lang = Path.GetRelativePath(nspDir, langDir);
                        LoadPackLanguageDirectroy(nsp, lang, langDir, languagePack);
                    }
                }
                return languagePack;
            }
            catch (Exception e)
            {
                Debug.LogError($"读取语言包{path}失败：{e}");
                return null;
            }
        }
        private void LoadPackLanguageDirectroy(string nsp, string lang, string dir, LanguagePack pack)
        {
            var asset = pack.GetOrCreateLanguageAsset(lang);
            // 加载文本
            foreach (var moFile in Directory.EnumerateFiles(dir, "*.mo"))
            {
                var filenameWithoutExt = Path.GetFileNameWithoutExtension(moFile);
                var catalog = new Catalog(File.Open(moFile, FileMode.Open), new CultureInfo(lang));
                asset.catalogs.Add(filenameWithoutExt, catalog);
            }
            // 加载贴图
            var manifestPath = Path.Combine(dir, SPRITE_MANIFEST_FILENAME);
            if (File.Exists(manifestPath))
            {
                using var manifestStream = File.Open(manifestPath, FileMode.Open);
                using var manifestReader = new StreamReader(manifestStream);
                var manifestJson = manifestReader.ReadToEnd();

                var manifest = JsonConvert.DeserializeObject<LocalizedSpriteManifest>(manifestJson);
                foreach (var localizedSprite in manifest.sprites)
                {
                    var resID = new NamespaceID(nsp, localizedSprite.name);
                    var texturePath = Path.Combine(dir, "sprites", localizedSprite.texture).Replace("/", "\\");

                    using var spriteStream = File.Open(texturePath, FileMode.Open);
                    using var spriteMemory = new MemoryStream();
                    spriteStream.CopyTo(spriteMemory);
                    var sprite = ReadEntryToSprite(resID, spriteMemory.ToArray(), localizedSprite);
                    asset.Sprites.Add(resID, sprite);
                }
                foreach (var localizedSpritesheet in manifest.spritesheets)
                {
                    var resID = new NamespaceID(nsp, localizedSpritesheet.name);
                    var texturePath = Path.Combine(dir, "spritesheets", localizedSpritesheet.texture).Replace("/", "\\");

                    using var spriteStream = File.Open(texturePath, FileMode.Open);
                    using var spriteMemory = new MemoryStream();
                    spriteStream.CopyTo(spriteMemory);
                    var spritesheet = ReadEntryToSpriteSheet(resID, spriteMemory.ToArray(), localizedSpritesheet);
                    asset.SpriteSheets.Add(resID, spritesheet);
                }
            }
        }
        #endregion

        #region 加载贴图
        private Sprite ReadEntryToSprite(NamespaceID spriteId, ZipArchiveEntry entry, LocalizedSprite meta)
        {
            var bytes = entry?.ReadBytes();
            return ReadEntryToSprite(spriteId, bytes, meta);
        }
        private Sprite[] ReadEntryToSpriteSheet(NamespaceID spriteId, ZipArchiveEntry entry, LocalizedSpriteSheet meta)
        {
            var bytes = entry?.ReadBytes();
            return ReadEntryToSpriteSheet(spriteId, bytes, meta);
        }
        private Sprite ReadEntryToSprite(NamespaceID spriteId, byte[] bytes, LocalizedSprite meta)
        {
            try
            {
                var texture2D = SpriteHelper.LoadTextureFromBytes(bytes);
                Rect spriteRect = new Rect(0, 0, texture2D.width, texture2D.height);
                Vector2 spritePivot;
                if (meta != null)
                {
                    spritePivot = new Vector2(meta.pivotX * spriteRect.width, meta.pivotY * spriteRect.height);
                }
                else
                {
                    spritePivot = new Vector2(0.5f * spriteRect.width, 0.5f * spriteRect.height);
                }
                var spr = main.ResourceManager.CreateSprite(texture2D, spriteRect, spritePivot, spriteId.ToString(), "language");
                return spr;
            }
            catch (Exception e)
            {
                Debug.LogError($"An exception thrown when loading sprite {spriteId} from a language pack: {e}");
                return Main.ResourceManager.GetDefaultSpriteClone();
            }
        }
        private Sprite[] ReadEntryToSpriteSheet(NamespaceID spriteId, byte[] bytes, LocalizedSpriteSheet meta)
        {
            try
            {
                var texture2D = SpriteHelper.LoadTextureFromBytes(bytes);
                (Rect rect, Vector2 pivot)[] spriteInfos;
                if (meta != null)
                {
                    spriteInfos = new (Rect rect, Vector2 pivot)[meta.slices.Length];
                    for (int i = 0; i < spriteInfos.Length; i++)
                    {
                        var slice = meta.slices[i];
                        var rect = new Rect(slice.x, slice.y, slice.width, slice.height);
                        var pivot = new Vector2(slice.pivotX * slice.width, slice.pivotY * slice.height);
                        spriteInfos[i] = (rect, pivot);
                    }
                }
                else
                {
                    spriteInfos = new (Rect rect, Vector2 pivot)[1]
                    {
                        (new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(texture2D.width * 0.5f, texture2D.height * 0.5f))
                    };
                }
                var sprites = new Sprite[spriteInfos.Length];
                for (int i = 0; i < sprites.Length; i++)
                {
                    var info = spriteInfos[i];
                    var rect = info.rect;
                    rect.width = Math.Min(rect.width, texture2D.width);
                    rect.height = Math.Min(rect.height, texture2D.height);
                    var spr = main.ResourceManager.CreateSprite(texture2D, rect, info.pivot / rect.size, $"{spriteId}[{i}]", "language");
                    sprites[i] = spr;
                }
                return sprites;
            }
            catch (Exception e)
            {
                Debug.LogError($"An exception thrown when loading spritesheet {spriteId} from a language pack: {e}");
                var length = meta.slices?.Length ?? 1;
                var sprites = new Sprite[length];
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i] = Main.ResourceManager.GetDefaultSpriteClone();
                }
                return sprites;
            }
        }
        #endregion

        public const string METADATA_FILENAME = "pack.json";
        public const string SPRITE_MANIFEST_FILENAME = "sprite_manifest.json";
        [SerializeField]
        private string externalLangaugePackDir = "language_packs";
        private List<LanguagePack> languagePacks = new List<LanguagePack>();
        private List<LanguagePack> enabledLanguagePacks = new List<LanguagePack>();
    }
}
