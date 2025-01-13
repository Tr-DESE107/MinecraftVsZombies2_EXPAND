using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public class MobileColliderExpander : MonoBehaviour
    {
        private void Update()
        {
            var shouldActive = MainManager.Instance.IsMobile();
            if (active != shouldActive)
            {
                if (shouldActive)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }
        }
        private void OnDisable()
        {
            Disable();
        }
        private void Enable()
        {
            if (active)
                return;
            active = true;
            var collider = GetComponent<Collider2D>();
            switch (collider)
            {
                case CircleCollider2D circle:
                    circle.radius *= 2;
                    break;
                case BoxCollider2D box:
                    box.size *= 2;
                    break;
            }
        }
        private void Disable()
        {
            if (!active)
                return;
            active = false;
            var collider = GetComponent<Collider2D>();
            switch (collider)
            {
                case CircleCollider2D circle:
                    circle.radius *= 0.5f;
                    break;
                case BoxCollider2D box:
                    box.size *= 0.5f;
                    break;
            }
        }
        private bool active;
    }
}
