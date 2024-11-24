using System;
using MVZ2.Models;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class LevelUIPreset : MonoBehaviour
    {
        #region 公有方法

        #region 基本
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public void SetRaycasterMask(LayerMask mask)
        {
            foreach (var raycaster in raycasters)
            {
                raycaster.blockingMask = mask;
            }
        }
        public void UpdateFrame(float deltaTime)
        {
            animator.Update(deltaTime);
            float targetSideUIBlend = sideUIVisible ? 1 : 0;
            float targetBlueprintChooseBlend = blueprintChooseVisible ? 1 : 0;
            const float blendSpeed = 10;
            float sideUIBlendAddition = (targetSideUIBlend - sideUIBlend) * blendSpeed * deltaTime;
            float blueprintChooseAddition = (targetBlueprintChooseBlend - blueprintChooseBlend) * blendSpeed * deltaTime;
            SetSideUIBlend(sideUIBlend + sideUIBlendAddition);
            SetBlueprintChooseBlend(blueprintChooseBlend + blueprintChooseAddition);
        }
        public void SetBlockRaycasts(bool value)
        {
            foreach (var group in canvasGroups)
            {
                group.blocksRaycasts = value;
            }
        }
        #endregion

        #region 能量
        public void SetEnergy(string value)
        {
            energyPanel.SetEnergy(value);
        }
        #endregion

        #region 蓝图
        public void SetBlueprintsActive(bool visible)
        {
            blueprintsObj.SetActive(visible);
        }
        public void SetBlueprintSlotCount(int count)
        {
            blueprints.SetSlotCount(count);
        }
        public Blueprint CreateBlueprint()
        {
            return blueprints.CreateBlueprint();
        }
        public void AddBlueprint(Blueprint blueprint)
        {
            blueprints.AddBlueprint(blueprint);
        }
        public void InsertBlueprint(int index, Blueprint blueprint)
        {
            blueprints.InsertBlueprint(index, blueprint);
        }
        public void ForceAlignBlueprint(int index)
        {
            blueprints.ForceAlign(index);
        }
        public bool RemoveBlueprint(Blueprint blueprint)
        {
            return blueprints.RemoveBlueprint(blueprint);
        }
        public void RemoveBlueprintAt(int index)
        {
            blueprints.RemoveBlueprintAt(index);
        }
        public Blueprint GetBlueprintAt(int index)
        {
            return blueprints.GetBlueprintAt(index);
        }
        public Vector3 GetBlueprintPosition(int index)
        {
            return blueprints.GetBlueprintPosition(index);
        }
        #endregion

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

        #region 蓝图选择
        public void SetSideUIVisible(bool visible)
        {
            sideUIVisible = visible;
        }
        public void SetBlueprintsChooseVisible(bool visible)
        {
            blueprintChooseVisible = visible;
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
        public void ResetBlueprintChooseArtifactCount(int count)
        {
            blueprintChoosePanel.ResetArtifactCount(count);
        }
        public void UpdateBlueprintChooseArtifactAt(int index, ArtifactViewData viewData)
        {
            blueprintChoosePanel.UpdateArtifactAt(index, viewData);
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

        #region 铁镐
        public void SetPickaxeActive(bool visible)
        {
            pickaxeSlotObj.SetActive(visible);
        }
        public void SetPickaxeVisible(bool visible)
        {
            pickaxeSlot.SetPickaxeVisible(visible);
        }
        #endregion

        #region 钱
        public void SetMoney(string money)
        {
            moneyPanel.SetMoney(money);
        }
        public void HideMoney()
        {
            moneyPanel.Hide();
        }
        public void SetMoneyFade(bool fade)
        {
            moneyPanel.SetFade(fade);
        }
        public void ResetMoneyFadeTime()
        {
            moneyPanel.ResetTimeout();
        }
        #endregion

        #region 星之碎片
        public void SetStarshardActive(bool visible)
        {
            starshardPanelObj.SetActive(visible);
        }
        public void SetStarshardCount(int count, int maxCount)
        {
            starshardPanel.SetPoints(count, maxCount);
        }
        #endregion

        #region 触发
        public void SetTriggerActive(bool visible)
        {
            triggerSlotObj.SetActive(visible);
        }
        #endregion

        #region 右上角

        #region 游戏难度
        public void SetDifficulty(string difficulty)
        {
            difficultyText.text = difficulty;
        }
        #endregion

        #region 加速
        public void SetSpeedUp(bool speedUp)
        {
            speedUpEnabledObject.SetActive(speedUp);
            speedUpDisabledObject.SetActive(!speedUp);
        }
        #endregion

        #endregion

        #region 关卡进度

        #region 关卡名
        public void SetLevelName(string name)
        {
            levelNameText.text = name;
        }
        #endregion

        #region 关卡进度
        public void SetProgressVisible(bool visible)
        {
            progressBar.gameObject.SetActive(visible);
        }
        public void SetProgress(float progress)
        {
            progressBar.SetProgress(progress);
        }
        public void SetBannerProgresses(float[] progresses)
        {
            progressBar.SetBannerProgresses(progresses);
        }
        #endregion

        #endregion

        #region 提示文本
        public void ShowHugeWaveText()
        {
            animator.SetTrigger("HugeWave");
        }
        public void ShowFinalWaveText()
        {
            animator.SetTrigger("FinalWave");
        }
        public void ShowReadySetBuild()
        {
            animator.SetTrigger("ReadySetBuild");
        }
        public void ShowAdvice(string advice)
        {
            adviceObject.SetActive(true);
            adviceText.text = advice;
        }
        public void HideAdvice()
        {
            adviceObject.SetActive(false);
        }
        #endregion

        #region 提示箭头
        public void SetHintArrowPointToBlueprint(int index)
        {
            var blueprint = GetBlueprintAt(index);
            if (!blueprint)
            {
                HideHintArrow();
                return;
            }
            hintArrow.SetVisible(true);
            hintArrow.SetTarget(blueprint.transform, hintArrowOffsetBlueprint * 0.01f, hintArrowAngleBlueprint);
        }
        public void SetHintArrowPointToPickaxe()
        {
            hintArrow.SetVisible(true);
            var pickaxe = pickaxeSlot;
            hintArrow.SetTarget(pickaxe.transform, hintArrowOffsetPickaxe * 0.01f, hintArrowAnglePickaxe);
        }
        public void SetHintArrowPointToStarshard()
        {
            hintArrow.SetVisible(true);
            var starshard = starshardPanel.Icon;
            hintArrow.SetTarget(starshard.transform, hintArrowOffsetStarshard * 0.01f, hintArrowAngleStarshard);
        }
        public void SetHintArrowPointToEntity(Transform transform, float height)
        {
            hintArrow.SetVisible(true);
            hintArrow.SetTarget(transform, new Vector2(0, height + 16) * 0.01f, 180);
        }
        public void HideHintArrow()
        {
            hintArrow.SetVisible(false);
        }
        #endregion

        #region 工具提示
        public void ShowTooltipOnBlueprint(int index, TooltipViewData viewData)
        {
            var blueprint = GetBlueprintAt(index);
            if (!blueprint)
                return;
            ShowTooltipOnComponent(blueprint, viewData);
        }
        public void ShowTooltipOnPickaxe(TooltipViewData viewData)
        {
            ShowTooltipOnComponent(pickaxeSlot, viewData);
        }
        public void ShowTooltipOnComponent(Component component, TooltipViewData viewData)
        {
            if (component is not ITooltipUI ui)
                return;
            var anchor = ui.Anchor;
            if (anchor.IsDisabled)
                return;
            tooltip.gameObject.SetActive(true);
            tooltip.SetPivot(anchor.Pivot);
            tooltip.SetData(anchor.transform.position, viewData);
        }
        public void HideTooltip()
        {
            tooltip.gameObject.SetActive(false);
        }
        #endregion

        public void SetUIVisibleState(VisibleState state)
        {
            animator.SetInteger("UIState", (int)state);
        }
        public Receiver GetReceiverType(RaycastReceiver receiver)
        {
            if (receiver == sideReceiver)
                return Receiver.Side;
            else if (receiver == bottomReceiver)
                return Receiver.Bottom;
            else
                return Receiver.Lawn;
        }

        public SerializableLevelUIPreset ToSerializable()
        {
            return new SerializableLevelUIPreset()
            {
                animator = new SerializableAnimator(animator),
            };
        }
        public void LoadFromSerializable(SerializableLevelUIPreset serializable)
        {
            if (serializable == null)
                return;
            serializable.animator?.Deserialize(animator);
        }

        public void CallStartGame()
        {
            OnStartGameCalled?.Invoke();
        }
        #endregion

        #region 私有方法
        private void Awake()
        {
            animator.enabled = false;
            sideReceiver.OnPointerDown += (data) => OnRaycastReceiverPointerDown?.Invoke(Receiver.Side, data);
            lawnReceiver.OnPointerDown += (data) => OnRaycastReceiverPointerDown?.Invoke(Receiver.Lawn, data);
            bottomReceiver.OnPointerDown += (data) => OnRaycastReceiverPointerDown?.Invoke(Receiver.Bottom, data);

            blueprints.OnBlueprintPointerEnter += (index, data) => OnBlueprintPointerEnter?.Invoke(index, data);
            blueprints.OnBlueprintPointerExit += (index, data) => OnBlueprintPointerExit?.Invoke(index, data);
            blueprints.OnBlueprintPointerDown += (index, data) => OnBlueprintPointerDown?.Invoke(index, data);

            blueprintChoosePanel.OnArtifactClick += (index) => OnBlueprintChooseArtifactClick?.Invoke(index);
            blueprintChoosePanel.OnStartButtonClick += () => OnBlueprintChooseStartClick?.Invoke();
            blueprintChoosePanel.OnViewLawnButtonClick += () => OnBlueprintChooseViewLawnClick?.Invoke();
            blueprintChoosePanel.OnCommandBlockBlueprintClick += () => OnBlueprintChooseCommandBlockClick?.Invoke();
            blueprintChoosePanel.OnBlueprintPointerEnter += (index, data) => OnBlueprintChooseBlueprintPointerEnter?.Invoke(index, data);
            blueprintChoosePanel.OnBlueprintPointerExit += (index, data) => OnBlueprintChooseBlueprintPointerExit?.Invoke(index, data);
            blueprintChoosePanel.OnBlueprintPointerDown += (index, data) => OnBlueprintChooseBlueprintPointerDown?.Invoke(index, data);

            choosingViewAlmanacButton.onClick.AddListener(() => OnBlueprintChooseViewAlmanacClick?.Invoke());
            choosingViewStoreButton.onClick.AddListener(() => OnBlueprintChooseViewStoreClick?.Invoke());

            pickaxeSlot.OnPointerEnter += (data) => OnPickaxePointerEnter?.Invoke(data);
            pickaxeSlot.OnPointerExit += (data) => OnPickaxePointerExit?.Invoke(data);
            pickaxeSlot.OnPointerDown += (data) => OnPickaxePointerDown?.Invoke(data);

            starshardPanel.OnPointerDown += (data) => OnStarshardPointerDown?.Invoke(data);

            triggerSlot.OnPointerDown += (data) => OnTriggerPointerDown?.Invoke(data);


            menuButton.onClick.AddListener(() => OnMenuButtonClick?.Invoke());
            speedUpButton.onClick.AddListener(() => OnSpeedUpButtonClick?.Invoke());
        }
        private void Update()
        {
            UpdateCameraLimit();
        }
        #region 摄像机
        private void UpdateCameraLimit()
        {
            foreach (var rectTrans in limitRectTransforms)
            {
                if (!rectTrans)
                    continue;
                var parentTrans = rectTrans.parent as RectTransform;
                if (!parentTrans)
                    continue;
                var localToWorldMatrix = parentTrans.localToWorldMatrix;
                var worldToLocalMatrix = parentTrans.worldToLocalMatrix;

                var parentRect = parentTrans.rect;
                var lastLocalMinPos = parentRect.min;

                var worldMinPos = localToWorldMatrix.MultiplyPoint(lastLocalMinPos);
                worldMinPos.x = Mathf.Max(cameraLimitMinX, worldMinPos.x);
                var localMinPos = worldToLocalMatrix.MultiplyPoint(worldMinPos);

                rectTrans.anchorMin = Vector2.zero;
                rectTrans.anchorMax = Vector2.one;

                rectTrans.sizeDelta = new Vector2(lastLocalMinPos.x - localMinPos.x, 0);
                rectTrans.anchoredPosition = new Vector2(rectTrans.sizeDelta.x * -0.5f, 0);
            }
        }
        #endregion
        #endregion

        #region 事件
        public event Action<Receiver, PointerEventData> OnRaycastReceiverPointerDown;

        public event Action<int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintPointerDown;

        public event Action OnBlueprintChooseStartClick;
        public event Action OnBlueprintChooseViewLawnClick;
        public event Action OnBlueprintChooseCommandBlockClick;
        public event Action<int> OnBlueprintChooseArtifactClick;
        public event Action<int, PointerEventData> OnBlueprintChooseBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintChooseBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintChooseBlueprintPointerDown;

        public event Action OnBlueprintChooseViewAlmanacClick;
        public event Action OnBlueprintChooseViewStoreClick;

        public event Action<PointerEventData> OnPickaxePointerEnter;
        public event Action<PointerEventData> OnPickaxePointerExit;
        public event Action<PointerEventData> OnPickaxePointerDown;

        public event Action<PointerEventData> OnStarshardPointerDown;

        public event Action<PointerEventData> OnTriggerPointerDown;
        public event Action OnStartGameCalled;
        public event Action OnMenuButtonClick;
        public event Action OnSpeedUpButtonClick;
        #endregion

        #region 属性字段

        [SerializeField]
        Animator animator;
        [SerializeField]
        GraphicRaycaster[] raycasters;
        [SerializeField]
        CanvasGroup[] canvasGroups;


        [Header("Enabling")]
        [SerializeField]
        GameObject pickaxeSlotObj;
        [SerializeField]
        GameObject starshardPanelObj;
        [SerializeField]
        GameObject triggerSlotObj;
        [SerializeField]
        GameObject blueprintsObj;

        [Header("Blueprints")]
        [SerializeField]
        EnergyPanel energyPanel;
        [SerializeField]
        TriggerSlot triggerSlot;
        [SerializeField]
        BlueprintList blueprints;
        [SerializeField]
        PickaxeSlot pickaxeSlot;
        [SerializeField]
        MovingBlueprintList movingBlueprints;

        [Header("Blueprint Choose")]
        [SerializeField]
        BlueprintChoosePanel blueprintChoosePanel;
        [SerializeField]
        Button choosingViewAlmanacButton;
        [SerializeField]
        Button choosingViewStoreButton;
        [SerializeField]
        bool sideUIVisible = true;
        float sideUIBlend = 1;
        [SerializeField]
        bool blueprintChooseVisible;
        float blueprintChooseBlend;

        [Header("Raycast Receivers")]
        [SerializeField]
        RaycastReceiver sideReceiver;
        [SerializeField]
        RaycastReceiver lawnReceiver;
        [SerializeField]
        RaycastReceiver bottomReceiver;

        [Header("CameraLimit")]
        [SerializeField]
        RectTransform[] limitRectTransforms;
        [SerializeField]
        float cameraLimitMinX = 2.2f;

        [Header("Bottom")]
        [SerializeField]
        MoneyPanel moneyPanel;
        [SerializeField]
        StarshardPanel starshardPanel;
        [SerializeField]
        TextMeshProUGUI levelNameText;
        [SerializeField]
        ProgressBar progressBar;

        [Header("Right Top")]
        [SerializeField]
        GameObject topRightObj;
        [SerializeField]
        Button speedUpButton;
        [SerializeField]
        GameObject speedUpEnabledObject;
        [SerializeField]
        GameObject speedUpDisabledObject;
        [SerializeField]
        Button menuButton;
        [SerializeField]
        TextMeshProUGUI difficultyText;

        [Header("Advice")]
        [SerializeField]
        GameObject adviceObject;
        [SerializeField]
        TextMeshProUGUI adviceText;

        [Header("Tooltip")]
        [SerializeField]
        Tooltip tooltip;

        [Header("Hint Arrow")]
        [SerializeField]
        HintArrow hintArrow;
        [SerializeField]
        Vector2 hintArrowOffsetBlueprint;
        [SerializeField]
        Vector2 hintArrowOffsetPickaxe;
        [SerializeField]
        Vector2 hintArrowOffsetStarshard;
        [SerializeField]
        float hintArrowAngleBlueprint;
        [SerializeField]
        float hintArrowAnglePickaxe;
        [SerializeField]
        float hintArrowAngleStarshard;
        #endregion

        #region 内嵌类
        public enum Receiver
        {
            Side,
            Lawn,
            Bottom
        }
        public enum VisibleState
        {
            Nothing = 0,
            ChoosingBlueprints = 1,
            InLevel = 2,
        }
        #endregion
    }
    [Serializable]
    public class SerializableLevelUIPreset
    {
        public SerializableAnimator animator;
    }
}
