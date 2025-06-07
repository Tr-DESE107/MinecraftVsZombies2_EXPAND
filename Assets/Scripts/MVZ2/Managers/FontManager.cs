﻿using System.Collections.Generic;
using System.Linq;
using MVZ2Logic;
using PVZEngine;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;

namespace MVZ2.Managers
{
    public class FontManager : MonoBehaviour
    {
        public void Init()
        {
            //TMP_Text.OnSpriteAssetRequest += OnSpriteAssetRequestCallback;
        }
        public void InitFontSprites()
        {
        }
        #region 自动生成字体贴图，暂时弃用
        private TMP_SpriteAsset OnSpriteAssetRequestCallback(int hashCode, string assetName)
        {
            return GetSpriteAsset(assetName);
        }
        private void InitAlmanacTagIcons()
        {
            HashSet<SpriteReference> spriteList = new HashSet<SpriteReference>();
            HashSet<SpriteReference> backgroundList = new HashSet<SpriteReference>();
            var almanacTags = Main.ResourceManager.GetAllAlmanacTagMetas();
            foreach (var tagMeta in almanacTags)
            {
                spriteList.Add(tagMeta.iconSprite);
                backgroundList.Add(tagMeta.backgroundSprite);
            }
            var enumMetas = Main.ResourceManager.GetAllAlmanacTagEnumMetas();
            foreach (var enumMeta in enumMetas)
            {
                foreach (var valueMeta in enumMeta.values)
                {
                    spriteList.Add(valueMeta.iconSprite);
                }
            }
            var iconGroups = spriteList.Where(s => SpriteReference.IsValid(s)).GroupBy(s => s.id);
            foreach (var group in iconGroups)
            {
                var id = group.Key;
                var sprites = group.Select(r => Main.GetFinalSprite(r)).Where(s => s).ToArray();
                if (sprites.Length <= 0)
                    continue;
                var texture = sprites.FirstOrDefault().texture;
                GenerateSpritesForTexture(id, texture, sprites, fontMaterial, false);
            }
            var backgroundGroups = backgroundList.Where(s => SpriteReference.IsValid(s)).GroupBy(s => s.id);
            foreach (var group in backgroundGroups)
            {
                var id = group.Key;
                var sprites = group.Select(r => Main.GetFinalSprite(r)).Where(s => s).ToArray();
                if (sprites.Length <= 0)
                    continue;
                var texture = sprites.FirstOrDefault().texture;
                GenerateSpritesForTexture(id, texture, sprites, backgroundIconMaterial, true);
            }
        }
        private void GenerateSpritesForTexture(NamespaceID id, Texture2D texture2D, Sprite[] sprites, Material material, bool takesPlace)
        {
            var asset = GetOrCreateSpriteAsset(id, texture2D, material);
            for (int i = 0; i < sprites.Length; i++)
            {
                var sprite = sprites[i];
                GenerateSpriteGlyph(asset, i, sprite, takesPlace);
            }
            asset.UpdateLookupTables();
        }
        private void GenerateSpriteGlyph(TMP_SpriteAsset asset, int index, Sprite sprite, bool takesPlace)
        {
            TMP_SpriteGlyph spriteGlyph = new TMP_SpriteGlyph();
            spriteGlyph.index = (uint)index;
            spriteGlyph.metrics = new GlyphMetrics(sprite.rect.width, sprite.rect.height, 0, sprite.rect.height - 2, takesPlace ? sprite.rect.width : 0);
            spriteGlyph.glyphRect = new GlyphRect(sprite.rect);
            spriteGlyph.scale = 1.0f;
            spriteGlyph.sprite = sprite;

            asset.spriteGlyphTable.Add(spriteGlyph);

            TMP_SpriteCharacter spriteCharacter = new TMP_SpriteCharacter(0x0001, spriteGlyph);
            spriteCharacter.name = sprite.name;
            spriteCharacter.scale = 1.0f;

            asset.spriteCharacterTable.Add(spriteCharacter);
        }
        private TMP_SpriteAsset GetOrCreateSpriteAsset(NamespaceID assetName, Texture2D texture, Material material)
        {
            if (spriteAssetDict.TryGetValue(assetName, out var asset))
            {
                return asset;
            }
            asset = CreateSpriteAsset();
            asset.name = assetName.ToString();
            asset.spriteSheet = texture;

            var materialClone = new Material(material);
            materialClone.SetTexture(ShaderUtilities.ID_MainTex, asset.spriteSheet);
            material.hideFlags = HideFlags.HideInHierarchy;

            asset.material = materialClone;
            asset.UpdateLookupTables();
            spriteAssetDict.Add(assetName, asset);
            return asset;
        }
        private TMP_SpriteAsset GetSpriteAsset(string name)
        {
            if (NamespaceID.TryParse(name, Main.BuiltinNamespace, out var id))
            {
                return spriteAssetDict.TryGetValue(id, out var asset) ? asset : null;
            }
            return null;
        }
        private TMP_SpriteAsset CreateSpriteAsset()
        {
            TMP_SpriteAsset spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            spriteAsset.hashCode = TMP_TextUtilities.GetSimpleHashCode(spriteAsset.name);
            spriteAsset.spriteInfoList = new List<TMP_Sprite>();
            return spriteAsset;
        }

        private Dictionary<NamespaceID, TMP_SpriteAsset> spriteAssetDict = new Dictionary<NamespaceID, TMP_SpriteAsset>();
        [SerializeField]
        private Material fontMaterial;
        [SerializeField]
        private Material backgroundIconMaterial;
        #endregion

        public MainManager Main => MainManager.Instance;
    }
}
