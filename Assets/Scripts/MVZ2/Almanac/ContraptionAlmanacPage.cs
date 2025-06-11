using System;
using MVZ2.Models;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Almanacs
{
    public class ContraptionAlmanacPage : BookAlmanacPage
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
            SetDescription(name, description);
            costText.text = cost;
            rechargeText.text = recharge;
        }
        protected override void Awake()
        {
            base.Awake();
            blueprintDisplayer.OnBlueprintSelect += OnEntryClickCallback;
            commandBlockSlot.OnSelect += OnCommandBlockClickCallback;
        }
        private void OnEntryClickCallback(int index, PointerEventData eventData)
        {
            OnEntryClick?.Invoke(index, eventData);
        }
        private void OnCommandBlockClickCallback(PointerEventData eventData)
        {
            OnCommandBlockClick?.Invoke(eventData);
        }
        public event Action<int, PointerEventData> OnEntryClick;
        public event Action<PointerEventData> OnCommandBlockClick;
        [SerializeField]
        private BlueprintDisplayer blueprintDisplayer;
        [SerializeField]
        CommandBlockSlot commandBlockSlot;
        [SerializeField]
        private AlmanacModel entryModel;
        [SerializeField]
        private TextMeshProUGUI costText;
        [SerializeField]
        private TextMeshProUGUI rechargeText;
    }
}
