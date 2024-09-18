using System.Collections.Generic;
using System.Linq;
using NGettext;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Localization
{
    public class LanguagePack
    {
        public LanguagePack(LanguagePackMetadata metadata, IEnumerable<LanguageAssets> assets)
        {
            Metadata = metadata;
            this.assets.AddRange(assets);
        }
        public bool TryGetString(string language, string text, out string result, params object[] args)
        {
            result = null;
            var asset = assets.FirstOrDefault(a => a.language == language);
            if (asset == null)
                return false;
            foreach (var catalog in asset.catalogs.Values)
            {
                if (catalog.IsTranslationExist(text))
                {
                    result = catalog.GetString(text, args);
                    return true;
                }
            }
            return false;
        }
        public bool TryGetStringParticular(string language, string context, string text, out string result, params object[] args)
        {
            result = null;
            var asset = assets.FirstOrDefault(a => a.language == language);
            if (asset == null)
                return false;
            foreach (var catalog in asset.catalogs.Values)
            {
                if (catalog.IsTranslationExist(context + "\u0004" + text))
                {
                    result = catalog.GetParticularString(context, text, args);
                    return true;
                }
            }
            return false;
        }
        public bool TryGetSprite(string language, NamespaceID id, out Sprite spr)
        {
            spr = null;
            var asset = assets.FirstOrDefault(a => a.language == language);
            if (asset == null)
                return false;
            return asset.Sprites.TryGetValue(id, out spr);
        }
        public bool TryGetSpriteSheet(string language, NamespaceID id, out Sprite[] res)
        {
            res = null;
            var asset = assets.FirstOrDefault(a => a.language == language);
            if (asset == null)
                return false;
            return asset.SpriteSheets.TryGetValue(id, out res);
        }
        public string[] GetLanguages()
        {
            return assets.Select(a => a.language).ToArray();
        }
        public LanguagePackMetadata Metadata { get; }
        private List<LanguageAssets> assets = new List<LanguageAssets>();
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
        public Dictionary<string, Catalog> catalogs = new Dictionary<string, Catalog>();
        public Dictionary<NamespaceID, Sprite> Sprites = new Dictionary<NamespaceID, Sprite>();
        public Dictionary<NamespaceID, Sprite[]> SpriteSheets = new Dictionary<NamespaceID, Sprite[]>();
        public LanguageAssets(string lang)
        {
            language = lang;
        }
    }
}
