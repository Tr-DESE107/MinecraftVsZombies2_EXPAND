using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class LevelLoadedDialog : MonoBehaviour
    {
        private void Awake()
        {
            resumeButton.onClick.AddListener(() => OnButtonClicked?.Invoke(ButtonType.Resume));
            restartButton.onClick.AddListener(() => OnButtonClicked?.Invoke(ButtonType.Restart));
            exitButton.onClick.AddListener(() => OnButtonClicked?.Invoke(ButtonType.Exit));
        }
        public event Action<ButtonType> OnButtonClicked;
        [SerializeField]
        private Button resumeButton;
        [SerializeField]
        private Button restartButton;
        [SerializeField]
        private Button exitButton;
        public enum ButtonType
        {
            Resume,
            Restart,
            Exit
        }
    }
}
