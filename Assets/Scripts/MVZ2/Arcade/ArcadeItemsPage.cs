using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Arcade
{
    public class ArcadeItemsPage : MonoBehaviour
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
        public void SetItems(ArcadeItemViewData[] items)
        {
            itemList.updateList(items.Length, (i, obj) =>
            {
                var entry = obj.GetComponent<ArcadeItem>();
                entry.UpdateItem(items[i]);
            },
            obj =>
            {
                var entry = obj.GetComponent<ArcadeItem>();
                entry.OnClick += OnEntryClickCallback;
            },
            obj =>
            {
                var entry = obj.GetComponent<ArcadeItem>();
                entry.OnClick -= OnEntryClickCallback;
            });
        }
        private void OnEntryClickCallback(ArcadeItem item)
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
