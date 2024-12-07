using MVZ2.UI;
using TMPro;
using UnityEngine;

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
