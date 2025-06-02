﻿using MVZ2.Managers;
using UnityEngine;

namespace Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteAlways]
    public class AnimationSpriteSetter : MonoBehaviour
    {
        private void OnEnable()
        {
            SetSpriteIndex(index);
        }
        private void LateUpdate()
        {
            if (sprites != null && sprites.Length > 0)
            {
                if (index != beforeIndex || Application.isEditor)
                {
                    SetSpriteIndex(index);
                }
            }
        }


        public void SetSpritePercent(float percent)
        {
            SetSpriteIndex(Mathf.FloorToInt(percent * sprites.Length));
        }

        public void SetSpriteIndex(int i)
        {
            index = Mathf.Clamp(i, 0, sprites.Length - 1);
            beforeIndex = index;
            Sprite sprite = sprites[index];
            if (Main)
            {
                sprite = Main.GetFinalSprite(sprite);
            }
            Renderer.sprite = sprite;
        }
        private SpriteRenderer Renderer
        {
            get
            {
                if (!sprRenderer)
                {
                    sprRenderer = GetComponent<SpriteRenderer>();
                }
                return sprRenderer;
            }
        }
        private MainManager Main => MainManager.Instance;
        private SpriteRenderer sprRenderer;
        public Sprite[] sprites;
        private int beforeIndex = -1;
        [SerializeField]
        private int index = 0;
    }
}