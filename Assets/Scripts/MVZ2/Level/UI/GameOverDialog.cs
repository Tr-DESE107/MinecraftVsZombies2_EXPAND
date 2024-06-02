using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class GameOverDialog : MonoBehaviour
    {
        public void SetMessage(string message)
        {
            messageText.text = message;
        }
        public void SetInteractable(bool interactable)
        {
            retryButton.interactable = interactable;
            backButton.interactable = interactable;
        }
        private void Awake()
        {
            retryButton.onClick.AddListener(() => OnRetryButtonClicked?.Invoke());
            backButton.onClick.AddListener(() => OnBackButtonClicked?.Invoke());
        }
        public event Action OnRetryButtonClicked;
        public event Action OnBackButtonClicked;
        [SerializeField]
        private TextMeshProUGUI messageText;
        [SerializeField]
        private Button retryButton;
        [SerializeField]
        private Button backButton;
    }
}
