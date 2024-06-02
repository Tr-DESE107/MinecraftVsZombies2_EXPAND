using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class PauseDialog : MonoBehaviour
    {
        public void SetPausedImage(Sprite sprite)
        {
            pausedImage.sprite = sprite;
        }
        private void Awake()
        {
            resumeButton.onClick.AddListener(() => OnResumeClicked?.Invoke());
        }
        public event Action OnResumeClicked;
        [SerializeField]
        private Image pausedImage;
        [SerializeField]
        private Button resumeButton;
    }
}
