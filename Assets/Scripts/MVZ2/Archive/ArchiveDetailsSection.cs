using TMPro;
using UnityEngine;

namespace MVZ2.Archives
{
    public class ArchiveDetailsSection : MonoBehaviour
    {
        public void UpdateSection(ArchiveDetailsSectionViewData viewData)
        {
            descriptionText.text = viewData.description;
            talksText.text = viewData.talks;
        }
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        [SerializeField]
        private TextMeshProUGUI talksText;
    }
    public struct ArchiveDetailsSectionViewData
    {
        public string description;
        public string talks;
    }
}
