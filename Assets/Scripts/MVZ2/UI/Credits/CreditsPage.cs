﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class CreditsPage : MonoBehaviour
    {
        public void UpdateCredits(CreditsCategoryViewData[] viewData)
        {
            categories.updateList(viewData.Length, (i, obj) =>
            {
                var data = viewData[i];
                var category = obj.GetComponent<CreditsCategory>();
                category.UpdateCategory(data);
            });
        }
        private void Awake()
        {
            backButton.onClick.AddListener(() => OnBackButtonClick?.Invoke());
        }
        public event Action OnBackButtonClick;


        [SerializeField]
        private ElementList categories;
        [SerializeField]
        private Button backButton;
    }

}
