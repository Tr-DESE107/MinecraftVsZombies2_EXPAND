using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class CreditsCategory : MonoBehaviour
    {
        public void UpdateCategory(CreditsCategoryViewData viewData)
        {
            nameText.text = viewData.name;
            entries.updateList(viewData.entries.Length, (i, obj) =>
            {
                var data = viewData.entries[i];
                var entry = obj.GetComponent<CreditsEntry>();
                entry.UpdateEntry(data);
            });
        }

        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private ElementList entries;
    }
    public struct CreditsCategoryViewData
    {
        public string name;
        public string[] entries;
    }
}
