using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
