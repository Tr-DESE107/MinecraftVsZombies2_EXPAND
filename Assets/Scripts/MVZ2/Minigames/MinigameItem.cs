using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Minigames
{
    public class MinigameItem : MonoBehaviour
    {
        public void UpdateItem(MinigameItemViewData viewData)
        {
            rootObject.SetActive(!viewData.empty);
            var sprite = viewData.sprite;
            icon.sprite = sprite;
            icon.enabled = sprite;

            clearSprite.sprite = viewData.clearSprite;
            clearSprite.enabled = viewData.clearSprite;

            nameText.text = viewData.name;
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        public Action<MinigameItem> OnClick;
        [SerializeField]
        private GameObject rootObject;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Image clearSprite;
        [SerializeField]
        private TextMeshProUGUI nameText;
    }
    public struct MinigameItemViewData
    {
        public bool empty;
        public Sprite sprite;
        public string name;
        public Sprite clearSprite;
        public static readonly MinigameItemViewData Empty = new MinigameItemViewData { empty = true };
    }
}
