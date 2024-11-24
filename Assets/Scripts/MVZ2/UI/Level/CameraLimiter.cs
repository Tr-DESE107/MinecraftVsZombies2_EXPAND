using MVZ2.Cameras;
using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Level.UI
{
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
            bool isMobile = MainManager.Instance.IsMobile();
            var min = isMobile ? safeAspectMinMobile : safeAspectMin;
            var max = isMobile ? safeAspectMaxMobile : safeAspectMax;
            var currentAspect = width / (float)height;
            var rectSize = new Vector2(1, 1);
            if (min > 0 && currentAspect < min)
            {
                rectSize = new Vector2(1, 1 / (min / currentAspect));
            }
            else if (max > 0 && currentAspect > max)
            {
                rectSize = new Vector2(max / currentAspect, 1);
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
        protected float safeAspectMinMobile = 1.7f;
        [SerializeField]
        protected float safeAspectMaxMobile = 1.7f;
        [SerializeField]
        protected Camera _camera;
    }
}