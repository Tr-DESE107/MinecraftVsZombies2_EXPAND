using System.Linq;
using MVZ2.Level.UI;
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
                page.OnBlueprintPointerEnter += OnBlueprintPointerEnterCallback;
                page.OnBlueprintPointerExit += OnBlueprintPointerExitCallback;
                page.OnBlueprintPointerDown += OnBlueprintPointerDownCallback;
            },
            rect =>
            {
                var page = rect.GetComponent<BlueprintDisplayerStandalonePage>();
                page.OnBlueprintPointerEnter -= OnBlueprintPointerEnterCallback;
                page.OnBlueprintPointerExit -= OnBlueprintPointerExitCallback;
                page.OnBlueprintPointerDown -= OnBlueprintPointerDownCallback;
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
        private void OnBlueprintPointerEnterCallback(BlueprintDisplayerStandalonePage page, int indexInPage, PointerEventData eventData)
        {
            CallBlueprintPointerEnter(pageList.indexOf(page) * maxCountPerPage + indexInPage, eventData);
        }
        private void OnBlueprintPointerExitCallback(BlueprintDisplayerStandalonePage page, int indexInPage, PointerEventData eventData)
        {
            CallBlueprintPointerExit(pageList.indexOf(page) * maxCountPerPage + indexInPage, eventData);
        }
        private void OnBlueprintPointerDownCallback(BlueprintDisplayerStandalonePage page, int indexInPage, PointerEventData eventData)
        {
            CallBlueprintPointerDown(pageList.indexOf(page) * maxCountPerPage + indexInPage, eventData);
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
