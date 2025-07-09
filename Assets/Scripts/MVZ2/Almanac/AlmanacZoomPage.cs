using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Almanacs
{
    public class AlmanacZoomPage : MonoBehaviour
    {
        public void Display()
        {
            gameObject.SetActive(true);
            panZoomController.ResetView();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }
        public void SetZoomHintText(string text)
        {
            hintText.text = text;
        }
        public void SetPageButtonActive(bool active)
        {
            prevButton.gameObject.SetActive(active);
            nextButton.gameObject.SetActive(active);
        }
        private void Awake()
        {
            returnButton.onClick.AddListener(() => OnReturnClick?.Invoke());
            prevButton.onClick.AddListener(() => OnPageButtonClick?.Invoke(false));
            nextButton.onClick.AddListener(() => OnPageButtonClick?.Invoke(true));
        }
        public Action OnReturnClick;
        public Action<bool> OnPageButtonClick;
        [SerializeField]
        private TextMeshProUGUI hintText;
        [SerializeField]
        private Image image;
        [SerializeField]
        private PanZoomController panZoomController;
        [SerializeField]
        private Button prevButton;
        [SerializeField]
        private Button nextButton;
        [SerializeField]
        private Button returnButton;
    }
}
