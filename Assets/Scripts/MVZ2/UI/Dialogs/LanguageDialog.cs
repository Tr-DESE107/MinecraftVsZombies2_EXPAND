﻿using System;
using System.Linq;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Mainmenu.UI
{
    public class LanguageDialog : Dialog
    {
        public void SetLanguages(string[] languages)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(languages.ToList());
        }
        private void Awake()
        {
            confirmButton.onClick.AddListener(() => OnConfirm?.Invoke(dropdown.value));
        }
        public event Action<int> OnConfirm;

        [SerializeField]
        private TMP_Dropdown dropdown;
        [SerializeField]
        private Button confirmButton;
    }
}
