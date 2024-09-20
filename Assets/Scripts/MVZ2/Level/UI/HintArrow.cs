using UnityEngine;

namespace MVZ2.UI
{
    public class HintArrow : MonoBehaviour
    {
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
        public void SetTarget(Transform target, Vector2 offset, float angle)
        {
            targetTransform = target;
            targetOffset = offset;
            transform.localEulerAngles = Vector3.forward * angle;
        }
        private void Update()
        {
            if (targetTransform)
            {
                var position = transform.position;
                position.x = targetTransform.position.x + targetOffset.x;
                position.y = targetTransform.position.y + targetOffset.y;
                transform.position = position;
            }
        }
        [SerializeField]
        private Transform targetTransform;
        [SerializeField]
        private Vector2 targetOffset;
    }
}
