using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class LevelUI : MonoBehaviour
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
        #endregion

        #region 左上角

        #region 能量
        public void SetEnergyVisible(bool value)
        {
            energyPanel.SetActive(value);
        }
        public void SetTriggerSlotVisible(bool value)
        {
            triggerSlot.SetActive(value);
        }
        public void SetEnergy(string value)
        {
            energyPanel.SetEnergy(value);
        }
        #endregion

        #region 蓝图
        public void SetBlueprintsVisible(bool value)
        {
            blueprints.SetActive(value);
        }
        public void SetBlueprintCount(int count)
        {
            blueprints.SetBlueprintCount(count);
        }
        public void SetBlueprintAt(int index, BlueprintViewData viewData)
        {
            var blueprint = blueprints.GetBlueprintAt(index);
            if (!blueprint)
                return;
            blueprint.UpdateView(viewData);
        }
        public void SetBlueprintRecharge(int index, float recharge)
        {
            var blueprint = blueprints.GetBlueprintAt(index);
            if (!blueprint)
                return;
            blueprint.SetRecharge(recharge);
        }
        public void SetBlueprintDisabled(int index, bool value)
        {
            var blueprint = blueprints.GetBlueprintAt(index);
            if (!blueprint)
                return;
            blueprint.SetDisabled(value);
        }
        public void SetBlueprintSelected(int index, bool value)
        {
            var blueprint = blueprints.GetBlueprintAt(index);
            if (!blueprint)
                return;
            blueprint.SetSelected(value);
        }
        public void SetBlueprintTwinkle(int index, bool twinkle)
        {
            var blueprint = GetBlueprintAt(index);
            if (!blueprint)
                return;
            blueprint.SetTwinkling(twinkle);
        }
        public Blueprint GetBlueprintAt(int index)
        {
            return blueprints.GetBlueprintAt(index);
        }
        #endregion

        #region 铁镐
        public void SetPickaxeSlotVisible(bool visible)
        {
            pickaxeSlot.SetActive(visible);
        }
        public void SetPickaxeVisible(bool visible)
        {
            pickaxeSlot.SetPickaxeVisible(visible);
        }
        #endregion

        #endregion

        #region 左下角

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
        public void SetStarshardVisible(bool visible)
        {
            starshardPanel.SetActive(visible);
        }
        public void SetStarshardCount(int count, int maxCount)
        {
            starshardPanel.SetPoints(count, maxCount);
        }
        #endregion

        #endregion

        #region 右上角

        public void SetTopRightVisible(bool visible)
        {
            topRightObj.SetActive(visible);
        }

        #region 游戏难度
        public void SetDifficulty(string difficulty)
        {
            difficultyText.text = difficulty;
        }
        #endregion

        #region 加速
        public void SetSpeedUpVisible(bool visible)
        {
            speedUpButton.gameObject.SetActive(visible);
        }
        public void SetSpeedUp(bool speedUp)
        {
            speedUpEnabledObject.SetActive(speedUp);
            speedUpDisabledObject.SetActive(!speedUp);
        }
        #endregion

        #endregion

        #region 关卡进度

        #region 关卡名
        public void SetLevelNameVisible(bool visible)
        {
            levelNameText.gameObject.SetActive(visible);
        }
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

        #region 手持物品
        public void SetHeldItemPosition(Vector2 worldPos)
        {
            heldItem.transform.position = worldPos;
        }
        public void SetHeldItemIcon(Sprite sprite)
        {
            heldItem.SetIcon(sprite);
        }
        #endregion

        #region 提示文本
        public void SetHugeWaveTextVisible(bool visible)
        {
            hugeWaveText.SetActive(visible);
        }
        public void SetFinalWaveTextVisible(bool visible)
        {
            finalWaveText.SetActive(visible);
        }
        public void SetReadySetBuildVisible(bool visible)
        {
            readyText.SetActive(visible);
        }
        public void SetLevelTextAnimationSpeed(float speed)
        {
            hugeWaveText.TextAnimator.speed = speed;
            finalWaveText.TextAnimator.speed = speed;
        }
        public void SetYouDiedVisible(bool visible)
        {
            youDiedText.SetActive(visible);
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

        #region 暂停对话框
        public void SetPauseDialogActive(bool active)
        {
            pauseDialogObj.SetActive(active);
            ResetPauseDialogPosition();
        }
        public void ResetPauseDialogPosition()
        {
            var rectTrans = pauseDialogObj.transform as RectTransform;
            rectTrans.anchoredPosition = Vector3.zero;
        }
        public void SetPauseDialogImage(Sprite sprite)
        {
            pauseDialog.SetPausedImage(sprite);
        }
        #endregion

        #region 游戏结束对话框
        public void SetGameOverDialogActive(bool active)
        {
            gameOverDialogObj.SetActive(active);
        }
        public void SetGameOverDialogMessage(string text)
        {
            gameOverDialog.SetMessage(text);
        }
        public void SetGameOverDialogInteractable(bool interactable)
        {
            gameOverDialog.SetInteractable(interactable);
        }
        #endregion

        #region 菜单对话框
        public void SetOptionsDialogActive(bool visible)
        {
            optionsDialogObj.SetActive(visible);
            ResetOptionsDialogPosition();
        }
        public void ResetOptionsDialogPosition()
        {
            var rectTrans = optionsDialogObj.transform as RectTransform;
            rectTrans.anchoredPosition = Vector3.zero;
        }
        #endregion

        #region 加载关卡对话框
        public void SetLevelLoadedDialogVisible(bool visible)
        {
            levelLoadedDialogObj.SetActive(visible);
        }
        public void SetLevelErrorLoadingDialogVisible(bool visible)
        {
            levelErrorLoadingDialogObj.SetActive(visible);
        }
        public void SetLevelErrorLoadingDialogDesc(string text)
        {
            levelErrorLoadingDialog.SetDescription(text);
        }
        public void SetLevelErrorLoadingDialogInteractable(bool interactable)
        {
            levelErrorLoadingDialog.SetInteractable(interactable);
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

        public Receiver GetReceiverType(RaycastReceiver receiver)
        {
            if (receiver == sideReceiver)
                return Receiver.Side;
            else if (receiver == bottomReceiver)
                return Receiver.Bottom;
            else
                return Receiver.Lawn;
        }

        #endregion

        #region 私有方法
        private void Awake()
        {
            sideReceiver.OnPointerDown += (data) => OnRaycastReceiverPointerDown?.Invoke(Receiver.Side, data);
            lawnReceiver.OnPointerDown += (data) => OnRaycastReceiverPointerDown?.Invoke(Receiver.Lawn, data);
            bottomReceiver.OnPointerDown += (data) => OnRaycastReceiverPointerDown?.Invoke(Receiver.Bottom, data);

            blueprints.OnBlueprintPointerEnter += (index, data) => OnBlueprintPointerEnter?.Invoke(index, data);
            blueprints.OnBlueprintPointerExit += (index, data) => OnBlueprintPointerExit?.Invoke(index, data);
            blueprints.OnBlueprintPointerDown += (index, data) => OnBlueprintPointerDown?.Invoke(index, data);

            pickaxeSlot.OnPointerEnter += (data) => OnPickaxePointerEnter?.Invoke(data);
            pickaxeSlot.OnPointerExit += (data) => OnPickaxePointerExit?.Invoke(data);
            pickaxeSlot.OnPointerDown += (data) => OnPickaxePointerDown?.Invoke(data);

            starshardPanel.OnPointerDown += (data) => OnStarshardPointerDown?.Invoke(data);

            triggerSlot.OnPointerDown += (data) => OnTriggerPointerDown?.Invoke(data);


            menuButton.onClick.AddListener(() => OnMenuButtonClick?.Invoke());
            speedUpButton.onClick.AddListener(() => OnSpeedUpButtonClick?.Invoke());
            readyText.OnStartGameCalled += () => OnStartGameCalled?.Invoke();
            pauseDialog.OnResumeClicked += () => OnPauseDialogResumeClicked?.Invoke();

            gameOverDialog.OnRetryButtonClicked += () => OnGameOverRetryButtonClicked?.Invoke();
            gameOverDialog.OnBackButtonClicked += () => OnGameOverBackButtonClicked?.Invoke();

            levelLoadedDialog.OnButtonClicked += (button) => OnLevelLoadedDialogButtonClicked?.Invoke(button);
            levelErrorLoadingDialog.OnButtonClicked += (restart) => OnLevelErrorLoadingDialogButtonClicked?.Invoke(restart);
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

        public event Action<PointerEventData> OnPickaxePointerEnter;
        public event Action<PointerEventData> OnPickaxePointerExit;
        public event Action<PointerEventData> OnPickaxePointerDown;

        public event Action<PointerEventData> OnStarshardPointerDown;

        public event Action<PointerEventData> OnTriggerPointerDown;

        public event Action OnMenuButtonClick;
        public event Action OnSpeedUpButtonClick;
        public event Action OnStartGameCalled;
        public event Action OnPauseDialogResumeClicked;
        public event Action OnGameOverRetryButtonClicked;
        public event Action OnGameOverBackButtonClicked;
        public event Action<LevelLoadedDialog.ButtonType> OnLevelLoadedDialogButtonClicked;
        public event Action<bool> OnLevelErrorLoadingDialogButtonClicked;
        #endregion

        #region 属性字段
        public OptionsDialog OptionsDialog => optionsDialog;

        [Header("Blueprints")]
        [SerializeField]
        EnergyPanel energyPanel;
        [SerializeField]
        TriggerSlot triggerSlot;
        [SerializeField]
        BlueprintList blueprints;
        [SerializeField]
        PickaxeSlot pickaxeSlot;

        [Header("Texts")]
        [SerializeField]
        LevelHintText hugeWaveText;
        [SerializeField]
        LevelHintText finalWaveText;
        [SerializeField]
        LevelHintText youDiedText;
        [SerializeField]
        ReadySetBuild readyText;

        [Header("Pause Dialog")]
        [SerializeField]
        GameObject pauseDialogObj;
        [SerializeField]
        PauseDialog pauseDialog;

        [Header("Game Over Dialog")]
        [SerializeField]
        GameObject gameOverDialogObj;
        [SerializeField]
        GameOverDialog gameOverDialog;

        [Header("Options Dialog")]
        [SerializeField]
        GameObject optionsDialogObj;
        [SerializeField]
        OptionsDialog optionsDialog;

        [Header("Level Loaded Dialog")]
        [SerializeField]
        GameObject levelLoadedDialogObj;
        [SerializeField]
        LevelLoadedDialog levelLoadedDialog;

        [Header("Level Error Loading Dialog")]
        [SerializeField]
        GameObject levelErrorLoadingDialogObj;
        [SerializeField]
        LevelErrorLoadingDialog levelErrorLoadingDialog;


        [Header("Raycast Receivers")]
        [SerializeField]
        GraphicRaycaster[] raycasters;
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

        [Header("HeldItem")]
        [SerializeField]
        HeldItem heldItem;

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
        float hintArrowAngleBlueprint;
        [SerializeField]
        float hintArrowAnglePickaxe;
        #endregion

        #region 内嵌类
        public enum Receiver
        {
            Side,
            Lawn,
            Bottom
        }
        #endregion
    }
}
