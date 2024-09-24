using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Localization;
using MVZ2.Resources;
using Newtonsoft.Json;
using PVZEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MVZ2.Managers
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
                {
                    continue;
                }
                var fullPath = entry.FullName.Replace("\\", "/");
                var splitedPaths = fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (splitedPaths.Length >= 1 && splitedPaths[0] == "assets")
                {
                    if (splitedPaths.Length >= 2)
                    {
                        var nsp = splitedPaths[1];
                        if (splitedPaths.Length >= 3)
                        {
                            var lang = splitedPaths[2].Replace('_', '-');
                            if (splitedPaths.Length == 4)
                            {
                                string filename = splitedPaths[3];
                                if (Path.GetExtension(filename) == ".mo")
                                {
                                    var filenameWithoutExt = Path.GetFileNameWithoutExtension(filename);
                                    var asset = GetOrCreateLanguageAsset(assets, lang);
                                    var catalog = entry.ReadCatalog(lang);
                                    asset.catalogs.Add(filenameWithoutExt, catalog);
                                }
                            }
                            else if (splitedPaths.Length >= 5)
                            {
                                string type = splitedPaths[3];
                                var asset = GetOrCreateLanguageAsset(assets, lang);
                                var rootPath = Path.Combine(splitedPaths[0], splitedPaths[1], splitedPaths[2], splitedPaths[3]);
                                var relativePath = Path.GetRelativePath(rootPath, fullPath);
                                var entryName = Path.ChangeExtension(relativePath, string.Empty).TrimEnd('.').Replace("\\", "/");

                                var resID = new NamespaceID(nsp, entryName);
                                switch (type)
                                {
                                    case "sprites":
                                        var sprite = ReadEntryToSprite(resID, entry);
                                        asset.Sprites.Add(resID, sprite);
                                        break;
                                    case "spritesheets":
                                        var spritesheet = ReadEntryToSpriteSheet(resID, entry);
                                        asset.SpriteSheets.Add(resID, spritesheet);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return new LanguagePack(metadata, assets);
        }
        private Sprite ReadEntryToSprite(NamespaceID spriteId, ZipArchiveEntry entry)
        {
            try
            {
                var originalSprite = Main.ResourceManager.GetSprite(spriteId);
                var texture2D = entry.ReadTexture2D();
                Rect spriteRect;
                Vector2 spritePivot;
                if (originalSprite)
                {
                    spriteRect = originalSprite.rect;
                    spritePivot = originalSprite.pivot / spriteRect.size;
                }
                else
                {
                    spriteRect = new Rect(0, 0, texture2D.width, texture2D.height);
                    spritePivot = Vector2.one * 0.5f;
                }
                var spr = Sprite.Create(texture2D, spriteRect, spritePivot);
                spr.name = spriteId.ToString();
                return spr;
            }
            catch (Exception e)
            {
                Debug.LogError($"An exception thrown when loading sprite {spriteId} from a language pack: {e}");
                return Main.ResourceManager.GetDefaultSpriteClone();
            }
        }
        private Sprite[] ReadEntryToSpriteSheet(NamespaceID spriteId, ZipArchiveEntry entry)
        {
            var originalSpriteSheet = Main.ResourceManager.GetSpriteSheet(spriteId);
            try
            {
                var texture2D = entry.ReadTexture2D();
                (Rect rect, Vector2 pivot)[] spriteInfos;
                if (originalSpriteSheet != null)
                {
                    spriteInfos = new (Rect rect, Vector2 pivot)[originalSpriteSheet.Length];
                    for (int i = 0; i < spriteInfos.Length; i++)
                    {
                        var originalSprite = originalSpriteSheet[i];
                        spriteInfos[i] = (originalSprite.rect, originalSprite.pivot);
                    }
                }
                else
                {
                    spriteInfos = new (Rect rect, Vector2 pivot)[1]
                    {
                        (new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one * 0.5f)
                    };
                }
                var sprites = new Sprite[spriteInfos.Length];
                for (int i = 0; i < sprites.Length; i++)
                {
                    var info = spriteInfos[i];
                    var rect = info.rect;
                    rect.width = Math.Min(rect.width, texture2D.width);
                    rect.height = Math.Min(rect.height, texture2D.height);
                    var spr = Sprite.Create(texture2D, rect, info.pivot / rect.size);
                    spr.name = $"{spriteId}[{i}]";
                    sprites[i] = spr;
                }
                return sprites;
            }
            catch (Exception e)
            {
                Debug.LogError($"An exception thrown when loading spritesheet {spriteId} from a language pack: {e}");
                var length = originalSpriteSheet?.Length ?? 1;
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
        private List<LanguagePack> languagePacks = new List<LanguagePack>();
    }
}
