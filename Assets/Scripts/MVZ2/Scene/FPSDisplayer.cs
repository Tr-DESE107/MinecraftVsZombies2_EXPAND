using TMPro;
using UnityEngine;

namespace MVZ2.Scenes
{
    public class FPSDisplayer : MonoBehaviour
    {
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public void SetCorner(Vector2 corner)
        {
            rectTransform.pivot = corner;
            rectTransform.anchorMin = corner;
            rectTransform.anchorMax = corner;
            rectTransform.anchoredPosition = Vector3.zero;
        }
        public void SetFPS(string text)
        {
            fpsText.text = text;
        }
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private TextMeshProUGUI fpsText;
    }
}
