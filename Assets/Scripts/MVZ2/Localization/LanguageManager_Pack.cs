using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.IO;
using MVZ2.Sprites;
using Newtonsoft.Json;
using NGettext;
using PVZEngine;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVZ2.Localization
{
    public delegate bool TryGetTranslation<in TKey, TResult>(LanguagePack pack, TKey key, out TResult result);
    public partial class LanguageManager
    {
        public async Task InitLanguagePacks()
        {
            // 加载所有语言包引用。
            var references = EvaluateAllLanguagePackReferences();
            foreach (var reference in references)
            {
                await LoadLanguagePackMetadata(reference);
            }

            // 加载启用的语言包列表。
            LoadEnabledLanguagePackList();

            // 加载启用的语言包内容。
            await LoadLanguagePacks();
        }
        /// <summary>
        /// 刷新语言包引用列表。
        /// 获取所有外部语言包引用，去掉不存在的，加入新添加的。
        /// 然后再次检测
        /// </summary>
        /// <returns>语言包列表是否有变动。</returns>
        public async Task<bool> RefreshLanguagePackReferences()
        {
            var references = EvaluateAllLanguagePackReferences();
            var removed = languagePackMetadatas.Keys.Except(references).ToArray();
            var added = references.Except(languagePackMetadatas.Keys).ToArray();
            bool changed = false;
            if (removed.Count() > 0)
            {
                foreach (var remove in removed)
                {
                    languagePackMetadatas.Remove(remove);
                }
                changed = true;
            }
            if (added.Count() > 0)
            {
                foreach (var add in added)
                {
                    await LoadLanguagePackMetadata(add);
                }
                changed = true;
            }
            return changed;
        }
        public async Task ReloadLanguagePacks(LanguagePackReference[] enabled)
        {
            enabledLanguagePacks.Clear();
            enabledLanguagePacks.AddRange(enabled.Where(r => languagePackMetadatas.Keys.Contains(r)));
            UnloadLanguagePacks();
            await LoadLanguagePacks();

            if (!allLanguages.Contains(GetCurrentLanguage()))
            {
                Main.OptionsManager.SetLanguage(allLanguages.FirstOrDefault());
            }
        }

        #region 语言包引用/元数据
        public LanguagePackMetadata GetLanguagePackMetadata(LanguagePackReference reference)
        {
            if (languagePackMetadatas.TryGetValue(reference, out var metadata))
            {
                return metadata;
            }
            return null;
        }
        public LanguagePackReference[] GetAllLanguagePackReferences()
        {
            return languagePackMetadatas.Keys.ToArray();
        }
        private LanguagePackReference[] EvaluateAllLanguagePackReferences()
        {
            List<LanguagePackReference> results = new List<LanguagePackReference>();
            results.Add(builtinLanguagePackRef);

            // 外部引用。
            var dir = GetExternalLanguagePackDirectory();
            if (Directory.Exists(dir))
            {
                // lang语言包
                foreach (var zip in Directory.EnumerateFiles(dir, "*.lang", SearchOption.TopDirectoryOnly))
                {
                    results.Add(new ExternalLanguagePackReference(zip, false));
                }
                // Zip语言包
                foreach (var zip in Directory.EnumerateFiles(dir, "*.zip", SearchOption.TopDirectoryOnly))
                {
                    results.Add(new ExternalLanguagePackReference(zip, false));
                }
                // 文件夹语言包
                foreach (var langDir in Directory.EnumerateDirectories(dir))
                {
                    results.Add(new ExternalLanguagePackReference(langDir, true));
                }
            }
            return results.ToArray();
        }
        private string GetExternalLanguagePackDirectory()
        {
            return Path.Combine(Application.persistentDataPath, externalLangaugePackDir);
        }
        private async Task LoadLanguagePackMetadata(LanguagePackReference reference)
        {
            try
            {
                var metadata = await reference.LoadMetadata(this);
                languagePackMetadatas.Add(reference, metadata);
            }
            catch (Exception e)
            {
                Debug.LogError($"加载语言包引用{reference}失败：{e}");
            }
        }
        #endregion

        #region 启用状态
        public void LoadEnabledLanguagePackList()
        {
            enabledLanguagePacks.Clear();
            enabledLanguagePacks.AddRange(languagePackMetadatas.Reverse().Select(p => p.Key));
        }
        public LanguagePackReference[] GetEnabledLanguagePackList()
        {
            return enabledLanguagePacks.ToArray();
        }
        #endregion

        #region 加载语言包
        public async Task LoadLanguagePacks()
        {
            foreach (var reference in enabledLanguagePacks)
            {
                var languagePack = await reference.LoadLanguagePack(this);
                if (languagePack == null)
                    continue;
                loadedLanguagePacks.Add(languagePack);
                foreach (var lang in languagePack.GetLanguages())
                {
                    if (!allLanguages.Contains(lang))
                    {
                        allLanguages.Add(lang);
                    }
                }
            }
        }
        #endregion

        #region 卸载语言包
        public void UnloadLanguagePacks()
        {
            foreach (var enabledLanguagePack in loadedLanguagePacks)
            {
                UnloadLanguagePack(enabledLanguagePack);
            }

            allLanguages.Clear();
            allLanguages.Add(SOURCE_LANGUAGE);
            loadedLanguagePacks.Clear();
        }
        public void UnloadLanguagePack(LanguagePack pack)
        {
            foreach (var lang in pack.GetLanguages())
            {
                var assets = pack.GetLanguageAssets(lang);
                if (assets == null)
                    continue;
                foreach (var pair in assets.Sprites)
                {
                    main.ResourceManager.RemoveCreatedSprite(pair.Value, pair.Key.ToString(), GetLanguagePackSpriteCategory(pack.Key));
                }
                foreach (var pair in assets.SpriteSheets)
                {
                    for (int i = 0; i < pair.Value.Length; i++)
                    {
                        var sprite = pair.Value[i];
                        main.ResourceManager.RemoveCreatedSprite(sprite, $"{pair.Key}[{i}]", GetLanguagePackSpriteCategory(pack.Key));
                    }
                }
            }
        }
        #endregion

        #region 加载Zip元数据
        public LanguagePackMetadata ReadLanguagePackMetadataZip(string path)
        {
            using var stream = File.Open(path, FileMode.Open);
            return ReadLanguagePackMetadataZip(stream);
        }
        public LanguagePackMetadata ReadLanguagePackMetadataZip(byte[] bytes)
        {
            using var memory = new MemoryStream(bytes);
            return ReadLanguagePackMetadataZip(memory);
        }
        public LanguagePackMetadata ReadLanguagePackMetadataZip(Stream stream)
        {
            try
            {
                using var archive = new ZipArchive(stream);

                var metadataEntry = archive.GetEntry(METADATA_FILENAME);
                if (metadataEntry == null)
                    return null;

                var json = metadataEntry.ReadString(Encoding.UTF8);
                // TODO：图标
                return JsonConvert.DeserializeObject<LanguagePackMetadata>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"读取语言包Zip的元数据失败：{e}");
                return null;
            }
        }
        #endregion

        #region 加载Zip语言包
        public LanguagePack ReadLanguagePackZip(string path)
        {
            using var stream = File.Open(path, FileMode.Open);
            return ReadLanguagePackZip(Path.GetFileName(path), stream);
        }
        public LanguagePack ReadLanguagePackZip(string key, byte[] bytes)
        {
            using var memory = new MemoryStream(bytes);
            return ReadLanguagePackZip(key, memory);
        }
        public LanguagePack ReadLanguagePackZip(string key, Stream stream)
        {
            try
            {
                using var archive = new ZipArchive(stream);
                var languagePack = new LanguagePack(key);
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

                                if (textureEntry == null)
                                    continue;
                                var bytes = textureEntry.ReadBytes();
                                var sprite = ReadEntryToSprite(resID, bytes, localizedSprite, key);
                                asset.Sprites.Add(resID, sprite);
                            }
                            foreach (var localizedSpritesheet in manifest.spritesheets)
                            {
                                var resID = new NamespaceID(nsp, localizedSpritesheet.name);
                                var texturePath = Path.Combine(splitedPaths[0], splitedPaths[1], splitedPaths[2], "spritesheets", localizedSpritesheet.texture).Replace("/", "\\");
                                var textureEntry = archive.GetEntry(texturePath);

                                if (textureEntry == null)
                                    continue;
                                var bytes = textureEntry.ReadBytes();
                                var spritesheet = ReadEntryToSpriteSheet(resID, bytes, localizedSpritesheet, key);
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

        #region 加载文件夹元数据
        public LanguagePackMetadata ReadLanguagePackMetadataDirectory(string path)
        {
            var metadataPath = Path.Combine(path, METADATA_FILENAME);
            if (!File.Exists(metadataPath))
                return null;
            try
            {
                using var metadataStream = File.Open(metadataPath, FileMode.Open);
                using var metadataReader = new StreamReader(metadataStream);
                var json = metadataReader.ReadToEnd();
                return JsonConvert.DeserializeObject<LanguagePackMetadata>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"读取语言包{path}的元数据失败：{e}");
                return null;
            }
        }
        #endregion

        #region 加载文件夹语言包
        public LanguagePack ReadLanguagePackDirectory(string path)
        {
            try
            {
                var key = Path.GetFileName(path);
                var languagePack = new LanguagePack(key);
                var assetsDir = Path.Combine(path, "assets");
                foreach (var nspDir in Directory.EnumerateDirectories(assetsDir))
                {
                    var nsp = Path.GetRelativePath(assetsDir, nspDir);
                    foreach (var langDir in Directory.EnumerateDirectories(nspDir))
                    {
                        var lang = Path.GetRelativePath(nspDir, langDir);
                        LoadPackLanguageDirectroy(nsp, lang, langDir, languagePack, key);
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
        private void LoadPackLanguageDirectroy(string nsp, string lang, string dir, LanguagePack pack, string key)
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

                    var sprite = ReadEntryToSprite(resID, spriteMemory.ToArray(), localizedSprite, key);
                    asset.Sprites.Add(resID, sprite);
                }
                foreach (var localizedSpritesheet in manifest.spritesheets)
                {
                    var resID = new NamespaceID(nsp, localizedSpritesheet.name);
                    var texturePath = Path.Combine(dir, "spritesheets", localizedSpritesheet.texture).Replace("/", "\\");

                    using var spriteStream = File.Open(texturePath, FileMode.Open);
                    using var spriteMemory = new MemoryStream();
                    spriteStream.CopyTo(spriteMemory);

                    var spritesheet = ReadEntryToSpriteSheet(resID, spriteMemory.ToArray(), localizedSpritesheet, key);
                    asset.SpriteSheets.Add(resID, spritesheet);
                }
            }
        }
        #endregion

        #region 加载贴图
        private Sprite ReadEntryToSprite(NamespaceID spriteId, byte[] bytes, LocalizedSprite meta, string key)
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
                var spr = main.ResourceManager.CreateSprite(texture2D, spriteRect, spritePivot, spriteId.ToString(), GetLanguagePackSpriteCategory(key));
                return spr;
            }
            catch (Exception e)
            {
                Debug.LogError($"An exception thrown when loading sprite {spriteId} from a language pack: {e}");
                return Main.ResourceManager.GetDefaultSpriteClone();
            }
        }
        private Sprite[] ReadEntryToSpriteSheet(NamespaceID spriteId, byte[] bytes, LocalizedSpriteSheet meta, string key)
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
                    var spr = main.ResourceManager.CreateSprite(texture2D, rect, info.pivot / rect.size, $"{spriteId}[{i}]", GetLanguagePackSpriteCategory(key));
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
        private string GetLanguagePackSpriteCategory(string key)
        {
            return $"language-{key}";
        }
        #endregion

        public const string METADATA_FILENAME = "pack.json";
        public const string SPRITE_MANIFEST_FILENAME = "sprite_manifest.json";
        [SerializeField]
        private string externalLangaugePackDir = "language_packs";
        private Dictionary<LanguagePackReference, LanguagePackMetadata> languagePackMetadatas = new Dictionary<LanguagePackReference, LanguagePackMetadata>();
        private List<LanguagePackReference> enabledLanguagePacks = new List<LanguagePackReference>();
        private List<LanguagePack> loadedLanguagePacks = new List<LanguagePack>();
        private BuiltinLanguagePackReference builtinLanguagePackRef = new BuiltinLanguagePackReference();
        private class BuiltinLanguagePackReference : LanguagePackReference
        {
            public override bool IsBuiltin => true;
            public override bool Equals(object obj)
            {
                return obj is BuiltinLanguagePackReference;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string GetKey()
            {
                return "builtin";
            }
            public override async Task<LanguagePackMetadata> LoadMetadata(LanguageManager manager)
            {
                var op = Addressables.LoadAssetAsync<TextAsset>("LanguagePack");
                var textAsset = await op.Task;
                return manager.ReadLanguagePackMetadataZip(textAsset.bytes);
            }
            public override async Task<LanguagePack> LoadLanguagePack(LanguageManager manager)
            {
                var op = Addressables.LoadAssetAsync<TextAsset>("LanguagePack");
                var textAsset = await op.Task;
                return manager.ReadLanguagePackZip(GetKey(), textAsset.bytes);
            }
        }
        private class ExternalLanguagePackReference : LanguagePackReference
        {
            public string path;
            public bool isDirectory;

            public ExternalLanguagePackReference(string path, bool isDirectory)
            {
                this.path = path;
                this.isDirectory = isDirectory;
            }

            public override bool Equals(object obj)
            {
                return obj is ExternalLanguagePackReference other &&
                    path == other.path &&
                    isDirectory == other.isDirectory;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(path, isDirectory);
            }
            public override string GetKey()
            {
                return Path.GetFileName(path);
            }
            public override Task<LanguagePackMetadata> LoadMetadata(LanguageManager manager)
            {
                if (isDirectory)
                {
                    return Task.FromResult(manager.ReadLanguagePackMetadataDirectory(path));
                }
                return Task.FromResult(manager.ReadLanguagePackMetadataZip(path));
            }
            public override Task<LanguagePack> LoadLanguagePack(LanguageManager manager)
            {
                if (isDirectory)
                {
                    return Task.FromResult(manager.ReadLanguagePackDirectory(path));
                }
                return Task.FromResult(manager.ReadLanguagePackZip(path));
            }
        }
    }
    public abstract class LanguagePackReference
    {
        public virtual bool IsBuiltin => false;
        public abstract Task<LanguagePackMetadata> LoadMetadata(LanguageManager manager);
        public abstract Task<LanguagePack> LoadLanguagePack(LanguageManager manager);
        public abstract string GetKey();
        public override string ToString()
        {
            return GetKey();
        }
    }
}
