using UnityEngine;

namespace MVZ2.Rendering
{
    public class RendererElement : MonoBehaviour
    {
        public Renderer Renderer
        {
            get
            {
                if (!_renderer)
                {
                    _renderer = GetComponent<Renderer>();
                }
                return _renderer;
            }
        }
        public bool ExcludedInGroup => excludedInGroup;
        public bool LockToGround => lockToGround;
        [SerializeField]
        private bool excludedInGroup;
        [SerializeField]
        private bool lockToGround;
        private Renderer _renderer;
    }
}
