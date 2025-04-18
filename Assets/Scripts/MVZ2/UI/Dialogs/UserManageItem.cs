﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Mainmenu.UI
{
    public class UserManageItem : MonoBehaviour
    {
        public void SetIsOn(bool value)
        {
            toggle.SetIsOnWithoutNotify(value);
        }
        public void SetName(string name)
        {
            nameText.text = name;
        }
        public void SetColor(Color color)
        {
            nameText.color = color;
        }
        private void Awake()
        {
            toggle.onValueChanged.AddListener((value) => OnValueChanged?.Invoke(this, value));
        }
        public event Action<UserManageItem, bool> OnValueChanged;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private Toggle toggle;
    }

}
