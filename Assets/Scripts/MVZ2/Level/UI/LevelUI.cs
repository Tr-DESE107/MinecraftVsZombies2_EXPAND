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
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public void SetEnergy(string value)
        {
            _energyText.text = value;
        }
        public void SetRaycasterMask(LayerMask mask)
        {
            foreach (var raycaster in _raycasters)
            {
                raycaster.blockingMask = mask;
            }
        }
        public void SetBlueprints(BlueprintViewData[] blueprints)
        {
            _blueprints.updateList(blueprints.Length, (i, rect) =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                var viewData = blueprints[i];
                blueprint.SetCost(viewData.cost);
                blueprint.SetIcon(viewData.icon);
                blueprint.SetTriggerActive(viewData.triggerActive);
                blueprint.SetTriggerCost(viewData.triggerCost);
            },
            rect =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                blueprint.OnPointerDown += OnBlueprintPointerDownCallback;
            },
            rect =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                blueprint.OnPointerDown -= OnBlueprintPointerDownCallback;
            });
        }
        public void SetBlueprintRecharges(float[] recharges)
        {
            for (int i = 0; i < _blueprints.count; i++)
            {
                var recharge = recharges[i];
                _blueprints.getElement<Blueprint>(i).SetRecharge(1 - recharge);
            }
        }
        public void SetBlueprintDisabled(bool[] disabledValues)
        {
            for (int i = 0; i < _blueprints.count; i++)
            {
                var disabled = disabledValues[i];
                _blueprints.getElement<Blueprint>(i).SetDisabled(disabled);
            }
        }
        public void SetBlueprintCount(int count)
        {
            _blueprintPlaceholders.updateList(count);
        }
        public void SetPickaxeVisible(bool visible)
        {
            _pickaxeSlot.SetPickaxeVisible(visible);
        }
        public void SetHeldItemPosition(Vector2 worldPos)
        {
            _heldItem.transform.position = worldPos;
        }
        public void SetHeldItemIcon(Sprite sprite)
        {
            _heldItem.SetIcon(sprite);
        }
        public void SetMoney(string money)
        {
            _moneyPanel.SetMoney(money);
        }
        public void SetMoneyFade(bool fade)
        {
            _moneyPanel.SetFade(fade);
        }
        public void ResetMoneyFadeTime()
        {
            _moneyPanel.ResetTimeout();
        }
        public void SetStarshardCount(int count, int maxCount)
        {
            _starshardPanel.SetPoints(count, maxCount);
        }
        public void SetLevelName(string name)
        {
            _levelNameText.text = name;
        }
        public void SetProgress(float progress)
        {
            progressBar.SetProgress(progress);
        }
        public void SetBannerProgresses(float[] progresses)
        {
            progressBar.SetBannerProgresses(progresses);
        }
        public void SetDifficulty(string difficulty)
        {
            _difficultyText.text = difficulty;
        }
        public void SetCameraLerp(float lerp)
        {
            foreach (var rectTrans in _limitRectTransforms)
            {
                if (!rectTrans)
                    return;
                var anchorX = Mathf.Lerp(_cameraLimitMinX, _cameraLimitMaxX, lerp);
                var anchorMin = rectTrans.anchorMin;
                var anchorMax = rectTrans.anchorMax;
                var pivot = rectTrans.pivot;
                anchorMin.x = anchorX;
                anchorMax.x = anchorX;
                pivot.x = anchorX;
                rectTrans.anchorMin = anchorMin;
                rectTrans.anchorMax = anchorMax;
                rectTrans.pivot = pivot;
                rectTrans.anchoredPosition = Vector2.zero;
            }
        }
        #region 私有方法
        private void Awake()
        {
            _sideReceiver.OnPointerDown += (data) => OnRaycastReceiverPointerDown?.Invoke(Receiver.Side);
            _lawnReceiver.OnPointerDown += (data) => OnRaycastReceiverPointerDown?.Invoke(Receiver.Lawn);
            _bottomReceiver.OnPointerDown += (data) => OnRaycastReceiverPointerDown?.Invoke(Receiver.Bottom);
            _pickaxeSlot.OnPointerDown += (data) => OnPickaxePointerDown?.Invoke(data);
            _starshardPanel.OnPointerDown += (data) => OnStarshardPointerDown?.Invoke(data);
            _menuButton.onClick.AddListener(() => OnMenuButtonClick?.Invoke());
        }
        #region 事件回调
        private void OnBlueprintPointerDownCallback(Blueprint blueprint, PointerEventData data)
        {
            OnBlueprintPointerDown?.Invoke(_blueprints.indexOf(blueprint), data);
        }
        #endregion

        #endregion
        public event Action<Receiver
            > OnRaycastReceiverPointerDown;
        public event Action<int, PointerEventData> OnBlueprintPointerDown;
        public event Action<PointerEventData> OnPickaxePointerDown;
        public event Action<PointerEventData> OnStarshardPointerDown;
        public event Action OnMenuButtonClick;

        [Header("General")]
        [SerializeField]
        GraphicRaycaster[] _raycasters;
        [SerializeField]
        TextMeshProUGUI _energyText;
        [SerializeField]
        ElementList _blueprints;
        [SerializeField]
        ElementList _blueprintPlaceholders;
        [SerializeField]
        PickaxeSlot _pickaxeSlot;

        [Header("Raycast Receivers")]
        [SerializeField]
        RaycastReciver _sideReceiver;
        [SerializeField]
        RaycastReciver _lawnReceiver;
        [SerializeField]
        RaycastReciver _bottomReceiver;

        [Header("CameraLimit")]
        [SerializeField]
        RectTransform[] _limitRectTransforms;
        [SerializeField]
        float _cameraLimitMinX;
        [SerializeField]
        float _cameraLimitMaxX;

        [Header("HeldItem")]
        [SerializeField]
        HeldItem _heldItem;

        [Header("Bottom")]
        [SerializeField]
        MoneyPanel _moneyPanel;
        [SerializeField]
        StarshardPanel _starshardPanel;
        [SerializeField]
        TextMeshProUGUI _levelNameText;
        [SerializeField]
        ProgressBar progressBar;

        [Header("Right Top")]
        [SerializeField]
        Button _menuButton;
        [SerializeField]
        TextMeshProUGUI _difficultyText;

        public enum Receiver
        {
            Side,
            Lawn,
            Bottom
        }
    }
}
