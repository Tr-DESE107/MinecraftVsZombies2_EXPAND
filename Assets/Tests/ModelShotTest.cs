using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.Tests
{
    public class ModelShotTest : MonoBehaviour
    {
        private void Update()
        {
            spriteRenderer.sprite = ShotIcon();
            Debug.Break();
        }
        private Sprite ShotIcon()
        {
            var pictureName = name ?? "ModelIcon";

            //设置模型

            //创建一个用于渲染图片的RenderTexture
            RenderTexture renderTexture = new RenderTexture(width, height, 32);
            renderTexture.filterMode = FilterMode.Point;
            renderTexture.antiAliasing = 2;
            modelShotCamera.targetTexture = renderTexture;
            modelShotCamera.enabled = false;

            // 相机渲染。
            modelShotCamera.orthographicSize = height * 0.005f;
            var localPos = modelShotPositionTransform.localPosition;
            localPos.x = modelOffset.x * 0.01f;
            localPos.y = -modelShotCamera.orthographicSize + modelOffset.y * 0.01f;
            modelShotPositionTransform.localPosition = localPos;
            SortingGroup.UpdateAllSortingGroups();
            modelShotCamera.Render();

            // 从Render Texture读取像素并保存为图片
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            texture.name = pictureName;
            RenderTexture.active = null; // 重置活动的Render Texture
            modelShotCamera.targetTexture = null;
            renderTexture.Release();

            // 创建Sprite。
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f);
            sprite.name = pictureName;

            return sprite;
        }
        public Camera modelShotCamera;
        public Transform modelShotPositionTransform;
        public SpriteRenderer spriteRenderer;
        public int width;
        public int height;
        public Vector2 modelOffset;
    }
}
