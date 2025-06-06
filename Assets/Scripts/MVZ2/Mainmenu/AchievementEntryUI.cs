﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Mainmenu
{
    public class AchievementEntryUI : MonoBehaviour
    {
        public void UpdateEntry(AchievementEntryViewData viewData)
        {
            iconImage.sprite = viewData.icon;
            notEarnedIconImage.sprite = viewData.icon;
            iconImage.enabled = viewData.earned;
            notEarnedIconImage.enabled = !viewData.earned;
            nameText.text = viewData.name;
            earnedObj.SetActive(viewData.earned);
            descriptionText.text = viewData.description;
        }
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private Image notEarnedIconImage;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private GameObject earnedObj;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
    }
    public struct AchievementEntryViewData
    {
        public Sprite icon;
        public string name;
        public bool earned;
        public string description;
    }
}
