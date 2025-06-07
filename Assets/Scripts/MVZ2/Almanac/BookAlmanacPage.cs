﻿using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Almanacs
{
    public abstract class BookAlmanacPage : AlmanacPage
    {
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
                tag.OnPointerDown += OnTagPointerDownCallback;
            },
            obj =>
            {
                var tag = obj.GetComponent<AlmanacTagIcon>();
                tag.OnPointerEnter -= OnTagPointerEnterCallback;
                tag.OnPointerExit -= OnTagPointerExitCallback;
                tag.OnPointerDown += OnTagPointerDownCallback;
            });
        }
        public void UpdateDescriptionIcons(AlmanacDescriptionTagViewData[] viewDatas)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(descriptionScrollRect.content);
            descriptionIconUpdater.UpdateIconStacks(viewDatas);
        }
        protected void SetDescription(string name, string description)
        {
            nameText.text = name;
            descriptionText.text = description;
            descriptionScrollRect.verticalNormalizedPosition = 1;
        }
        protected override void Awake()
        {
            base.Awake();
            descriptionIconUpdater.OnIconEnter += id => OnDescriptionIconEnter?.Invoke(id);
            descriptionIconUpdater.OnIconExit += id => OnDescriptionIconExit?.Invoke(id);
            descriptionIconUpdater.OnIconDown += id => OnDescriptionIconDown?.Invoke(id);
        }
        private void OnTagPointerEnterCallback(AlmanacTagIcon icon)
        {
            OnTagIconEnter?.Invoke(entryTags.indexOf(icon));
        }
        private void OnTagPointerExitCallback(AlmanacTagIcon icon)
        {
            OnTagIconExit?.Invoke(entryTags.indexOf(icon));
        }
        private void OnTagPointerDownCallback(AlmanacTagIcon icon)
        {
            OnTagIconDown?.Invoke(entryTags.indexOf(icon));
        }
        public event Action<string> OnDescriptionIconEnter;
        public event Action<string> OnDescriptionIconExit;
        public event Action<string> OnDescriptionIconDown;
        public event Action<int> OnTagIconEnter;
        public event Action<int> OnTagIconExit;
        public event Action<int> OnTagIconDown;
        [SerializeField]
        private ScrollRect descriptionScrollRect;
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        [SerializeField]
        private ElementList entryTags;
        [SerializeField]
        private AlmanacTaggedDescription descriptionIconUpdater;
    }
}
