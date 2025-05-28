﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class KeybindingItem : MonoBehaviour
    {
        public void UpdateItem(KeybindingItemViewData viewData)
        {
            nameText.text = viewData.name;
            keyText.text = viewData.key;
            keyText.color = viewData.keyColor;
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnKeyButtonClick?.Invoke(this));
        }
        public event Action<KeybindingItem> OnKeyButtonClick;

        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI keyText;
    }
    public struct KeybindingItemViewData
    {
        public string name;
        public string key;
        public Color keyColor;
    }
}
