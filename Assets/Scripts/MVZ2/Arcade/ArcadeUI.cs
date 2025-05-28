using System;
using UnityEngine;

namespace MVZ2.Arcade
{
    public class ArcadeUI : MonoBehaviour
    {
        public void DisplayPage(ArcadePage page)
        {
            indexUI.SetActive(page == ArcadePage.Index);
            minigamePage.SetActive(page == ArcadePage.Minigame);
            puzzlePage.SetActive(page == ArcadePage.Puzzle);
        }
        public void SetAllInteractable(bool interactable)
        {
            canvasGroup.interactable = interactable;
        }
        public void SetMinigameItems(ArcadeItemViewData[] items)
        {
            minigamePage.SetItems(items);
        }
        public void SetPuzzleItems(ArcadeItemViewData[] items)
        {
            puzzlePage.SetItems(items);
        }

        private void Awake()
        {
            indexUI.OnReturnClick += () => OnIndexReturnClick?.Invoke();
            indexUI.OnButtonClick += type => OnIndexButtonClick?.Invoke(type);
            minigamePage.OnEntryClick += index => OnItemClick?.Invoke(ArcadePage.Minigame, index);
            minigamePage.OnReturnClick += () => OnPageReturnClick?.Invoke(ArcadePage.Minigame);
            puzzlePage.OnEntryClick += index => OnItemClick?.Invoke(ArcadePage.Puzzle, index);
            puzzlePage.OnReturnClick += () => OnPageReturnClick?.Invoke(ArcadePage.Puzzle);
        }
        public event Action OnIndexReturnClick;
        public event Action<ArcadePage> OnPageReturnClick;
        public event Action<IndexArcadePage.ButtonType> OnIndexButtonClick;
        public event Action<ArcadePage, int> OnItemClick;
        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField]
        private IndexArcadePage indexUI;
        [SerializeField]
        private ArcadeItemsPage minigamePage;
        [SerializeField]
        private ArcadeItemsPage puzzlePage;
        public enum ArcadePage
        {
            Index,
            Minigame,
            Puzzle,
        }
    }
}
