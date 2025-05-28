﻿using UnityEngine;

namespace MVZ2.Rendering
{
    public class RenderDowngrade : MonoBehaviour
    {
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            // 1. 将场景渲染到低分辨率RT
            Graphics.Blit(src, lowResRT);
            // 2. 将低分辨率RT放大到屏幕
            Graphics.Blit(lowResRT, dest);
        }
        [SerializeField]
        private RenderTexture lowResRT;
    }
}
