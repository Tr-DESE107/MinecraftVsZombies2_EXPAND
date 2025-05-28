﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Addons
{
    public class LanguagePackItem : MonoBehaviour
    {
        public void UpdateItem(LanguagePackViewData viewData)
        {
            nameText.text = viewData.name;
            descriptionText.text = viewData.description;
            icon.sprite = viewData.icon;
        }
        public void SetToggled(bool value)
        {
            toggle.SetIsOnWithoutNotify(value);
        }
        private void Awake()
        {
            toggle.onValueChanged.AddListener(v => OnToggled?.Invoke(this, v));
        }
        public event Action<LanguagePackItem, bool> OnToggled;
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        [SerializeField]
        private Image icon;
    }
    public struct LanguagePackViewData
    {
        public string name;
        public string description;
        public Sprite icon;
    }
}
