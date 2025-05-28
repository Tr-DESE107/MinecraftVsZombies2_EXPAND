using System;
using MVZ2.UI;
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
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void SetScale(float scale)
        {
            imageRectTransform.localScale = Vector3.one * scale;
        }
        public void SetSliderText(string text)
        {
            scaleSlider.Text.text = text;
        }
        public void SetSliderValue(float value)
        {
            scaleSlider.Slider.SetValueWithoutNotify(value);
        }
        private void Awake()
        {
            returnButton.onClick.AddListener(() => OnReturnClick?.Invoke());
            scaleSlider.Slider.onValueChanged.AddListener((v) => OnScaleValueChanged?.Invoke(v));
        }
        public Action OnReturnClick;
        public Action<float> OnScaleValueChanged;
        [SerializeField]
        private Image image;
        [SerializeField]
        private RectTransform imageRectTransform;
        [SerializeField]
        private TextSlider scaleSlider;
        [SerializeField]
        private Button returnButton;
    }
}
