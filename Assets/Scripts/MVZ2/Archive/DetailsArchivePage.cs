using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Archives
{
    public class DetailsArchivePage : ArchivePage
    {
        public void UpdateDetails(ArchiveDetailsViewData viewData)
        {
            nameText.text = viewData.name;
            backgroundImage.sprite = viewData.background;
            segmentsText.text = viewData.segments;
            musicText.text = viewData.music;
            tagsText.text = viewData.tags;
            sectionList.updateList(viewData.sections.Length, (i, obj) =>
            {
                var section = obj.GetComponent<ArchiveDetailsSection>();
                section.UpdateSection(viewData.sections[i]);
            });
            descriptionScrollRect.verticalNormalizedPosition = 1;
        }
        private void Awake()
        {
            playButton.onClick.AddListener(() => OnPlayClick?.Invoke());
            returnButton.onClick.AddListener(() => OnReturnClick?.Invoke());
        }
        public Action OnReturnClick;
        public Action OnPlayClick;
        [SerializeField]
        private Button returnButton;
        [Header("General")]
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private TextMeshProUGUI segmentsText;
        [SerializeField]
        private TextMeshProUGUI musicText;
        [SerializeField]
        private TextMeshProUGUI tagsText;
        [SerializeField]
        private Button playButton;
        [Header("Sections")]
        [SerializeField]
        private ScrollRect descriptionScrollRect;
        [SerializeField]
        private ElementList sectionList;
    }
    public struct ArchiveDetailsViewData
    {
        public string name;
        public Sprite background;
        public string segments;
        public string music;
        public string tags;
        public ArchiveDetailsSectionViewData[] sections;
    }
}
