using UnityEngine;

namespace MVZ2.Models
{
    [RequireComponent(typeof(RendererElement))]
    [ExecuteAlways]
    public class HSVOffsetSetter : MonoBehaviour
    {
        private void OnEnable()
        {
            element = GetComponent<RendererElement>();
        }
        private void Update()
        {
            if (string.IsNullOrEmpty(propertyName))
                return;
            element.SetVector(propertyName, new Vector4(hue, saturation, value, 0));
        }
        private RendererElement element;
        [SerializeField]
        private string propertyName = "_HSVOffset";
        [Range(-180, 180)]
        [SerializeField]
        private float hue;
        [Range(-100, 100)]
        [SerializeField]
        private float saturation;
        [Range(-100, 100)]
        [SerializeField]
        private float value;
    }
}
