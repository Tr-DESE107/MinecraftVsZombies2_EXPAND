using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Almanacs
{
    public class AlmanacZoomPage : MonoBehaviour
    {
        public void Display(Sprite sprite)
        {
            image.sprite = sprite;
            gameObject.SetActive(true);
            panZoomController.ResetView();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void SetZoomHintText(string text)
        {
            hintText.text = text;
        }
        private void Awake()
        {
            returnButton.onClick.AddListener(() => OnReturnClick?.Invoke());
        }
        public Action OnReturnClick;
        [SerializeField]
        private TextMeshProUGUI hintText;
        [SerializeField]
        private Image image;
        [SerializeField]
        private PanZoomController panZoomController;
        [SerializeField]
        private Button returnButton;
    }
}
