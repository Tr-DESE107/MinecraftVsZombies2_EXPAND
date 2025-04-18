﻿using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Localization
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererTranslator : TranslateComponentSprite<SpriteRenderer>
    {
        protected override Sprite GetKeyInner()
        {
            return Component.sprite;
        }
        protected override void Translate(string language)
        {
            base.Translate(language);
            Component.sprite = MainManager.Instance.GetFinalSprite(Key, language);
        }
    }
}
