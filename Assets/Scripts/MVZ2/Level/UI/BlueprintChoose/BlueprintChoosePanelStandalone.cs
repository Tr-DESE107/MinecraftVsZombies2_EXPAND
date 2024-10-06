using System.Linq;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class BlueprintChoosePanelStandalone : BlueprintChoosePanel
    {
        public void SetCurrentPage(int index, int maxPages)
        {
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
        public override void UpdateItems(BlueprintViewData[] viewDatas)
        {
            var pageCount = Mathf.CeilToInt(viewDatas.Length / (float)maxCountPerPage);
            pageList.updateList(pageCount, (i, rect) =>
            {
                var page = rect.GetComponent<BlueprintChoosePanelPage>();
                page.UpdateItems(viewDatas.Skip(i * maxCountPerPage).Take(maxCountPerPage).ToArray());
            },
            rect =>
            {
                var page = rect.GetComponent<BlueprintChoosePanelPage>();
                page.OnBlueprintPointerEnter += OnBlueprintPointerEnterCallback;
                page.OnBlueprintPointerExit += OnBlueprintPointerExitCallback;
                page.OnBlueprintPointerDown += OnBlueprintPointerDownCallback;
            },
            rect =>
            {
                var page = rect.GetComponent<BlueprintChoosePanelPage>();
                page.OnBlueprintPointerEnter -= OnBlueprintPointerEnterCallback;
                page.OnBlueprintPointerExit -= OnBlueprintPointerExitCallback;
                page.OnBlueprintPointerDown -= OnBlueprintPointerDownCallback;
            });
            SetCurrentPage(0, pageCount);
        }
        private void OnBlueprintPointerEnterCallback(BlueprintChoosePanelPage page, int indexInPage, PointerEventData eventData)
        {
            CallBlueprintPointerEnter(pageList.indexOf(page) * maxCountPerPage + indexInPage, eventData);
        }
        private void OnBlueprintPointerExitCallback(BlueprintChoosePanelPage page, int indexInPage, PointerEventData eventData)
        {
            CallBlueprintPointerExit(pageList.indexOf(page) * maxCountPerPage + indexInPage, eventData);
        }
        private void OnBlueprintPointerDownCallback(BlueprintChoosePanelPage page, int indexInPage, PointerEventData eventData)
        {
            CallBlueprintPointerDown(pageList.indexOf(page) * maxCountPerPage + indexInPage, eventData);
        }
        [Header("Standalone")]
        [SerializeField]
        TextMeshProUGUI pageText;
        [SerializeField]
        ElementList pageList;
        [SerializeField]
        Button previousPageButton;
        [SerializeField]
        Button nextPageButton;
        [SerializeField]
        int maxCountPerPage = 40;
    }
}
