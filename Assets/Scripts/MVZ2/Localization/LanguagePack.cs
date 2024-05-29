using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NGettext;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Localization
{
    public class LanguagePack
    {
        public LanguagePackMetadata Metadata { get; }
        private List<LanguageAssets> assets = new List<LanguageAssets>();
        public const string metadataName = "pack.json";

        public bool TryGetSprite(string language, NamespaceID id, out Sprite spr)
        {
            spr = null;
            var asset = assets.FirstOrDefault(a => a.language == language);
            if (asset == null)
                return false;
            return asset.Sprites.TryGetValue(id, out spr);
        }
        public string[] GetLanguages()
        {
            return assets.Select(a => a.language).ToArray();
        }
        private LanguagePack(LanguagePackMetadata metadata, IEnumerable<LanguageAssets> assets)
        {
            Metadata = metadata;
            this.assets.AddRange(assets);
        }
        public static void Compress(string sourceDirectory, string destPath)
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
        public static LanguagePack Read(string path)
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
                var splitedPaths = entry.FullName.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (splitedPaths.Length >= 1 && splitedPaths[0] == "assets")
                {
                    if (splitedPaths.Length >= 2)
                    {
                        var nsp = splitedPaths[1];
                        if (splitedPaths.Length >= 3)
                        {
                            var typeLang = splitedPaths[2];
                            SplitTypeLang(typeLang, out var type, out var lang);

                            var asset = GetOrCreateLanguageAsset(assets, lang);
                            var entryName = Path.ChangeExtension(entry.Name, string.Empty).TrimEnd('.');
                            asset.AddAsset(type, new NamespaceID(nsp, entryName), entry);
                        }
                        else
                        {
                            var fileName = splitedPaths[2];
                            if (Path.GetExtension(fileName) == ".mo")
                            {
                                var lang = Path.GetFileNameWithoutExtension(fileName);
                                var asset = GetOrCreateLanguageAsset(assets, lang);
                                asset.AddCatalog(entry);
                            }
                        }
                    }
                }
            }
            return new LanguagePack(metadata, assets);

            LanguageAssets GetOrCreateLanguageAsset(List<LanguageAssets> assets, string lang)
            {
                var asset = assets.FirstOrDefault(a => a.language == lang);
                if (asset == null)
                {
                    asset = new LanguageAssets(lang);
                    assets.Add(asset);
                }
                return asset;
            }
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
    }
    public class LanguagePackMetadata
    {
        public string name;
        public string author;
        public string description;
        public LanguagePackVersion version;
    }
    public class LanguagePackVersion
    {
        public int major;
        public int minor;
    }
    public class LanguageAssets
    {
        public string language;
        public Catalog catalog;
        public Dictionary<NamespaceID, Sprite> Sprites = new Dictionary<NamespaceID, Sprite>();

        public LanguageAssets(string lang)
        {
            language = lang;
        }
        public void AddCatalog(ZipArchiveEntry entry)
        {
            using var stream = entry.Open();
            catalog = new Catalog(stream, new CultureInfo(language));
        }
        public void AddAsset(string type, NamespaceID id, ZipArchiveEntry entry)
        {
            switch (type)
            {
                case "textures":
                    using (var entryStream = entry.Open())
                    {
                        using (var memory = new MemoryStream())
                        {
                            entryStream.CopyTo(memory);

                            Texture2D texture = new Texture2D(2, 2);
                            texture.LoadImage(memory.ToArray());

                            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                            Sprites.Add(id, sprite);
                        }
                    }
                    break;
            }
        }
    }
}
