using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Arcade
{
    public class ArcadeCategoryItem : MonoBehaviour
    {
        public void UpdateItem(ArcadeCategoryItemViewData viewData)
        {
            var sprite = viewData.sprite;
            icon.sprite = sprite;
            icon.enabled = sprite;
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        public Action<ArcadeCategoryItem> OnClick;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image icon;
    }
    public struct ArcadeCategoryItemViewData
    {
        public Sprite sprite;
    }
}
