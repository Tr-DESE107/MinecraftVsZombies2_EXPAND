﻿using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    public class LocalSpriteRectSetter : ShaderPropertySetter<Vector4>
    {
        public override Vector4 GetCurrentValue()
        {
            if (Element.Renderer is not SpriteRenderer sprRenderer)
                return defaultValue;
            Sprite sprite = sprRenderer.sprite;
            if (!sprite)
                return defaultValue;
            if (lastSprite != sprite)
            {
                currentValue = new Vector4(
                sprite.textureRect.min.x / sprite.texture.width,
                sprite.textureRect.min.y / sprite.texture.height,
                sprite.textureRect.max.x / sprite.texture.width,
                sprite.textureRect.max.y / sprite.texture.height);
            }
            return currentValue;
        }
        public override Vector4 GetDefaultValue() => defaultValue;
        public override void SetProperty(Vector4 value)
        {
            Element.SetVector("_LocalRect", value);
        }
        private Sprite lastSprite;
        private Vector4 currentValue;
        private Vector4 defaultValue = new Vector4(0, 0, 1, 1);
    }
}
