﻿using System.Linq;
using MVZ2.Level.UI;
using MVZ2Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class BlueprintDisplayerStandalone : BlueprintDisplayer
    {
        public void SetCurrentPage(int index)
        {
            currentPage = index;
            pageText.text = $"{index + 1}/{maxPages}";
            for (int i = 0; i < pageList.count; i++)
            {
                var element = pageList.getElement(i);
                element.gameObject.SetActive(i == index);
            }
            SetPageButtonInteractable(false, index > 0);
            SetPageButtonInteractable(true, index < maxPages - 1);
        }
        public void SetPageButtonInteractable(bool isNextPage, bool interactable)
        {
            var button = isNextPage ? nextPageButton : previousPageButton;
            button.interactable = interactable;
        }
        public override void UpdateItems(ChoosingBlueprintViewData[] viewDatas)
        {
            var pageCount = Mathf.CeilToInt(viewDatas.Length / (float)maxCountPerPage);
            pageList.updateList(pageCount, (i, rect) =>
            {
                var page = rect.GetComponent<BlueprintDisplayerStandalonePage>();
                page.UpdateItems(viewDatas.Skip(i * maxCountPerPage).Take(maxCountPerPage).ToArray());
            },
            rect =>
            {
                var page = rect.GetComponent<BlueprintDisplayerStandalonePage>();
                page.OnBlueprintPointerInteraction += OnBlueprintPointerInteractionCallback;
            },
            rect =>
            {
                var page = rect.GetComponent<BlueprintDisplayerStandalonePage>();
                page.OnBlueprintPointerInteraction -= OnBlueprintPointerInteractionCallback;
            });
            maxPages = pageCount;
            SetCurrentPage(0);
            pageRoot.SetActive(pageCount > 1);
        }
        public override Blueprint GetItem(int index)
        {
            var pageNum = Mathf.FloorToInt(index / (float)maxCountPerPage);
            var page = pageList.getElement<BlueprintDisplayerStandalonePage>(pageNum);
            if (page == null)
                return null;
            return page.GetItem(index % maxCountPerPage);
        }
        private void Awake()
        {
            previousPageButton.onClick.AddListener(() => SetCurrentPage(currentPage - 1));
            nextPageButton.onClick.AddListener(() => SetCurrentPage(currentPage + 1));
        }
        private void OnBlueprintPointerInteractionCallback(BlueprintDisplayerStandalonePage page, int indexInPage, PointerEventData eventData, PointerInteraction interaction)
        {
            var index = pageList.indexOf(page) * maxCountPerPage + indexInPage;
            CallBlueprintPointerInteraction(index, eventData, interaction);
            if (interaction == PointerInteraction.Down)
            {
                CallBlueprintSelect(index);
            }
        }
        [Header("Standalone")]
        [SerializeField]
        GameObject pageRoot;
        [SerializeField]
        TextMeshProUGUI pageText;
        [SerializeField]
        ElementListUI pageList;
        [SerializeField]
        Button previousPageButton;
        [SerializeField]
        Button nextPageButton;
        [SerializeField]
        int maxCountPerPage = 40;
        private int currentPage;
        private int maxPages;
    }
}
