using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public abstract class BlueprintChoosePanel : MonoBehaviour
    {
        public void UpdateElements(BlueprintChoosePanelViewData viewData)
        {
            viewLawnButton.gameObject.SetActive(viewData.canViewLawn);
            commandBlockRoot.SetActive(viewData.hasCommandBlock);
            artifactRoot.SetActive(viewData.hasArtifacts);
        }
        public abstract void UpdateItems(ChoosingBlueprintViewData[] viewDatas);
        public abstract Blueprint GetItem(int index);
        public void ResetArtifactCount(int count)
        {
            artifactList.updateList(count, (i ,rect) =>
            {
                var artifactIcon = rect.GetComponent<ArtifactSlot>();
                artifactIcon.ResetView();
            }, 
            rect =>
            {
                var artifactIcon = rect.GetComponent<ArtifactSlot>();
                artifactIcon.OnClick += OnArtifactIconClickCallback;
            }, 
            rect =>
            {
                var artifactIcon = rect.GetComponent<ArtifactSlot>();
                artifactIcon.OnClick -= OnArtifactIconClickCallback;
            });
        }
        public void UpdateArtifactAt(int index, ArtifactViewData viewData)
        {
            var element = artifactList.getElement<ArtifactSlot>(index);
            if (!element)
                return;
            element.UpdateView(viewData);
        }
        private void Awake()
        {
            startButton.onClick.AddListener(() => OnStartButtonClick?.Invoke());
            viewLawnButton.onClick.AddListener(() => OnViewLawnButtonClick?.Invoke());
            commandBlockBlueprint.OnPointerDown += (blueprint, eventData) => OnCommandBlockBlueprintClick?.Invoke();
        }
        private void OnArtifactIconClickCallback(ArtifactSlot icon)
        {
            OnArtifactClick?.Invoke(artifactList.indexOf(icon));
        }
        protected void CallBlueprintPointerEnter(int index, PointerEventData eventData)
        {
            OnBlueprintPointerEnter?.Invoke(index, eventData);
        }
        protected void CallBlueprintPointerExit(int index, PointerEventData eventData)
        {
            OnBlueprintPointerExit?.Invoke(index, eventData);
        }
        protected void CallBlueprintPointerDown(int index, PointerEventData eventData)
        {
            OnBlueprintPointerDown?.Invoke(index, eventData);
        }
        public event Action OnStartButtonClick;
        public event Action OnViewLawnButtonClick;
        public event Action OnCommandBlockBlueprintClick;
        public event Action<int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintPointerDown;
        public event Action<int> OnArtifactClick;
        [SerializeField]
        Button startButton;
        [SerializeField]
        Button viewLawnButton;
        [SerializeField]
        GameObject commandBlockRoot;
        [SerializeField]
        Blueprint commandBlockBlueprint;
        [SerializeField]
        GameObject artifactRoot;
        [SerializeField]
        ElementListUI artifactList;
    }
    public struct BlueprintChoosePanelViewData
    {
        public bool canViewLawn;
        public bool hasCommandBlock;
        public bool hasArtifacts;
    }
    public struct ChoosingBlueprintViewData
    {
        public BlueprintViewData blueprint;
        public bool disabled;
        public bool selected;

        public static readonly ChoosingBlueprintViewData Empty = new ChoosingBlueprintViewData()
        {
            blueprint = BlueprintViewData.Empty
        };
    }
}
