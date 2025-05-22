using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Minigames
{
    public class MinigameItemsPage : MonoBehaviour
    {
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            itemsScrollRect.verticalNormalizedPosition = 0;
        }
        protected virtual void Awake()
        {
            returnButton.onClick.AddListener(() => OnReturnClick?.Invoke());
        }
        public void SetItems(MinigameItemViewData[] items)
        {
            itemList.updateList(items.Length, (i, obj) =>
            {
                var entry = obj.GetComponent<MinigameItem>();
                entry.UpdateItem(items[i]);
            },
            obj =>
            {
                var entry = obj.GetComponent<MinigameItem>();
                entry.OnClick += OnEntryClickCallback;
            },
            obj =>
            {
                var entry = obj.GetComponent<MinigameItem>();
                entry.OnClick -= OnEntryClickCallback;
            });
        }
        private void OnEntryClickCallback(MinigameItem item)
        {
            OnEntryClick?.Invoke(itemList.indexOf(item));
        }
        public Action<int> OnEntryClick;
        public event Action OnReturnClick;
        [SerializeField]
        private Button returnButton;
        [SerializeField]
        private ElementList itemList;
        [SerializeField]
        private ScrollRect itemsScrollRect;
    }
}
