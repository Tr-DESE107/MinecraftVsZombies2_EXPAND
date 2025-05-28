﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using NGettext;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Localization
{
    public class LanguagePack
    {
        public LanguagePack(string key)
        {
            Key = key;
        }
        public LanguageAssets GetOrCreateLanguageAsset(string lang)
        {
            var asset = assets.FirstOrDefault(a => a.language == lang);
            if (asset == null)
            {
                asset = new LanguageAssets(lang);
                assets.Add(asset);
            }
            return asset;
        }
        public LanguageAssets GetLanguageAssets(string lang)
        {
            return assets.FirstOrDefault(a => a.language == lang);
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
        public bool TryGetStringPlural(string language, string text, string textPlural, long n, out string result, params object[] args)
        {
            result = null;
            var asset = assets.FirstOrDefault(a => a.language == language);
            if (asset == null)
                return false;
            foreach (var catalog in asset.catalogs.Values)
            {
                if (catalog.IsTranslationExist(text))
                {
                    result = catalog.GetPluralString(text, textPlural, n, args);
                    return true;
                }
            }
            return false;
        }
        public bool TryGetStringParticularPlural(string language, string context, string text, string textPlural, long n, out string result, params object[] args)
        {
            result = null;
            var asset = assets.FirstOrDefault(a => a.language == language);
            if (asset == null)
                return false;
            foreach (var catalog in asset.catalogs.Values)
            {
                if (catalog.IsTranslationExist(context + "\u0004" + text))
                {
                    result = catalog.GetParticularPluralString(context, text, textPlural, n, args);
                    return true;
                }
            }
            return false;
        }
        public bool TryGetSprite(string language, NamespaceID id, out Sprite spr)
        {
            spr = null;
            foreach (var asset in assets)
            {
                if (asset.language != language)
                    continue;
                return asset.Sprites.TryGetValue(id, out spr);
            }
            return false;
        }
        public bool TryGetSpriteSheet(string language, NamespaceID id, out Sprite[] res)
        {
            res = null;
            foreach (var asset in assets)
            {
                if (asset.language != language)
                    continue;
                return asset.SpriteSheets.TryGetValue(id, out res);
            }
            return false;
        }
        public string[] GetLanguages()
        {
            return assets.Select(a => a.language).ToArray();
        }
        private List<LanguageAssets> assets = new List<LanguageAssets>();
        public string Key { get; private set; }
    }
    public class LanguagePackMetadata
    {
        public string name;
        public string author;
        public string description;
        public int dataVersion;
        [JsonIgnore]
        public Sprite icon;
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
