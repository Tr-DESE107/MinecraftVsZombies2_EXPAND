using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Metas;
using MVZ2.TalkData;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public TalkCharacterMetaList GetCharacterMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.TalkCharacterMetaList;
        }
        #endregion

        #region 元数据
        public TalkCharacterMeta GetCharacterMeta(NamespaceID characterID)
        {
            var modResource = main.ResourceManager.GetModResource(characterID.spacename);
            if (modResource == null)
                return null;
            return modResource.TalkCharacterMetaList.metas.FirstOrDefault(m => m.id == characterID.path);
        }
        #endregion

        public string GetCharacterName(NamespaceID characterID)
        {
            var character = GetCharacterMeta(characterID);
            if (character?.name == null)
                return string.Empty;
            return main.LanguageManager._p(VanillaStrings.CONTEXT_CHARACTER_NAME, character.name);
        }
        public Sprite GetCharacterSprite(NamespaceID characterID)
        {
            var variants = FindInMods(characterID, mr => mr.CharacterVariantSprites);
            if (variants == null)
                return null;
            var variant = variants.FirstOrDefault();
            if (variant == null)
                return null;
            return variant.sprite;
        }
        public Sprite GetCharacterSprite(NamespaceID characterID, NamespaceID variantID)
        {
            var variants = FindInMods(characterID, mr => mr.CharacterVariantSprites);
            if (variants == null)
                return null;
            var variant = variants.FirstOrDefault(v => v.variant == variantID);
            if (variant == null)
                return null;
            return variant.sprite;
        }
        public Sprite GenerateCharacterVariantSprite(NamespaceID character, NamespaceID variantID)
        {
            TalkCharacterMeta info = GetCharacterMeta(character);
            TalkCharacterVariant variantInfo = info.GetVariant(variantID);

            // Variables
            Vector2 spritePivot = new Vector2(variantInfo.pivotX, variantInfo.pivotY);

            var width = variantInfo.width;
            var height = variantInfo.height;
            var imageTexture = new Texture2D(width, height);
            var emptyColors = new Color32[width * height];
            Array.Fill(emptyColors, new Color32(0, 0, 0, 0));
            imageTexture.SetPixels32(emptyColors);
            imageTexture.name = $"{character}({variantID})";
            foreach (TalkCharacterLayer layer in variantInfo.layers)
            {
                Sprite sourceSpr = GetSprite(layer.sprite);
                Texture2D sourceTex = sourceSpr.texture;
                Rect sourceRect = sourceSpr.rect;
                var layerWidth = (int)sourceRect.width;
                var layerHeight = (int)sourceRect.height;
                var colors = sourceTex.GetPixels((int)sourceRect.xMin, (int)sourceRect.yMin, layerWidth, layerHeight);

                imageTexture.SetPixels(layer.positionX, height - layer.positionY - layerHeight, layerWidth, layerHeight, colors);
            }

            imageTexture.Apply();

            Sprite spr = CreateSprite(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), spritePivot, imageTexture.name, "character");
            return spr;
        }
        #region 私有方法
        private void LoadCharacterVariantSprites(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var metaList = GetCharacterMetaList(nsp);
            if (metaList == null)
                return;
            foreach (var meta in metaList.metas)
            {
                var characterID = new NamespaceID(nsp, meta.id);
                List<CharacterVariantSprite> sprites = new List<CharacterVariantSprite>();
                foreach (var variant in meta.variants)
                {
                    var spr = GenerateCharacterVariantSprite(characterID, variant.id);
                    var cvs = new CharacterVariantSprite()
                    {
                        variant = variant.id,
                        sprite = spr
                    };
                    sprites.Add(cvs);
                }
                modResource.CharacterVariantSprites.Add(meta.id, sprites.ToArray());
            }
        }
        #endregion
    }
}
