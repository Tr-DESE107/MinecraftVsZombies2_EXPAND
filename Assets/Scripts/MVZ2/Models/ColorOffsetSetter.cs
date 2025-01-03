using Tools;
using UnityEngine;

namespace MVZ2.Models
{
    [RequireComponent(typeof(RendererElement))]
    [ExecuteAlways]
    public class ColorOffsetSetter : MonoBehaviour
    {
        private void OnEnable()
        {
            element = GetComponent<RendererElement>();
        }
        private void Update()
        {
            if (string.IsNullOrEmpty(propertyName))
                return;
            element.SetColor(propertyName, color);
        }
        private RendererElement element;
        [SerializeField]
        private string propertyName;
        [SerializeField]
        private Color color;
    }
}
