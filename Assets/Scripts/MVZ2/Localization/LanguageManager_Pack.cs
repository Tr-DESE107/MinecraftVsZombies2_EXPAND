using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.IO;
using Newtonsoft.Json;
using PVZEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVZ2.Localization
{
    public delegate bool TryGetTranslation<in TKey, TResult>(LanguagePack pack, TKey key, out TResult result);
    public partial class LanguageManager
    {
        public Task LoadAllLanguagePacks()
        {
            var op = Addressables.LoadAssetsAsync<TextAsset>("LanguagePack", textAsset =>
            {
                var bytes = textAsset.bytes;
                var loaded = ReadLanguagePack(bytes);
                if (loaded == null)
                    return;
                languagePacks.Add(loaded);
                foreach (var lang in loaded.GetLanguages())
                {
                    if (!allLanguages.Contains(lang))
                    {
                        allLanguages.Add(lang);
                    }
                }
            });

            // TODO：加载外部语言包。
            return op.Task;
        }
        public LanguagePack[] GetAllLanguagePacks()
        {
            return languagePacks.ToArray();
        }
        public LanguagePack ReadLanguagePack(string path)
        {
            using var stream = File.Open(path, FileMode.Open);
            return ReadLanguagePack(stream);
        }
        public LanguagePack ReadLanguagePack(byte[] bytes)
        {
            using var memory = new MemoryStream(bytes);
            return ReadLanguagePack(memory);
        }
        public LanguagePack ReadLanguagePack(Stream stream)
        {
            using var archive = new ZipArchive(stream);

            var metadataEntry = archive.GetEntry(metadataName);
            if (metadataEntry == null)
                return null;

            var json = metadataEntry.ReadString(Encoding.UTF8);
            LanguagePackMetadata metadata = JsonConvert.DeserializeObject<LanguagePackMetadata>(json);
            List<LanguageAssets> assets = new();
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
                    var asset = GetOrCreateLanguageAsset(assets, lang);
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
            return new LanguagePack(metadata, assets);
        }
        private Sprite ReadEntryToSprite(NamespaceID spriteId, ZipArchiveEntry entry, LocalizedSprite meta)
        {
            try
            {
                var texture2D = entry.ReadTexture2D();
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
        private Sprite[] ReadEntryToSpriteSheet(NamespaceID spriteId, ZipArchiveEntry entry, LocalizedSpriteSheet meta)
        {
            try
            {
                var texture2D = entry.ReadTexture2D();
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
        private LanguageAssets GetOrCreateLanguageAsset(List<LanguageAssets> assets, string lang)
        {
            var asset = assets.FirstOrDefault(a => a.language == lang);
            if (asset == null)
            {
                asset = new LanguageAssets(lang);
                assets.Add(asset);
            }
            return asset;
        }
        private static void SplitTypeLang(string str, out string type, out string lang)
        {
            var dotIndex = str.IndexOf('.');
            if (dotIndex < 0)
            {
                type = str;
                lang = string.Empty;
                return;
            }
            type = str.Substring(0, dotIndex);
            lang = str.Substring(dotIndex + 1);
        }
        public const string metadataName = "pack.json";
        public const string SPRITE_MANIFEST_FILENAME = "sprite_manifest.json";
        private List<LanguagePack> languagePacks = new List<LanguagePack>();
        private List<LanguagePack> enabledLanguagePacks = new List<LanguagePack>();
    }
}
