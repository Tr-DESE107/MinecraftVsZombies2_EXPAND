using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Arcade
{
    public class ArcadeItem : MonoBehaviour
    {
        public void UpdateItem(ArcadeItemViewData viewData)
        {
            rootObject.SetActive(!viewData.empty);

            icon.gameObject.SetActive(viewData.unlocked);
            lockedIcon.gameObject.SetActive(!viewData.unlocked);

            button.interactable = viewData.unlocked;

            var sprite = viewData.sprite;
            icon.sprite = sprite;
            icon.enabled = sprite;

            clearSprite.sprite = viewData.clearSprite;
            clearSprite.enabled = viewData.clearSprite;

            nameText.text = viewData.name;
            hintText.text = viewData.hint;
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        public Action<ArcadeItem> OnClick;
        [SerializeField]
        private GameObject rootObject;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Image lockedIcon;
        [SerializeField]
        private Image clearSprite;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private TextMeshProUGUI hintText;
    }
    public struct ArcadeItemViewData
    {
        public bool empty;
        public Sprite sprite;
        public string hint;
        public string name;
        public Sprite clearSprite;
        public bool unlocked;
        public static readonly ArcadeItemViewData Empty = new ArcadeItemViewData { empty = true };
    }
}
