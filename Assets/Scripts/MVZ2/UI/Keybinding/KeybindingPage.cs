﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class KeybindingPage : MonoBehaviour
    {
        public void UpdateItems(KeybindingItemViewData[] viewData)
        {
            items.updateList(viewData.Length, (i, obj) =>
            {
                var data = viewData[i];
                var item = obj.GetComponent<KeybindingItem>();
                item.UpdateItem(data);
            },
            obj =>
            {
                var item = obj.GetComponent<KeybindingItem>();
                item.OnKeyButtonClick += OnItemButtonClickCallback;
            },
            obj =>
            {
                var item = obj.GetComponent<KeybindingItem>();
                item.OnKeyButtonClick -= OnItemButtonClickCallback;
            });
        }
        public void UpdateItem(int index, KeybindingItemViewData viewData)
        {
            var item = items.getElement<KeybindingItem>(index);
            item.UpdateItem(viewData);
        }
        private void Awake()
        {
            backButton.onClick.AddListener(() => OnBackButtonClick?.Invoke());
            resetButton.onClick.AddListener(() => OnResetButtonClick?.Invoke());
        }
        private void OnItemButtonClickCallback(KeybindingItem item)
        {
            OnItemButtonClick?.Invoke(items.indexOf(item));
        }
        public event Action OnBackButtonClick;
        public event Action OnResetButtonClick;
        public event Action<int> OnItemButtonClick;


        [SerializeField]
        private ElementList items;
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private Button resetButton;
    }

}
