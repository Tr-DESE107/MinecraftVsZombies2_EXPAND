﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.MusicRoom
{
    public class MusicRoomListItem : MonoBehaviour
    {
        public void UpdateName(string name)
        {
            nameText.text = name;
        }
        public void SetSelected(bool selected)
        {
            toggle.SetIsOnWithoutNotify(selected);
        }
        private void Awake()
        {
            toggle.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    OnClick?.Invoke(this);
                }
            });
        }
        public event Action<MusicRoomListItem> OnClick;
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private TextMeshProUGUI nameText;
    }
}
