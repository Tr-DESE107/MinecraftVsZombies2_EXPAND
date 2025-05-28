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
        public void SetEntries(ChoosingBlueprintViewData[] entries, bool commandBlockVisible, ChoosingBlueprintViewData commandBlockViewData)
        {
            blueprintDisplayer.UpdateItems(entries);
            commandBlockSlot.SetCommandBlockActive(commandBlockVisible);
            commandBlockSlot.UpdateCommandBlockItem(commandBlockViewData);
        }
        public void SetActiveEntry(Model prefab, Camera camera, string name, string description, string cost, string recharge)
        {
            entryModel.ChangeModel(prefab, camera);
            nameText.text = name;
            descriptionText.text = description;
            descriptionScrollRect.verticalNormalizedPosition = 1;
            costText.text = cost;
            rechargeText.text = recharge;
        }
        protected override void Awake()
        {
            base.Awake();
            blueprintDisplayer.OnBlueprintSelect += OnEntryClickCallback;
            commandBlockSlot.OnClick += OnCommandBlockClickCallback;
        }
        private void OnEntryClickCallback(int index, PointerEventData eventData)
        {
            OnEntryClick?.Invoke(index);
        }
        private void OnCommandBlockClickCallback()
        {
            OnCommandBlockClick?.Invoke();
        }
        public Action<int> OnEntryClick;
        public Action OnCommandBlockClick;
        [SerializeField]
        private BlueprintDisplayer blueprintDisplayer;
        [SerializeField]
        CommandBlockSlot commandBlockSlot;
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
