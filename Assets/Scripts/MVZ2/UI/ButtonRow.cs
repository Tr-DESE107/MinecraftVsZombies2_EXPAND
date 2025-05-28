﻿using System;
using UnityEngine;

namespace MVZ2.UI
{
    public class ButtonRow : MonoBehaviour
    {
        public void UpdateButtons(string[] buttonTexts)
        {
            buttonList.updateList(buttonTexts.Length, (i, obj) =>
            {
                var button = obj.GetComponent<TextButton>();
                button.Text.text = buttonTexts[i];
            },
            rect =>
            {
                var button = rect.GetComponent<TextButton>();
                button.Button.onClick.RemoveAllListeners();
                button.Button.onClick.AddListener(() => OnButtonClick?.Invoke(this, buttonList.indexOf(rect)));
            });
        }
        public event Action<ButtonRow, int> OnButtonClick;
        [SerializeField]
        private ElementList buttonList;
    }
}
