﻿using System.Collections.Generic;
using MVZ2.Metas;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 元数据列表
        public CreditMetaList GetCreditsMetaList(string nsp)
        {
            var modResource = main.ResourceManager.GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.CreditsMetaList;
        }
        #endregion

        #region 分类
        public CreditsCategoryMeta[] GetAllCreditsCategories()
        {
            List<CreditsCategoryMeta> categories = new List<CreditsCategoryMeta>();
            foreach (var modResource in modResources)
            {
                var creditsMetaList = modResource?.CreditsMetaList;
                if (creditsMetaList == null)
                    continue;
                categories.AddRange(creditsMetaList.categories);
            }
            return categories.ToArray();
        }
        #endregion
    }
}
