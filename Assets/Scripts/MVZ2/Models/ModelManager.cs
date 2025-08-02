using MVZ2.Managers;
using PVZEngine;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.Models
{
    public class ModelManager : MonoBehaviour
    {
        public Sprite ShotIcon(NamespaceID id, int width, int height, Vector2 modelOffset, string name = null)
        {
            var pictureName = name ?? "ModelIcon";
            //激活摄像机与灯光
            modelShotRoot.gameObject.SetActive(true);

            //设置模型
            var modelInstance = Model.Create(id, modelShotPositionTransform, modelShotCamera);
            if (!modelInstance)
            {
                Debug.LogWarning($"Prefab of model {id} is missing!");
                return null;
            }
            modelInstance.transform.localPosition = Vector3.zero;
            modelInstance.UpdateAnimators(0);
            modelInstance.UpdateFrame(0);


            //创建一个用于渲染图片的RenderTexture
            var colorFormat = Main.GraphicsManager.GetSupportedColorFormat();
            var depthFormat = Main.GraphicsManager.GetSupportedDepthFormat();
            RenderTexture renderTexture = new RenderTexture(width, height, colorFormat, depthFormat);
            renderTexture.antiAliasing = 1;
            renderTexture.filterMode = FilterMode.Trilinear;
            modelShotCamera.targetTexture = renderTexture;
            modelShotCamera.enabled = false;

            // 相机渲染。
            modelShotCamera.orthographicSize = height * 0.005f;

            var localPos = modelShotPositionTransform.localPosition;
            localPos.x = modelOffset.x * 0.01f;
            localPos.y = modelOffset.y * 0.01f;
            modelShotPositionTransform.localPosition = localPos;

            SortingGroup.UpdateAllSortingGroups();

            Shader.SetGlobalInt("_PixelSnap", 1);

            // Create a standard request
            RenderPipeline.StandardRequest request = new RenderPipeline.StandardRequest();

            // Check if the request is supported by the active render pipeline
            if (RenderPipeline.SupportsRenderRequest(modelShotCamera, request))
            {
                // 2D Texture
                request.destination = renderTexture;
                // Render camera and fill texture2D with its view
                RenderPipeline.SubmitRenderRequest(modelShotCamera, request);
            }
            else
            {
                modelShotCamera.Render();
            }
            Shader.SetGlobalInt("_PixelSnap", 0);

            // 从Render Texture读取像素并保存为图片
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Bilinear;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            texture.name = pictureName;
            RenderTexture.active = null; // 重置活动的Render Texture
            modelShotCamera.targetTexture = null;
            renderTexture.Release();
            DestroyImmediate(modelInstance.gameObject);

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
