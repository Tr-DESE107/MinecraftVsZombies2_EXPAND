using UnityEngine;

namespace MVZ2.Cameras
{
    public class LightCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            lightCamera.SetReplacementShader(entityBlackShader, "Lighting2D");
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
