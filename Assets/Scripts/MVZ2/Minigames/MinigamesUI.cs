using System;
using MVZ2.Models;
using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Minigames
{
    public class MinigamesUI : MonoBehaviour
    {
        public void DisplayPage(MinigamePage page)
        {
            indexUI.SetActive(page == MinigamePage.Index);
            minigamePage.SetActive(page == MinigamePage.Minigame);
            puzzlePage.SetActive(page == MinigamePage.Puzzle);
        }
        public void SetAllInteractable(bool interactable)
        {
            canvasGroup.interactable = interactable;
        }
        public void SetMinigameItems(MinigameItemViewData[] items)
        {
            minigamePage.SetItems(items);
        }
        public void SetPuzzleItems(MinigameItemViewData[] items)
        {
            puzzlePage.SetItems(items);
        }

        private void Awake()
        {
            indexUI.OnReturnClick += () => OnIndexReturnClick?.Invoke();
            indexUI.OnButtonClick += type => OnIndexButtonClick?.Invoke(type);
            minigamePage.OnEntryClick += index => OnItemClick?.Invoke(MinigamePage.Minigame, index);
            minigamePage.OnReturnClick += () => OnPageReturnClick?.Invoke(MinigamePage.Minigame);
            puzzlePage.OnEntryClick += index => OnItemClick?.Invoke(MinigamePage.Puzzle, index);
            puzzlePage.OnReturnClick += () => OnPageReturnClick?.Invoke(MinigamePage.Puzzle);
        }
        public event Action OnIndexReturnClick;
        public event Action<MinigamePage> OnPageReturnClick;
        public event Action<IndexMinigamePage.ButtonType> OnIndexButtonClick;
        public event Action<MinigamePage, int> OnItemClick;
        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField]
        private IndexMinigamePage indexUI;
        [SerializeField]
        private MinigameItemsPage minigamePage;
        [SerializeField]
        private MinigameItemsPage puzzlePage;
        public enum MinigamePage
        {
            Index,
            Minigame,
            Puzzle,
        }
    }
}
