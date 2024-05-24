using UnityEngine;

namespace MVZ2.Rendering
{
    public class RendererElement : MonoBehaviour
    {
        public bool ExcludedInGroup => excludedInGroup;
        public bool LockToGround => lockToGround;
        [SerializeField]
        private bool excludedInGroup;
        [SerializeField]
        private bool lockToGround;
    }
}
