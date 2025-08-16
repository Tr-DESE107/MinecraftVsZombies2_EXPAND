using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class Tooltip : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void SetView(TooltipViewData viewData)
        {
            SetContent(viewData.content);

            var rectTransform = transform as RectTransform;
            rectTransform.pivot = viewData.pivot;
            rectTransform.position = viewData.position;
            var rootCanvas = rectTransform.GetRootCanvasNonAlloc(canvasListCache);
            if (rootCanvas)
            {
                rectTransform.LimitInsideScreen(rootCanvas.transform, rootTransform.rect.size);
            }
        }
        public void SetContent(TooltipContent content)
        {
            nameText.text = content.name;
            errorText.text = content.error;
            descriptionText.text = content.description;
            nameText.gameObject.SetActive(!string.IsNullOrEmpty(content.name));
            errorText.gameObject.SetActive(!string.IsNullOrEmpty(content.error));
            descriptionText.gameObject.SetActive(!string.IsNullOrEmpty(content.description));
        }
        public void ForceRebuildLayout()
        {
            var rectTransform = transform as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
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
        public Vector2 pivot;
        public Vector2 position;
        public TooltipContent content;
    }
    public struct TooltipContent
    {
        public Vector2 pivot;
        public Vector2 position;
        public string name;
        public string error;
        public string description;
    }
}
