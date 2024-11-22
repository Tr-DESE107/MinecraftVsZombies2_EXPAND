using System.Collections.Generic;
using MVZ2.UI;
using TMPro;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class Tooltip : MonoBehaviour
    {
        public void SetPivot(Vector2 pivot)
        {
            var rectTransform = transform as RectTransform;
            rectTransform.pivot = pivot;
        }
        public void SetData(Vector2 position, TooltipViewData viewData)
        {
            var rectTransform = transform as RectTransform;
            rectTransform.position = position;
            nameText.text = viewData.name;
            errorText.text = viewData.error;
            descriptionText.text = viewData.description;
            nameText.gameObject.SetActive(!string.IsNullOrEmpty(viewData.name));
            errorText.gameObject.SetActive(!string.IsNullOrEmpty(viewData.error));
            descriptionText.gameObject.SetActive(!string.IsNullOrEmpty(viewData.description));

            var rootCanvas = rectTransform.GetRootCanvasNonAlloc(canvasListCache);
            if (rootCanvas)
            {
                rectTransform.LimitInsideScreen(rootCanvas.transform, rootTransform.rect.size);
            }
        }
        [SerializeField]
        RectTransform rootTransform;
        [SerializeField]
        TextMeshProUGUI nameText;
        [SerializeField]
        TextMeshProUGUI errorText;
        [SerializeField]
        TextMeshProUGUI descriptionText;
        private List<Canvas> canvasListCache = new List<Canvas>();
    }
    public struct TooltipViewData
    {
        public string name;
        public string error;
        public string description;
    }
}
