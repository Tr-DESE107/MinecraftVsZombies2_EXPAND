using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Almanacs
{
    public class AlmanacEntryGroupUI : MonoBehaviour
    {
        public void UpdateEntry(AlmanacEntryGroupViewData viewData)
        {
            titleText.text = viewData.name;
            entryList.updateList(viewData.entries.Length, (i, obj) =>
            {
                var entry = obj.GetComponent<AlmanacEntry>();
                entry.UpdateEntry(viewData.entries[i]);
            },
            obj =>
            {
                var entry = obj.GetComponent<AlmanacEntry>();
                entry.OnClick += OnEntryClickCallback;
            },
            obj =>
            {
                var entry = obj.GetComponent<AlmanacEntry>();
                entry.OnClick -= OnEntryClickCallback;
            });
        }
        private void OnEntryClickCallback(AlmanacEntry entry)
        {
            OnEntryClick?.Invoke(this, entryList.indexOf(entry));
        }
        public Action<AlmanacEntryGroupUI, int> OnEntryClick;
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private ElementList entryList;
    }
    public struct AlmanacEntryGroupViewData
    {
        public string name;
        public AlmanacEntryViewData[] entries;
    }
}
