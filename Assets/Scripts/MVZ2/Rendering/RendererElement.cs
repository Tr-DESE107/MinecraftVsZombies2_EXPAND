using UnityEngine;

namespace MVZ2.Rendering
{
    public class RendererElement : MonoBehaviour
    {
        public bool ExcludedInGroup => excludedInGroup;
        [SerializeField]
        private bool excludedInGroup;
    }
}
