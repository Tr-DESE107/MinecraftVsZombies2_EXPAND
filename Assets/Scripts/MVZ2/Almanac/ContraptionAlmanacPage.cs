using System;
using MVZ2.Models;
using MVZ2.UI;
using TMPro;
using UnityEngine;
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
        public AlmanacTagIcon GetTagIcon(int index)
        {
            return entryTags.getElement<AlmanacTagIcon>(index);
        }
        public AlmanacTagIcon GetDescriptionIcon(string linkID)
        {
            return descriptionIconUpdater.GetIconContainer(linkID);
        }
        public void UpdateTagIcons(AlmanacTagIconViewData[] viewDatas)
        {
            entryTags.updateList(viewDatas.Length, (i, obj) =>
            {
                var tag = obj.GetComponent<AlmanacTagIcon>();
                tag.UpdateContainer(viewDatas[i]);
            },
            obj =>
            {
                var tag = obj.GetComponent<AlmanacTagIcon>();
                tag.OnPointerEnter += OnTagPointerEnterCallback;
                tag.OnPointerExit += OnTagPointerExitCallback;
            },
            obj =>
            {
                var tag = obj.GetComponent<AlmanacTagIcon>();
                tag.OnPointerEnter -= OnTagPointerEnterCallback;
                tag.OnPointerExit -= OnTagPointerExitCallback;
            });
        }
        public void UpdateDescriptionIcons(AlmanacDescriptionTagViewData[] viewDatas)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(descriptionScrollRect.content);
            descriptionIconUpdater.UpdateIconStacks(viewDatas);
        }
        protected override void Awake()
        {
            base.Awake();
            blueprintDisplayer.OnBlueprintSelect += OnEntryClickCallback;
            commandBlockSlot.OnSelect += OnCommandBlockClickCallback;
            descriptionIconUpdater.OnIconEnter += id => OnDescriptionIconEnter?.Invoke(id);
            descriptionIconUpdater.OnIconExit += id => OnDescriptionIconExit?.Invoke(id);
        }
        private void OnEntryClickCallback(int index)
        {
            OnEntryClick?.Invoke(index);
        }
        private void OnCommandBlockClickCallback()
        {
            OnCommandBlockClick?.Invoke();
        }
        private void OnTagPointerEnterCallback(AlmanacTagIcon icon)
        {
            OnTagIconEnter?.Invoke(entryTags.indexOf(icon));
        }
        private void OnTagPointerExitCallback(AlmanacTagIcon icon)
        {
            OnTagIconExit?.Invoke(entryTags.indexOf(icon));
        }
        public event Action<string> OnDescriptionIconEnter;
        public event Action<string> OnDescriptionIconExit;
        public event Action<int> OnTagIconEnter;
        public event Action<int> OnTagIconExit;
        public event Action<int> OnEntryClick;
        public event Action OnCommandBlockClick;
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
        [SerializeField]
        private ElementList entryTags;
        [SerializeField]
        private AlmanacTaggedDescription descriptionIconUpdater;
    }
}
