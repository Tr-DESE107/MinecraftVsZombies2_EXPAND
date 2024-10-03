using System.Collections.Generic;
using System.Linq;
using MVZ2.Talk;
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
            return modResource.TalkCharacterMetaList.metas.FirstOrDefault(m => m.name == characterID.path);
        }
        #endregion

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
            var layers = new List<TalkCharacterLayer>();
            GetCharacterVariantProperties(info, variantInfo, layers, out float pivotX, out float pivotY, out int width, out int height);
            Vector2 spritePivot = new Vector2(pivotX, pivotY);

            var imageTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            imageTexture.name = $"{character}({variantID})";
            foreach (TalkCharacterLayer layer in layers)
            {
                Sprite sourceSpr = GetSprite(layer.sprite);
                Texture2D sourceTex = sourceSpr.texture;
                Rect sourceRect = sourceSpr.rect;
                var layerWidth = (int)sourceRect.width;
                var layerHeight = (int)sourceRect.height;
                var colors = sourceTex.GetPixels((int)sourceRect.xMin, (int)sourceRect.yMin, layerWidth, layerHeight);

                imageTexture.SetPixels(layer.positionX, layer.positionY, layerWidth, layerHeight, colors);
            }

            imageTexture.Apply();

            Sprite spr = CreateSprite(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), spritePivot, imageTexture.name, "character");
            return spr;
        }
        #region 私有方法
        private void GetCharacterVariantProperties(TalkCharacterMeta info, TalkCharacterVariant variantInfo, List<TalkCharacterLayer> layers, out float pivotX, out float pivotY, out int width, out int height)
        {
            var parentID = variantInfo.parent;
            if (NamespaceID.IsValid(parentID))
            {
                var parent = info.GetVariant(variantInfo.parent);
                GetCharacterVariantProperties(info, parent, layers, out pivotX, out pivotY, out width, out height);
            }
            else
            {
                pivotX = 0.5f;
                pivotY = 0.5f;
                width = 0;
                height = 0;
            }
            layers.AddRange(variantInfo.layers);
            pivotX = variantInfo.pivotX ?? pivotX;
            pivotY = variantInfo.pivotY ?? pivotY;
            width = variantInfo.width ?? width;
            height = variantInfo.height ?? height;
        }
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
                var characterID = new NamespaceID(nsp, meta.name);
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
                modResource.CharacterVariantSprites.Add(meta.name, sprites.ToArray());
            }
        }
        #endregion
    }
}
