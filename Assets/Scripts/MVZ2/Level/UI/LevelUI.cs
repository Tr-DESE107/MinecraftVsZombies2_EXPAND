using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
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
        public void SetBlueprints(BlueprintViewData[] blueprints)
        {
            _blueprints.updateList(blueprints.Length, ((i, rect) =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                var viewData = blueprints[i];
                blueprint.SetCost(viewData.cost);
                blueprint.SetIcon(viewData.icon);
                blueprint.SetTriggerActive(viewData.triggerActive);
                blueprint.SetTriggerCost(viewData.triggerCost);
            }));
        }
        public void SetPickaxeVisible(bool visible)
        {
            _pickaxeSlot.SetPickaxeVisible(visible);
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
        private void Awake()
        {
            _pickaxeSlot.OnPointerDown += () => OnPickaxePointerDown?.Invoke();
            _starshardPanel.OnPointerDown += () => OnStarshardPointerDown?.Invoke();
            _menuButton.onClick.AddListener(() => OnMenuButtonClick?.Invoke());
        }
        public event Action OnPickaxePointerDown;
        public event Action OnStarshardPointerDown;
        public event Action OnMenuButtonClick;

        [Header("General")]
        [SerializeField]
        TextMeshProUGUI _energyText;
        [SerializeField]
        ElementList _blueprints;
        [SerializeField]
        PickaxeSlot _pickaxeSlot;

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
    }
}
