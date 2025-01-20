using System;
using System.Linq;
using PVZEngine;
using TMPro;
using UnityEngine;

namespace MVZ2.Map
{
    public class MapModel : MonoBehaviour
    {
        public void SetEndlessFlagsTextActive(bool active)
        {
            if (!endlessFlagsText)
                return;
            endlessFlagsText.gameObject.SetActive(active);
        }
        public void SetEndlessFlagsText(string text)
        {
            if (!endlessFlagsText)
                return;
            endlessFlagsText.text = text;
        }
        public int GetMapButtonCount()
        {
            return mapButtons.Length;
        }
        public MapButton GetMapButton(int index)
        {
            if (index < 0 || index >= mapButtons.Length)
                return null;
            return mapButtons[index];
        }
        public void SetEndlessButtonInteractable(bool interactable)
        {
            if (!endlessButton)
                return;
            endlessButton.interactable = interactable;
        }
        public void SetEndlessButtonColor(Color color)
        {
            if (!endlessButton)
                return;
            endlessButton.SetColor(color);
        }
        public void SetEndlessButtonText(string text)
        {
            if (!endlessButton)
                return;
            endlessButton.SetText(text);
        }
        public void SetMapKeyActive(bool active)
        {
            if (mapKey)
                mapKey.gameObject.SetActive(active);
        }
        public void SetMapButtonInteractable(int index, bool interactable)
        {
            var button = GetMapButton(index);
            if (!button)
                return;
            button.interactable = interactable;
        }
        public void SetMapButtonColor(int index, Color color)
        {
            var button = GetMapButton(index);
            if (!button)
                return;
            button.SetColor(color);
        }
        public void SetMapButtonText(int index, string text)
        {
            var button = GetMapButton(index);
            if (!button)
                return;
            button.SetText(text);
        }
        public void SetMapButtonDifficulty(int index, NamespaceID difficulty)
        {
            var button = GetMapButton(index);
            if (!button)
                return;
            button.SetDifficulty(difficulty);
        }
        public NamespaceID[] GetMapElementUnlocks()
        {
            return mapElements.Select(e => e.unlock.Get()).ToArray();
        }
        public void SetMapElementUnlocked(NamespaceID unlock, bool unlocked)
        {
            var element = mapElements.FirstOrDefault(e => e.unlock.Get() == unlock);
            if (element == null)
                return;
            element.SetActive(unlocked);
        }
        private void Awake()
        {
            foreach (var button in mapButtons)
            {
                var index = Array.IndexOf(mapButtons, button);
                button.OnClick += () => OnMapButtonClick?.Invoke(index);
            }
            if (endlessButton)
            {
                endlessButton.OnClick += () => OnEndlessButtonClick?.Invoke();
            }
            if (mapKey)
            {
                mapKey.OnClick += () => OnMapKeyClick?.Invoke();
            }
            if (nightmareBox)
            {
                nightmareBox.OnClick += () => OnMapNightmareBoxClick?.Invoke();
            }
        }
        public event Action<int> OnMapButtonClick;
        public event Action OnEndlessButtonClick;
        public event Action OnMapKeyClick;
        public event Action OnMapNightmareBoxClick;
        [SerializeField]
        private TextMeshPro endlessFlagsText;
        [SerializeField]
        private MapButton endlessButton;
        [SerializeField]
        private MapElementButton mapKey;
        [SerializeField]
        private MapElementButton nightmareBox;
        [SerializeField]
        private MapButton[] mapButtons;
        [SerializeField]
        private MapElement[] mapElements;
    }
}
