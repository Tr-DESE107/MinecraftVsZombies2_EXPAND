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
        public void SetGroups(AlmanacEntryGroupViewData[] groups)
        {
            groupList.updateList(groups.Length, (i, obj) =>
            {
                var entry = obj.GetComponent<AlmanacEntryGroupUI>();
                entry.UpdateEntry(groups[i]);
            },
            obj =>
            {
                var entry = obj.GetComponent<AlmanacEntryGroupUI>();
                entry.OnEntryClick += OnGroupEntryClickCallback;
            },
            obj =>
            {
                var entry = obj.GetComponent<AlmanacEntryGroupUI>();
                entry.OnEntryClick -= OnGroupEntryClickCallback;
            });
        }
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
        public void SetActiveEntry(Sprite image, string name, string description, bool sized, bool zoom)
        {
            entryImageRegion.gameObject.SetActive(true);
            entryModel.gameObject.SetActive(false);
            if (sized)
            {
                entryImageFull.sprite = null;
                entryImageFull.enabled = false;
                entryImageSized.sprite = image;
                entryImageSized.enabled = image;
                if (image)
                {
                    entryImageSized.rectTransform.sizeDelta = image.rect.size;
                }
            }
            else
            {
                entryImageFull.sprite = image;
                entryImageFull.enabled = image;
                entryImageSized.sprite = null;
                entryImageSized.enabled = false;
                entryImageSized.rectTransform.sizeDelta = Vector2.zero;
            }
            iconZoomButtonRoot.SetActive(zoom);
            nameText.text = name;
            descriptionText.text = description;
        }
        public void SetActiveEntry(Model prefab, Camera camera, string name, string description)
        {
            entryImageRegion.gameObject.SetActive(false);
            entryModel.gameObject.SetActive(true);
            entryModel.ChangeModel(prefab, camera);
            nameText.text = name;
            descriptionText.text = description;
            descriptionScrollRect.verticalNormalizedPosition = 1;
        }
        protected override void Awake()
        {
            base.Awake();
            iconZoomButton.onClick.AddListener(() => OnZoomClick?.Invoke());
        }
        private void OnGroupEntryClickCallback(AlmanacEntryGroupUI group, int entryIndex)
        {
            OnGroupEntryClick?.Invoke(groupList.indexOf(group), entryIndex);
        }
        private void OnEntryClickCallback(AlmanacEntry entry)
        {
            OnEntryClick?.Invoke(entryList.indexOf(entry));
        }
        public Action<int, int> OnGroupEntryClick;
        public Action<int> OnEntryClick;
        public Action OnZoomClick;
        [SerializeField]
        private ElementList entryList;
        [SerializeField]
        private ElementList groupList;
        [SerializeField]
        private AlmanacModel entryModel;
        [SerializeField]
        private ScrollRect descriptionScrollRect;
        [SerializeField]
        private GameObject entryImageRegion;
        [SerializeField]
        private Image entryImageFull;
        [SerializeField]
        private Image entryImageSized;
        [SerializeField]
        private GameObject iconZoomButtonRoot;
        [SerializeField]
        private Button iconZoomButton;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
    }
}
