using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Metas;
using MVZ2.Talk;
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
            var modResource = main.ResourceManager.GetModResource(characterID.SpaceName);
            if (modResource == null)
                return null;
            return modResource.TalkCharacterMetaList.metas.FirstOrDefault(m => m.id == characterID.Path);
        }
        #endregion

        public string GetCharacterName(NamespaceID characterID)
        {
            var character = GetCharacterMeta(characterID);
            return GetCharacterName(character?.name);
        }
        public string GetCharacterName(string nameKey)
        {
            if (nameKey == null)
                return string.Empty;
            return main.LanguageManager._p(VanillaStrings.CONTEXT_CHARACTER_NAME, nameKey);
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
        public TalkCharacterViewData GetCharacterViewData(NamespaceID characterID, NamespaceID variantID, CharacterSide side)
        {
            Sprite sprite;
            Vector2 widthExtend = Vector2.zero;
            if (!NamespaceID.IsValid(variantID))
            {
                sprite = Main.ResourceManager.GetCharacterSprite(characterID);
                var meta = Main.ResourceManager.GetCharacterMeta(characterID);
                var variantMeta = meta.variants.FirstOrDefault();
                if (variantMeta != null)
                {
                    widthExtend = variantMeta.widthExtend;
                }
            }
            else
            {
                sprite = Main.ResourceManager.GetCharacterSprite(characterID, variantID);
                var meta = Main.ResourceManager.GetCharacterMeta(characterID);
                var variantMeta = meta.variants.FirstOrDefault(v => v.id == variantID);
                if (variantMeta != null)
                {
                    widthExtend = variantMeta.widthExtend;
                }
            }
            return new TalkCharacterViewData()
            {
                name = characterID?.ToString(),
                side = side,
                sprite = sprite,
                widthExtend = widthExtend
            };
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
            imageTexture.name = $"{character}({variantID})";

            // 清除颜色。
            var bufferWidth = 16;
            var bufferHeight = 16;
            var colorBuffer = new Color[bufferWidth * bufferHeight];
            Array.Fill(colorBuffer, Color.clear);
            for (int x = 0; x < width; x += bufferWidth)
            {
                var w = Mathf.Min(bufferWidth, width - x);
                for (int y = 0; y < height; y += bufferHeight)
                {
                    var h = Mathf.Min(bufferHeight, height - y);
                    imageTexture.SetPixels(x, y, w, h, colorBuffer);
                }
            }

            // 添加颜色。
            foreach (TalkCharacterLayer layer in variantInfo.layers)
            {
                Sprite sourceSpr = GetSprite(layer.sprite);
                Texture2D sourceTex = sourceSpr.texture;
                Rect sourceRect = sourceSpr.rect;
                var layerX = (int)sourceRect.xMin;
                var layerY = (int)sourceRect.yMin;
                var layerWidth = (int)sourceRect.width;
                var layerHeight = (int)sourceRect.height;

                var layerOffsetX = layer.positionX;
                var layerOffsetY = height - (layer.positionY + layerHeight);
                for (int x = 0; x < layerWidth; x += bufferWidth)
                {
                    var w = Mathf.Min(bufferWidth, layerWidth - x);
                    for (int y = 0; y < layerHeight; y += bufferHeight)
                    {
                        var h = Mathf.Min(bufferHeight, layerHeight - y);
                        var colors = sourceTex.GetPixels(x + layerX, y + layerY, w, h);
                        imageTexture.SetPixels(x + layerOffsetX, y + layerOffsetY, w, h, colors);
                    }
                }
            }

            imageTexture.Apply();

            Sprite spr = CreateSprite(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), spritePivot, imageTexture.name, "character");
            return spr;
        }
        #region 私有方法
        private async Task LoadCharacterVariantSprites(string nsp, TaskProgress progress, int maxYieldCount = 1)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return;
            var metaList = GetCharacterMetaList(nsp);
            if (metaList == null)
                return;
            var metas = metaList.metas;
            int count = metas.Count;
            var childProgresses = progress.AddChildren(count);
            for (int i = 0; i < count; i++)
            {
                var meta = metas[i];

                progress.SetCurrentTaskName($"Character {meta.id}");
                var sprites = await GenerateCharacterVariantSprites(nsp, meta, childProgresses[i], maxYieldCount);
                modResource.CharacterVariantSprites.Add(meta.id, sprites.ToArray());
                progress.SetProgress(i / (float)count);
            }
            progress.SetProgress(1, "Finished");
        }
        private async Task<CharacterVariantSprite[]> GenerateCharacterVariantSprites(string nsp, TalkCharacterMeta meta, TaskProgress progress, int maxYieldCount = 1)
        {
            var characterID = new NamespaceID(nsp, meta.id);
            List<CharacterVariantSprite> sprites = new List<CharacterVariantSprite>();
            var count = meta.variants.Count;
            int yieldCounter = 0;
            for (int i = 0; i < count; i++)
            {
                var variant = meta.variants[i];
                var spr = GenerateCharacterVariantSprite(characterID, variant.id);
                var cvs = new CharacterVariantSprite()
                {
                    variant = variant.id,
                    sprite = spr
                };
                sprites.Add(cvs);
                progress.SetProgress(i / (float)count, $"Variant {variant.id}");
                yieldCounter++;
                if (yieldCounter >= maxYieldCount)
                {
                    yieldCounter = 0;
                    await Task.Yield();
                }
            }
            progress.SetProgress(1, "Finished");
            return sprites.ToArray();
        }
        #endregion
    }
}
