﻿using UnityEngine;

namespace MVZ2.Models
{
    public class NightmareEye : MonoBehaviour
    {
        public float GetAlpha()
        {
            Color col = eyeRenderer.color;
            return col.a;
        }
        public void SetAlpha(float value)
        {
            Color col = eyeRenderer.color;
            col.a = value;
            eyeRenderer.color = col;
        }
        public void SetTime(int time)
        {
            int index = Mathf.Clamp((int)(time / interval), 0, indexes.Length - 1);
            int i = indexes[index];
            spriteSetter.SetSpriteIndex(i);
        }

        private float interval = 1.5f;
        private static int[] indexes = new int[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 6
        };
        [SerializeField]
        private SpriteRenderer eyeRenderer;
        [SerializeField]
        private AnimationSpriteSetter spriteSetter;
    }
}
