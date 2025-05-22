using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Minigames
{
    public class MinigameCategoryItem : MonoBehaviour
    {
        public void UpdateItem(MinigameCategoryItemViewData viewData)
        {
            var sprite = viewData.sprite;
            icon.sprite = sprite;
            icon.enabled = sprite;
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        public Action<MinigameCategoryItem> OnClick;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image icon;
    }
    public struct MinigameCategoryItemViewData
    {
        public Sprite sprite;
    }
}
