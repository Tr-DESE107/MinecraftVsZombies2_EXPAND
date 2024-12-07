using System;
using MVZ2.Models;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Almanacs
{
    public class ContraptionAlmanacPage : AlmanacPage
    {
        public void SetEntries(ChoosingBlueprintViewData[] entries, bool commandBlockVisible)
        {
            blueprintDisplayer.UpdateItems(entries);
            blueprintDisplayer.SetCommandBlockActive(commandBlockVisible);
        }
        public void SetActiveEntry(Model prefab, string name, string description, string cost, string recharge)
        {
            entryModel.ChangeModel(prefab);
            nameText.text = name;
            descriptionText.text = description;
            descriptionScrollRect.verticalNormalizedPosition = 1;
            costText.text = cost;
            rechargeText.text = recharge;
        }
        protected override void Awake()
        {
            base.Awake();
            blueprintDisplayer.OnBlueprintPointerDown += OnEntryClickCallback;
        }
        private void OnEntryClickCallback(int index, PointerEventData eventData)
        {
            OnEntryClick?.Invoke(index);
        }
        public Action<int> OnEntryClick;
        [SerializeField]
        private BlueprintDisplayer blueprintDisplayer;
        [SerializeField]
        private AlmanacModel entryModel;
        [SerializeField]
        private ScrollRect descriptionScrollRect;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        [SerializeField]
        private TextMeshProUGUI costText;
        [SerializeField]
        private TextMeshProUGUI rechargeText;
    }
}
