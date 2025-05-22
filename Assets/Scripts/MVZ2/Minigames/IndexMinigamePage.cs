using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Minigames
{
    public class IndexMinigamePage : MonoBehaviour
    {
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        private void Awake()
        {
            minigameItem.OnClick += (item) => OnButtonClick?.Invoke(ButtonType.Minigame);
            puzzleItem.OnClick += (item) => OnButtonClick?.Invoke(ButtonType.Puzzle);
            returnButton.onClick.AddListener(() => OnReturnClick?.Invoke());
        }
        public event Action OnReturnClick;
        public event Action<ButtonType> OnButtonClick;
        [SerializeField]
        private MinigameCategoryItem minigameItem;
        [SerializeField]
        private MinigameCategoryItem puzzleItem;
        [SerializeField]
        private Button returnButton;
        public enum ButtonType
        {
            Minigame,
            Puzzle
        }
    }
}
