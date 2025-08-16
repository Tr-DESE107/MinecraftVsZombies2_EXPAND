using UnityEngine;

namespace MVZ2.UI
{
    public class TooltipAnchor : MonoBehaviour, ITooltipAnchor
    {
        public bool IsDisabled => disabled || !isActiveAndEnabled;
        public Vector2 Pivot => pivot;
        public Vector3 Position => transform.position;
        [SerializeField]
        private bool disabled = false;
        [SerializeField]
        private Vector2 pivot = new Vector2(0.5f, 0.5f);
    }
}
