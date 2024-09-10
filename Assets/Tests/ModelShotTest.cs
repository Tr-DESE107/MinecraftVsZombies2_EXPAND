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
            //创建一个用于渲染图片的RenderTexture
            var renderTexture = CreateRenderTexture();
            //设置模型。
            SetupModel();
            // 相机渲染。
            CameraRender(renderTexture);
            // 从Render Texture读取像素并保存为图片
            var texture = GrabTexture(renderTexture, pictureName);
            var sprite = CreateSprite(texture, pictureName);
            FinalClean(renderTexture);
            // 创建Sprite。
            return sprite;
        }
        private RenderTexture CreateRenderTexture()
        {
            RenderTexture renderTexture = new RenderTexture(width, height, 32);
            renderTexture.filterMode = FilterMode.Point;
            renderTexture.antiAliasing = 2;
            return renderTexture;
        }
        private void SetupModel()
        {
            var localPos = modelShotPositionTransform.localPosition;
            localPos.x = modelOffset.x * 0.01f;
            localPos.y = -modelShotCamera.orthographicSize + modelOffset.y * 0.01f;
            modelShotPositionTransform.localPosition = localPos;
        }
        private void CameraRender(RenderTexture renderTexture)
        {
            modelShotCamera.targetTexture = renderTexture;
            modelShotCamera.enabled = false;
            modelShotCamera.orthographicSize = height * 0.005f;

            SortingGroup.UpdateAllSortingGroups();
            modelShotCamera.Render();
        }
        private Texture2D GrabTexture(RenderTexture renderTexture, string pictureName)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            texture.name = pictureName;
            RenderTexture.active = null; // 重置活动的Render Texture

            return texture;
        }
        private void FinalClean(RenderTexture renderTexture)
        {
            modelShotCamera.targetTexture = null;
            renderTexture.Release();
        }
        private Sprite CreateSprite(Texture2D texture, string pictureName)
        {
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
