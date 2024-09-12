using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using MVZ2.Localization;
using Newtonsoft.Json;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public delegate bool TryGetTranslation<in TKey, TResult>(LanguagePack pack, TKey key, out TResult result);
    public partial class LanguageManager
    {
        public void LoadAllLanguagePacks()
        {
            var directory = GetLanguagePackDirectory();
            var packs = Directory.GetFiles(directory, "*.pack", SearchOption.AllDirectories);
            foreach (var pack in packs)
            {
                var loaded = ReadLanguagePack(pack);
                if (loaded == null)
                    continue;
                languagePacks.Add(loaded);
                foreach (var lang in loaded.GetLanguages())
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
        public static string GetLanguagePackDirectory()
        {
            if (Application.isEditor)
            {
                var path = Application.dataPath;
                path = Path.GetDirectoryName(path);
                return Path.Combine(path, "ExternalData", "languages");
            }
            return Path.Combine(Application.streamingAssetsPath, "languages");
        }
        public static void CompressLanguagePack(string sourceDirectory, string destPath)
        {
            var sourceDirInfo = new DirectoryInfo(sourceDirectory);
            var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
            using var stream = File.Open(destPath, FileMode.Create);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            foreach (var filePath in files)
            {
                var entryName = Path.GetRelativePath(sourceDirectory, filePath);
                var entry = archive.CreateEntryFromFile(filePath, entryName);
            }
        }
        public LanguagePack ReadLanguagePack(string path)
        {
            using var stream = File.Open(path, FileMode.Open);
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
            var originalSprite = Main.ResourceManager.GetSprite(spriteId);
            var texture2D = entry.ReadTexture2D();
            Rect spriteRect;
            Vector2 spritePivot;
            if (originalSprite)
            {
                spriteRect = originalSprite.rect;
                spritePivot = originalSprite.pivot;
            }
            else
            {
                spriteRect = new Rect(0, 0, texture2D.width, texture2D.height);
                spritePivot = Vector2.one * 0.5f;
            }
            return Sprite.Create(texture2D, spriteRect, spritePivot);
        }
        private Sprite[] ReadEntryToSpriteSheet(NamespaceID spriteId, ZipArchiveEntry entry)
        {
            var originalSpriteSheet = Main.ResourceManager.GetSpriteSheet(spriteId);
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
                spr.name = spriteId.ToString();
                sprites[i] = spr;
            }
            return sprites;
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
