﻿using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Mainmenu
{
    public class StatCategoryUI : MonoBehaviour
    {
        public void UpdateCategory(StatCategoryViewData viewData)
        {
            titleText.text = viewData.title;
            sumText.text = viewData.sum;
            entryList.updateList(viewData.entries.Length, (i, obj) =>
            {
                var entry = obj.GetComponent<StatEntryUI>();
                entry.UpdateEntry(viewData.entries[i]);
            });
        }
        public void SetExpanded(bool expanded)
        {
            expandArrowTransform.localEulerAngles = new Vector3(0, 0, expanded ? -90 : 0);
            entriesRoot.SetActive(expanded);
        }
        public bool IsExpanded()
        {
            return entriesRoot.activeSelf;
        }
        private void Awake()
        {
            expandArrowButton.onClick.AddListener(() => SetExpanded(!IsExpanded()));
        }
        [SerializeField]
        private Transform expandArrowTransform;
        [SerializeField]
        private Button expandArrowButton;
        [SerializeField]
        private GameObject entriesRoot;
        [SerializeField]
        private ElementList entryList;
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private TextMeshProUGUI sumText;
    }
    public struct StatCategoryViewData
    {
        public StatEntryViewData[] entries;
        public string title;
        public string sum;
    }
}
