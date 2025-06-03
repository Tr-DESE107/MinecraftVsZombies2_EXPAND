using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class AlmanacTagIconLayer : MonoBehaviour
    {
        public void UpdateView(AlmanacTagIconLayerViewData viewData)
        {
            transform.localPosition = (Vector3)viewData.offset;
            transform.localScale = viewData.scale;
            image.sprite = viewData.sprite;
            image.enabled = image.sprite;
            image.color = viewData.tint;
        }
        [SerializeField]
        private Image image;
    }
    public struct AlmanacTagIconLayerViewData
    {
        public Vector2 offset;
        public Vector3 scale;
        public Sprite sprite;
        public Color tint;
    }
}
