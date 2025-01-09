using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Level;
using MVZ2.Level.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class LevelUIBlueprintChoose : MonoBehaviour, ILevelBlueprintChooseUI
    {
        #region 移动蓝图
        public MovingBlueprint CreateMovingBlueprint()
        {
            return movingBlueprints.CreateMovingBlueprint();
        }
        public void RemoveMovingBlueprint(MovingBlueprint blueprint)
        {
            movingBlueprints.RemoveMovingBlueprint(blueprint);
        }
        #endregion

        #region 选择蓝图
        public void SetViewLawnReturnBlockerActive(bool active)
        {
            viewLawnReturnBlocker.SetActive(active);
        }
        public void SetSideUIDisplaying(bool displaying)
        {
            sideUIDisplaying = displaying;
        }
        public void SetBlueprintChooseDisplaying(bool displaying)
        {
            blueprintChooseDisplaying = displaying;
        }
        public void SetSideUIBlend(float blend)
        {
            sideUIBlend = blend;
            animator.SetFloat("SideUIBlend", sideUIBlend);
        }
        public void SetBlueprintChooseBlend(float blend)
        {
            blueprintChooseBlend = blend;
            animator.SetFloat("BlueprintChooseBlend", blend);
        }
        public void SetBlueprintChooseViewAlmanacButtonActive(bool active)
        {
            choosingViewAlmanacButton.gameObject.SetActive(active);
        }
        public void SetBlueprintChooseViewStoreButtonActive(bool active)
        {
            choosingViewStoreButton.gameObject.SetActive(active);
        }
        public void UpdateBlueprintChooseElements(BlueprintChoosePanelViewData viewData)
        {
            blueprintChoosePanel.UpdateElements(viewData);
        }
        public void UpdateBlueprintChooseItems(ChoosingBlueprintViewData[] viewDatas)
        {
            blueprintChoosePanel.UpdateItems(viewDatas);
        }
        public Blueprint GetBlueprintChooseItem(int index)
        {
            return blueprintChoosePanel.GetItem(index);
        }
        #endregion

        #region 已选蓝图
        public Blueprint CreateChosenBlueprint()
        {
            return chosenBlueprints.CreateBlueprint();
        }
        public void AddChosenBlueprint(Blueprint blueprint)
        {
            chosenBlueprints.AddBlueprint(blueprint);
        }
        public void InsertChosenBlueprint(int index, Blueprint blueprint)
        {
            chosenBlueprints.InsertBlueprint(index, blueprint);
        }
        public bool RemoveChosenBlueprint(Blueprint blueprint)
        {
            return chosenBlueprints.RemoveBlueprint(blueprint);
        }
        public void RemoveChosenBlueprintAt(int index)
        {
            chosenBlueprints.RemoveBlueprintAt(index);
        }
        public bool DestroyChosenBlueprint(Blueprint blueprint)
        {
            return chosenBlueprints.DestroyBlueprint(blueprint);
        }
        public void DestroyChosenBlueprintAt(int index)
        {
            chosenBlueprints.DestroyBlueprintAt(index);
        }
        public Blueprint GetChosenBlueprintAt(int index)
        {
            return chosenBlueprints.GetBlueprintAt(index);
        }
        public int GetChosenBlueprintIndex(Blueprint blueprint)
        {
            return chosenBlueprints.GetBlueprintIndex(blueprint);
        }
        public Vector3 GetChosenBlueprintPosition(int index)
        {
            return chosenBlueprints.GetBlueprintPosition(index);
        }
        #endregion

        #region 选择制品
        public void SetArtifactSlotsActive(bool visible)
        {
            artifactSlotsRoot.SetActive(visible);
        }
        public ArtifactSlot GetArtifactSlotAt(int index)
        {
            return artifactSlotList.getElement<ArtifactSlot>(index);
        }
        public void ShowArtifactChoosePanel(ArtifactSelectItemViewData[] viewDatas)
        {
            artifactChoosingDialogObj.SetActive(true);
            artifactChoosingDialog.UpdateArtifacts(viewDatas);
        }
        public void HideArtifactChoosePanel()
        {
            artifactChoosingDialogObj.SetActive(false);
        }
        public ArtifactSelectItem GetArtifactSelectItem(int index)
        {
            return artifactChoosingDialog.GetArtifactSelectItem(index);
        }
        public void ResetArtifactSlotCount(int count)
        {
            artifactSlotList.updateList(count, (i, rect) =>
            {
                var artifactIcon = rect.GetComponent<ArtifactSlot>();
                artifactIcon.ResetView();
            },
            rect =>
            {
                var artifactIcon = rect.GetComponent<ArtifactSlot>();
                artifactIcon.OnClick += OnBlueprintChooseArtifactSlotClickCallback;
                artifactIcon.OnPointerEnter += OnBlueprintChooseArtifactSlotPointerEnterCallback;
                artifactIcon.OnPointerExit += OnBlueprintChooseArtifactSlotPointerExitCallback;
            },
            rect =>
            {
                var artifactIcon = rect.GetComponent<ArtifactSlot>();
                artifactIcon.OnClick -= OnBlueprintChooseArtifactSlotClickCallback;
                artifactIcon.OnPointerEnter -= OnBlueprintChooseArtifactSlotPointerEnterCallback;
                artifactIcon.OnPointerExit -= OnBlueprintChooseArtifactSlotPointerExitCallback;
            });
        }
        public void UpdateArtifactSlotAt(int index, ArtifactViewData viewData)
        {
            var element = artifactSlotList.getElement<ArtifactSlot>(index);
            if (!element)
                return;
            element.UpdateView(viewData);
        }
        #endregion

        private void Awake()
        {
            viewLawnReturnButton.onClick.AddListener(() => OnViewLawnReturnClick?.Invoke());

            artifactChoosingDialog.OnItemClicked += (index) => OnArtifactChoosingItemClicked?.Invoke(index);
            artifactChoosingDialog.OnItemPointerEnter += (index) => OnArtifactChoosingItemEnter?.Invoke(index);
            artifactChoosingDialog.OnItemPointerExit += (index) => OnArtifactChoosingItemExit?.Invoke(index);
            artifactChoosingDialog.OnBackButtonClicked += () => OnArtifactChoosingBackClicked?.Invoke();

            blueprintChoosePanel.OnStartButtonClick += () => OnStartClick?.Invoke();
            blueprintChoosePanel.OnViewLawnButtonClick += () => OnViewLawnClick?.Invoke();
            blueprintChoosePanel.OnCommandBlockBlueprintClick += () => OnCommandBlockClick?.Invoke();

            blueprintChoosePanel.OnBlueprintPointerEnter += (index, data) => OnBlueprintItemPointerEnter?.Invoke(index, data);
            blueprintChoosePanel.OnBlueprintPointerExit += (index, data) => OnBlueprintItemPointerExit?.Invoke(index, data);
            blueprintChoosePanel.OnBlueprintPointerDown += (index, data) => OnBlueprintItemPointerDown?.Invoke(index, data);

            choosingViewAlmanacButton.onClick.AddListener(() => OnViewAlmanacClick?.Invoke());
            choosingViewStoreButton.onClick.AddListener(() => OnViewStoreClick?.Invoke());
        }
        public void UpdateFrame(float deltaTime)
        {
            float targetSideUIBlend = sideUIDisplaying ? 1 : 0;
            float targetBlueprintChooseBlend = blueprintChooseDisplaying ? 1 : 0;
            const float blendSpeed = 10;
            float sideUIBlendAddition = (targetSideUIBlend - sideUIBlend) * blendSpeed * deltaTime;
            float blueprintChooseAddition = (targetBlueprintChooseBlend - blueprintChooseBlend) * blendSpeed * deltaTime;
            SetSideUIBlend(sideUIBlend + sideUIBlendAddition);
            SetBlueprintChooseBlend(blueprintChooseBlend + blueprintChooseAddition);
        }

        #region 事件回调
        private void OnBlueprintChooseArtifactSlotClickCallback(ArtifactSlot icon)
        {
            OnArtifactSlotClick?.Invoke(artifactSlotList.indexOf(icon));
        }
        private void OnBlueprintChooseArtifactSlotPointerEnterCallback(ArtifactSlot icon)
        {
            OnArtifactSlotPointerEnter?.Invoke(artifactSlotList.indexOf(icon));
        }
        private void OnBlueprintChooseArtifactSlotPointerExitCallback(ArtifactSlot icon)
        {
            OnArtifactSlotPointerExit?.Invoke(artifactSlotList.indexOf(icon));
        }
        #endregion

        #region 事件
        public event Action OnStartClick;
        public event Action OnViewLawnClick;
        public event Action OnCommandBlockClick;
        public event Action<int> OnArtifactSlotClick;
        public event Action<int> OnArtifactSlotPointerEnter;
        public event Action<int> OnArtifactSlotPointerExit;
        public event Action<int, PointerEventData> OnBlueprintItemPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintItemPointerExit;
        public event Action<int, PointerEventData> OnBlueprintItemPointerDown;

        public event Action OnViewAlmanacClick;
        public event Action OnViewStoreClick;
        public event Action OnViewLawnReturnClick;

        public event Action<int> OnArtifactChoosingItemClicked;
        public event Action<int> OnArtifactChoosingItemEnter;
        public event Action<int> OnArtifactChoosingItemExit;
        public event Action OnArtifactChoosingBackClicked;
        #endregion

        float sideUIBlend = 1;
        float blueprintChooseBlend;
        [SerializeField]
        Animator animator;
        [Header("Blueprint Choose")]
        [SerializeField]
        BlueprintList chosenBlueprints;
        [SerializeField]
        Button viewLawnReturnButton;
        [SerializeField]
        GameObject viewLawnReturnBlocker;
        [SerializeField]
        GameObject artifactChoosingDialogObj;
        [SerializeField]
        ArtifactChoosingDialog artifactChoosingDialog;
        [SerializeField]
        MovingBlueprintList movingBlueprints;
        [SerializeField]
        BlueprintChoosePanel blueprintChoosePanel;
        [SerializeField]
        Button choosingViewAlmanacButton;
        [SerializeField]
        Button choosingViewStoreButton;
        [SerializeField]
        bool sideUIDisplaying = true;
        [SerializeField]
        bool blueprintChooseDisplaying;
        [SerializeField]
        GameObject artifactSlotsRoot;
        [SerializeField]
        ElementListUI artifactSlotList;
    }
}
