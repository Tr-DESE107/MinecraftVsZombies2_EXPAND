using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Metas;
using MVZ2.Sprites;
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
        #region 私有方法
        private IEnumerator LoadCharacterVariantSprites(string nsp, TaskProgress progress, int maxYieldCount = 1)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                yield break;
            var metaList = GetCharacterMetaList(nsp);
            if (metaList == null)
                yield break;
            var metas = metaList.metas;
            int count = metas.Count;
            var childProgresses = progress.AddChildren(count);

            characterTextureDict.Clear();
            var sprites = new List<CharacterVariantSprite>();
            for (int i = 0; i < count; i++)
            {
                var meta = metas[i];

                progress.SetCurrentTaskName($"Character {meta.id}");
                sprites.Clear();
                yield return GenerateCharacterVariantSprites(nsp, meta, childProgresses[i], sprites, maxYieldCount);
                modResource.CharacterVariantSprites.Add(meta.id, sprites.ToArray());
                progress.SetProgress(i / (float)count);
            }
            progress.SetProgress(1, "Finished");

            characterTextureDict.Clear();
        }
        private IEnumerator GenerateCharacterVariantSprites(string nsp, TalkCharacterMeta meta, TaskProgress progress, List<CharacterVariantSprite> list, int maxYieldCount = 1)
        {
            var characterID = new NamespaceID(nsp, meta.id);
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
                list.Add(cvs);
                progress.SetProgress(i / (float)count, $"Variant {variant.id}");
                yieldCounter++;
                if (yieldCounter >= maxYieldCount)
                {
                    yieldCounter = 0;
                    yield return null;
                }
            }
            progress.SetProgress(1, "Finished");
        }
        private Sprite GenerateCharacterVariantSprite(NamespaceID character, NamespaceID variantID)
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
            Array.Fill(spriteColorBuffer, new Color32(0, 0, 0, 0));
            for (int x = 0; x < width; x += COLOR_BUFFER_WIDTH)
            {
                var w = Mathf.Min(COLOR_BUFFER_WIDTH, width - x);
                for (int y = 0; y < height; y += COLOR_BUFFER_HEIGHT)
                {
                    var h = Mathf.Min(COLOR_BUFFER_HEIGHT, height - y);
                    imageTexture.SetPixels32(x, y, w, h, spriteColorBuffer);
                }
            }

            // 添加颜色。
            foreach (TalkCharacterLayer layer in variantInfo.layers)
            {
                Sprite sourceSpr = GetSprite(layer.sprite);
                Texture2D sourceTex = sourceSpr.texture;
                Rect sourceRect = sourceSpr.rect;
                var sourceWidth = sourceTex.width;
                var layerX = (int)sourceRect.xMin;
                var layerY = (int)sourceRect.yMin;
                var layerWidth = (int)sourceRect.width;
                var layerHeight = (int)sourceRect.height;

                var layerOffsetX = layer.positionX;
                var layerOffsetY = height - (layer.positionY + layerHeight);
                if (!characterTextureDict.TryGetValue(sourceTex, out var fullColors))
                {
                    fullColors = sourceTex.GetPixels32();
                    characterTextureDict.Add(sourceTex, fullColors);
                }
                for (int x = 0; x < layerWidth; x += COLOR_BUFFER_WIDTH)
                {
                    var w = Mathf.Min(COLOR_BUFFER_WIDTH, layerWidth - x);
                    for (int y = 0; y < layerHeight; y += COLOR_BUFFER_HEIGHT)
                    {
                        var h = Mathf.Min(COLOR_BUFFER_HEIGHT, layerHeight - y);
                        var srcX = layerX + x;
                        var srcY = layerY + y;
                        var dstX = layerOffsetX + x;
                        var dstY = layerOffsetY + y;
                        fullColors.GetPixels32(sourceWidth, srcX, srcY, w, h, spriteColorBuffer);
                        imageTexture.SetPixels32(dstX, dstY, w, h, spriteColorBuffer);
                    }
                }
            }

            imageTexture.Apply();

            Sprite spr = CreateSprite(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), spritePivot, imageTexture.name, "character");
            return spr;
        }
        #endregion
        private Dictionary<Texture2D, Color32[]> characterTextureDict = new Dictionary<Texture2D, Color32[]>();
    }
}
