using System;
using System.Linq;
using MVZ2Logic.Models;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Map
{
    public class MapModel : MonoBehaviour, IMapModel
    {
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
            endlessButton.interactable = interactable;
        }
        public void SetEndlessButtonColor(Color color)
        {
            endlessButton.SetColor(color);
        }
        public void SetEndlessButtonText(string text)
        {
            endlessButton.SetText(text);
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
            return mapElements.Select(e => e.unlock).ToArray();
        }
        public void SetMapElementUnlocked(NamespaceID unlock, bool unlocked)
        {
            var element = mapElements.FirstOrDefault(e => e.unlock == unlock);
            if (element == null)
                return;
            element.gameObject.SetActive(unlocked);
        }
        private void Awake()
        {
            foreach (var button in mapButtons)
            {
                var index = Array.IndexOf(mapButtons, button);
                button.OnClick += () => OnMapButtonClick?.Invoke(index);
            }
            endlessButton.OnClick += () => OnEndlessButtonClick?.Invoke();
        }
        public event Action<int> OnMapButtonClick;
        public event Action OnEndlessButtonClick;
        [SerializeField]
        private MapButton endlessButton;
        [SerializeField]
        private MapButton[] mapButtons;
        [SerializeField]
        private MapElement[] mapElements;
    }
    [Serializable]
    public class MapElement
    {
        public NamespaceID unlock;
        public GameObject gameObject;
    }
}
