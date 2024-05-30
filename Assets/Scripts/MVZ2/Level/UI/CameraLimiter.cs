using UnityEngine;

namespace MVZ2.Level.UI
{
    [ExecuteAlways]
    public class CameraLimiter : MonoBehaviour
    {
        private void OnEnable()
        {
            var currentAspect = Screen.width / (float)Screen.height;
            UpdateCamera(currentAspect);
        }
        private void OnDisable()
        {
            var rect = _camera.rect;
            rect.size = Vector2.one;
            rect.center = new Vector2(0.5f, 0.5f);
            _camera.rect = rect;
        }
        private void Update()
        {
            var currentAspect = Screen.width / (float)Screen.height;
            if (currentAspect != lastAspect)
            {
                lastAspect = currentAspect;
                UpdateCamera(currentAspect);
            }
        }
        private void UpdateCamera(float currentAspect)
        {
            if (!_camera)
                return;
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

        private float lastAspect;
        [SerializeField]
        protected float safeAspectMin = -1;
        [SerializeField]
        protected float safeAspectMax = -1;
        [SerializeField]
        protected Camera _camera;
    }
}