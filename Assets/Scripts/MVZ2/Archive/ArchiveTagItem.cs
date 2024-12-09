using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Archives
{
    public class ArchiveTagItem : MonoBehaviour
    {
        public void UpdateTag(ArchiveTagViewData tag)
        {
            tagText.text = tag.name;
            toggle.SetIsOnWithoutNotify(tag.value);
        }
        private void Awake()
        {
            toggle.onValueChanged.AddListener(value => OnValueChanged?.Invoke(this, value));
        }
        public event Action<ArchiveTagItem, bool> OnValueChanged;
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private TextMeshProUGUI tagText;
    }
    public struct ArchiveTagViewData
    {
        public string name;
        public bool value;
    }
}
