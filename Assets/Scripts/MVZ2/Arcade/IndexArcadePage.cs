using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Arcade
{
    public class IndexArcadePage : MonoBehaviour
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
        private ArcadeCategoryItem minigameItem;
        [SerializeField]
        private ArcadeCategoryItem puzzleItem;
        [SerializeField]
        private Button returnButton;
        public enum ButtonType
        {
            Minigame,
            Puzzle
        }
    }
}
