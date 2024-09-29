using UnityEngine;

namespace MVZ2.Rendering
{
    public class LightCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            lightCamera.SetReplacementShader(entityBlackShader, "RenderType");
        }
        private void OnDisable()
        {
            lightCamera.ResetReplacementShader();
        }
        [SerializeField]
        private Camera lightCamera;
        [SerializeField]
        private Shader entityBlackShader;
    }
}
