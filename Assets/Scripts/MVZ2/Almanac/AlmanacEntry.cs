using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Almanacs
{
    public class AlmanacEntry : MonoBehaviour
    {
        public void UpdateEntry(AlmanacEntryViewData viewData)
        {
            rootObject.SetActive(!viewData.empty);
            var sprite = viewData.sprite;
            icon.sprite = sprite;
            icon.enabled = sprite;
            if (sprite)
            {
                ratioFitter.aspectRatio = sprite.rect.width / sprite.rect.height;
            }
            else
            {
                ratioFitter.aspectRatio = 1;
            }
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        public Action<AlmanacEntry> OnClick;
        [SerializeField]
        private GameObject rootObject;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private AspectRatioFitter ratioFitter;
    }
    public struct AlmanacEntryViewData
    {
        public bool empty;
        public Sprite sprite;
        public static readonly AlmanacEntryViewData Empty = new AlmanacEntryViewData { empty = true };
    }
}
