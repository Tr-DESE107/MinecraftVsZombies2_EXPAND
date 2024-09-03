using UnityEngine;

namespace MVZ2.Level.UI
{
    [ExecuteAlways]
    public class LevelCamera : MonoBehaviour
    {
        public void SetPosition(Vector3 position, Vector2 anchor)
        {
            cameraPosition = position;
            cameraAnchor = anchor;
            UpdatePosition();
        }
        private void OnEnable()
        {
            UpdatePosition();
        }
        private void Update()
        {
            UpdatePosition();
        }
        private void UpdatePosition()
        {
            if (!_camera)
                return;
            var aspect = Screen.width * _camera.rect.width / (Screen.height * _camera.rect.height);
            var height = _camera.orthographicSize * 2;
            var cameraSize = new Vector2(height * aspect, height);
            var targetPos = CameraPosition + (Vector3)((Vector2.one * 0.5f - (Vector2)CameraAnchor) * cameraSize);
            _camera.transform.position = targetPos + ShakeOffset;
        }
        public Vector3 CameraPosition
        {
            get => cameraPosition;
            set
            {
                cameraPosition = value;
                UpdatePosition();
            }
        }
        public Vector2 CameraAnchor
        {
            get => cameraAnchor;
            set
            {
                cameraAnchor = value;
                UpdatePosition();
            }
        }
        public Vector3 ShakeOffset
        {
            get => cameraShakeOffset;
            set
            {
                cameraShakeOffset = value;
                UpdatePosition();
            }
        }
        public Camera Camera => _camera;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Vector2 cameraAnchor;
        [SerializeField]
        private Vector3 cameraPosition;
        private Vector3 cameraShakeOffset;
    }
}