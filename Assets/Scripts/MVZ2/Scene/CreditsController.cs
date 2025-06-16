﻿using System.Collections.Generic;
using System.Linq;
using MVZ2.Managers;
using MVZ2.UI;
using MVZ2.Vanilla;
using UnityEngine;

namespace MVZ2.Scenes
{
    public class CreditsController : MonoBehaviour
    {
        #region 制作人员名单
        public void Display()
        {
            var viewDatas = new List<CreditsCategoryViewData>();
            var categories = main.ResourceManager.GetAllCreditsCategories();
            foreach (var category in categories)
            {
                var viewData = new CreditsCategoryViewData()
                {
                    name = main.LanguageManager._p(VanillaStrings.CONTEXT_CREDITS_CATEGORY, category.Name),
                    entries = category.Entries.Select(e => main.LanguageManager._p(VanillaStrings.CONTEXT_STAFF_NAME, e)).ToArray(),
                };
                viewDatas.Add(viewData);
            }
            ui.UpdateCredits(viewDatas.ToArray());
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        #endregion

        #region 生命周期
        private void Awake()
        {
            ui.OnBackButtonClick += OnCreditsReturnClickCallback;
        }
        #endregion

        #region 事件回调
        private void OnCreditsReturnClickCallback()
        {
            Hide();
        }
        #endregion
        private MainManager main => MainManager.Instance;
        [SerializeField]
        private CreditsPage ui;
    }
}
