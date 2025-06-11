﻿using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSizeFitter : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var renderer = Renderer;
            var spr = renderer.sprite;
            if (!spr)
                return;
            float scale = 1;
            var sprSize = spr.rect.size;
            if (mode == SpriteFitMode.SmallerThanSize)
            {
                scale = Mathf.Min(1, maxSize.x / sprSize.x, maxSize.y / sprSize.y);
            }
            else if (mode == SpriteFitMode.FitSize)
            {
                scale = Mathf.Min(maxSize.x / sprSize.x, maxSize.y / sprSize.y);
            }
            renderer.transform.localScale = Vector3.one * scale;
        }
        public SpriteRenderer Renderer
        {
            get
            {
                if (!_sprRenderer)
                {
                    _sprRenderer = GetComponent<SpriteRenderer>();
                }
                return _sprRenderer;
            }
        }
        private SpriteRenderer _sprRenderer;
        [SerializeField]
        private SpriteFitMode mode = SpriteFitMode.SmallerThanSize;
        [SerializeField]
        private Vector2 maxSize = new Vector2(64, 64);
    }
    public enum SpriteFitMode
    {
        SmallerThanSize = 0,
        FitSize = 1
    }
}
