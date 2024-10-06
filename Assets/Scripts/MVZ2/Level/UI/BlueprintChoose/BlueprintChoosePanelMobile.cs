using System.Linq;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class BlueprintChoosePanelMobile : BlueprintChoosePanel
    {
        public override void UpdateItems(BlueprintViewData[] viewDatas)
        {
            var groups = viewDatas
                .Select((v, i) => (v, i))
                .GroupBy(p => p.i / countPerRow);
            var validViewDatas = groups
                .Where(g => !g.All(p => p.v.empty))
                .SelectMany(g => g.Select(p => p.v))
                .ToArray();
            blueprintList.updateList(viewDatas.Length, (i, rect) =>
            {
                var page = rect.GetComponent<Blueprint>();
                page.UpdateView(validViewDatas[i]);
            },
            rect =>
            {
                var page = rect.GetComponent<Blueprint>();
                page.OnPointerEnter += OnBlueprintPointerEnterCallback;
                page.OnPointerExit += OnBlueprintPointerExitCallback;
                page.OnPointerDown += OnBlueprintPointerDownCallback;
            },
            rect =>
            {
                var page = rect.GetComponent<Blueprint>();
                page.OnPointerEnter -= OnBlueprintPointerEnterCallback;
                page.OnPointerExit -= OnBlueprintPointerExitCallback;
                page.OnPointerDown -= OnBlueprintPointerDownCallback;
            });
        }
        private void OnBlueprintPointerEnterCallback(Blueprint blueprint, PointerEventData eventData)
        {
            CallBlueprintPointerEnter(blueprintList.indexOf(blueprint), eventData);
        }
        private void OnBlueprintPointerExitCallback(Blueprint blueprint, PointerEventData eventData)
        {
            CallBlueprintPointerExit(blueprintList.indexOf(blueprint), eventData);
        }
        private void OnBlueprintPointerDownCallback(Blueprint blueprint, PointerEventData eventData)
        {
            CallBlueprintPointerDown(blueprintList.indexOf(blueprint), eventData);
        }
        [Header("Mobile")]
        [SerializeField]
        ElementList blueprintList;
        [SerializeField]
        int countPerRow = 4;
    }
}
