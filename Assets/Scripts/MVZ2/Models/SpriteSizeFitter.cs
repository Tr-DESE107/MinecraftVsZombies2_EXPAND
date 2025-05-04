using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVZ2.Models
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSizeFitter : MonoBehaviour
    {
        private void Update()
        {
            var renderer = sprRenderer;
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
        private SpriteRenderer sprRenderer
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
