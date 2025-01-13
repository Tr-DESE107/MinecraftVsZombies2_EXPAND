using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Mainmenu
{
    public class StatsUI : MonoBehaviour
    {
        public void UpdateStats(StatCategoryViewData[] categories)
        {
            categoryList.updateList(categories.Length, (i, obj) =>
            {
                var category = obj.GetComponent<StatCategoryUI>();
                category.UpdateCategory(categories[i]);
                category.SetExpanded(false);
            });
        }
        private void Awake()
        {
            backButton.onClick.AddListener(() => OnReturnClick?.Invoke());
        }
        public event Action OnReturnClick;
        [SerializeField]
        private ElementList categoryList;
        [SerializeField]
        private Button backButton;
    }
}
