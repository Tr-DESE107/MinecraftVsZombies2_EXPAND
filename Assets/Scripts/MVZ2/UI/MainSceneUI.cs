using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class MainSceneUI : MonoBehaviour
    {
        public void SetBlackScreen(float value)
        {
            blackscreenFader.Value = value;
        }
        public void FadeBlackScreen(float target, float duration)
        {
            blackscreenFader.StartFade(target, duration);
        }
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            dialog.gameObject.SetActive(true);
            dialog.SetDialog(title, desc, options, (i) =>
            {
                onSelect?.Invoke(i);
                dialog.gameObject.SetActive(false);
            });
            dialog.ResetPosition();
        }
        private void Awake()
        {
            blackscreenFader.OnValueChanged += OnBlackscreenFaderValueChangedCallback;
        }
        private void OnBlackscreenFaderValueChangedCallback(float value)
        {
            var color = blackscreenImage.color;
            color.a = value;
            blackscreenImage.color = color;
        }
        [SerializeField]
        private CustomDialog dialog;
        [SerializeField]
        private Image blackscreenImage;
        [SerializeField]
        private FloatFader blackscreenFader;
    }
}
