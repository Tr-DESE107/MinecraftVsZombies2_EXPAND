using UnityEngine;

namespace MVZ2.Level.UI
{
    public class TooltipAnchor : MonoBehaviour
    {
        public bool IsDisabled => disabled;
        public Vector2 Pivot => pivot;
        [SerializeField]
        private bool disabled = false;
        [SerializeField]
        private Vector2 pivot = new Vector2(0.5f, 0.5f);
    }
    public interface ITooltipTarget
    {
        TooltipAnchor Anchor { get; }
    }
}
