using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Level.UI
{
    [ExecuteAlways]
    [DefaultExecutionOrder(1)]
    public class CameraLimiter : MonoBehaviour
    {
        private void OnEnable()
        {
            ResolutionManager.OnResolutionChanged += OnResolutionChangedCallback;
            UpdateCamera(Screen.width, Screen.height);
        }
        private void OnDisable()
        {
            ResolutionManager.OnResolutionChanged -= OnResolutionChangedCallback;
        }
        private void OnResolutionChangedCallback(int width, int height)
        {
            UpdateCamera(width, height);
        }
        private void UpdateCamera(int width, int height)
        {
            if (!_camera)
                return;
            var currentAspect = width / (float)height;
            var rectSize = new Vector2(1, 1);
            if (safeAspectMin > 0 && currentAspect < safeAspectMin)
            {
                rectSize = new Vector2(1, 1 / (safeAspectMin / currentAspect));
            }
            else if (safeAspectMax > 0 && currentAspect > safeAspectMax)
            {
                rectSize = new Vector2(safeAspectMax / currentAspect, 1);
            }
            var rect = _camera.rect;
            rect.size = rectSize;
            rect.center = new Vector2(0.5f, 0.5f);
            _camera.rect = rect;
        }

        [SerializeField]
        protected float safeAspectMin = -1;
        [SerializeField]
        protected float safeAspectMax = -1;
        [SerializeField]
        protected Camera _camera;
    }
}