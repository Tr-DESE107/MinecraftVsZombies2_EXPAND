using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class DebugConsoleAutoCompleteItem : MonoBehaviour
    {
        public void SetText(string text)
        {
            this.text.text = text;
        }
        public void SetIsOn(bool isOn)
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }
        private void Awake()
        {
            toggle.onValueChanged.AddListener((v) => OnValueChanged?.Invoke(this, v));
        }
        public event Action<DebugConsoleAutoCompleteItem, bool> OnValueChanged;
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private TextMeshProUGUI text;
    }
}
