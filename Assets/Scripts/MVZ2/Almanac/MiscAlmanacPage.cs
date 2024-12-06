using System;
using MVZ2.Models;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Almanacs
{
    public class MiscAlmanacPage : AlmanacPage
    {
        public void SetEntries(AlmanacEntryViewData[] entries)
        {
            entryList.updateList(entries.Length, (i, obj) =>
            {
                var entry = obj.GetComponent<AlmanacEntry>();
                entry.UpdateEntry(entries[i]);
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
        public void SetActiveEntry(Sprite image, string name, string description)
        {
            entryImage.sprite = image;
            nameText.text = name;
            descriptionText.text = description;
        }
        public void SetActiveEntry(Model prefab, string name, string description)
        {
            entryModel.ChangeModel(prefab);
            nameText.text = name;
            descriptionText.text = description;
            descriptionScrollRect.verticalNormalizedPosition = 1;
        }
        private void OnEntryClickCallback(AlmanacEntry entry)
        {
            OnEntryClick?.Invoke(entryList.indexOf(entry));
        }
        public Action<int> OnEntryClick;
        [SerializeField]
        private ElementListUI entryList;
        [SerializeField]
        private AlmanacModel entryModel;
        [SerializeField]
        private ScrollRect descriptionScrollRect;
        [SerializeField]
        private Image entryImage;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
    }
}
