using System.Linq;
using MVZ2.Talk;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public partial class ResourceManager : MonoBehaviour
    {
        public TalkCharacterMeta GetCharacterMeta(NamespaceID characterID)
        {
            var modResource = main.ResourceManager.GetModResource(characterID.spacename);
            if (modResource == null)
                return null;
            return modResource.TalkCharacterMetaList.metas.FirstOrDefault(m => m.name == characterID.path);
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
            var imageTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            foreach (TalkCharacterLayer layer in variantInfo.layers)
            {
                Sprite sourceSpr = main.ResourceManager.GetSprite(layer.sprite);
                Texture2D sourceTex = sourceSpr.texture;
                Rect sourceRect = sourceSpr.rect;
                var colors = sourceTex.GetPixels((int)sourceRect.xMin, (int)sourceRect.xMax, (int)sourceRect.width, (int)sourceRect.height);

                var layerPos = new Vector2(layer.positionX, layer.positionY);
                Rect targetRect = new Rect(layerPos, sourceRect.size);
                imageTexture.SetPixels((int)targetRect.xMin, (int)targetRect.yMin, (int)targetRect.width, (int)targetRect.height, colors);
            }

            imageTexture.Apply();

            Sprite spr = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), spritePivot);
            Destroy(imageTexture);
            return spr;
        }
    }
}
