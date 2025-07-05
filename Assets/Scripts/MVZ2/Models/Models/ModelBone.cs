using UnityEngine;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    public sealed class ModelBone : MonoBehaviour
    {
        public void SetLightVisible(bool visible)
        {
            if (lightController)
            {
                lightController.gameObject.SetActive(visible);
            }
        }
        public void SetLightColor(Color color)
        {
            if (lightController)
            {
                lightController.SetColor(color);
            }
        }
        public void SetLightRange(Vector2 range)
        {
            if (lightController)
            {
                lightController.SetRange(range);
            }
        }
        public void SetColliderActive(bool active)
        {
            if (modelCollider)
            {
                modelCollider.enabled = active;
            }
        }
        public Collider2D Collider => modelCollider;
        [Header("Collision")]
        [SerializeField]
        private Collider2D modelCollider;

        [Header("Light")]
        [SerializeField]
        private LightController lightController;
    }
}