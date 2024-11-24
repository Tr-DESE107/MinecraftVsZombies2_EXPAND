using MVZ2.Managers;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.Models
{
    public class ModelManager : MonoBehaviour
    {
        public Sprite ShotIcon(Model model, int width, int height, Vector2 modelOffset, string name = null)
        {
            var pictureName = name ?? "ModelIcon";
            //激活摄像机与灯光
            modelShotRoot.gameObject.SetActive(true);

            //设置模型
            var modelInstance = Instantiate(model.gameObject, modelShotPositionTransform);
            modelInstance.transform.localPosition = Vector3.zero;

            //创建一个用于渲染图片的RenderTexture
            RenderTexture renderTexture = new RenderTexture(width, height, 32);
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
            Texture2D texture = new Texture2D(width, height);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            texture.name = pictureName;
            RenderTexture.active = null; // 重置活动的Render Texture
            modelShotCamera.targetTexture = null;
            renderTexture.Release();
            DestroyImmediate(modelInstance);

            // 创建Sprite。
            Sprite sprite = main.ResourceManager.CreateSprite(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f, pictureName, "modelIcon");

            return sprite;
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private Transform modelShotRoot;
        [SerializeField]
        private Transform modelShotPositionTransform;
        [SerializeField]
        private Camera modelShotCamera;
    }
}
