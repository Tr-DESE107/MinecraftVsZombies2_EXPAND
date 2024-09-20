using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
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
