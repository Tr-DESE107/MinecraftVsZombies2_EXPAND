﻿using TMPro;
using UnityEngine;

namespace MVZ2.UI
{
    public class CreditsEntry : MonoBehaviour
    {
        public void UpdateEntry(string name)
        {
            nameText.text = name;
        }

        [SerializeField]
        private TextMeshProUGUI nameText;
    }

}
