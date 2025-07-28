using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Mainmenu
{
    public class StatsUI : MonoBehaviour
    {
        public void UpdateStats(StatsViewData viewData)
        {
            playTimeText.text = viewData.playTimeText;
            categoryList.updateList(viewData.categories.Length, (i, obj) =>
            {
                var category = obj.GetComponent<StatCategoryUI>();
                category.UpdateCategory(viewData.categories[i]);
                category.SetExpanded(false);
            });
        }
        private void Awake()
        {
            backButton.onClick.AddListener(() => OnReturnClick?.Invoke());
        }
        public event Action OnReturnClick;
        [SerializeField]
        private TextMeshProUGUI playTimeText;
        [SerializeField]
        private ElementList categoryList;
        [SerializeField]
        private Button backButton;
    }
    public struct StatsViewData
    {
        public string playTimeText;
        public StatCategoryViewData[] categories;
    }
}
