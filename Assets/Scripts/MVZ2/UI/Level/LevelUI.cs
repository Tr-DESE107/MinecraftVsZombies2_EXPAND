using System;
using MVZ2.Level.UI;
using MVZ2.Models;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class LevelUI : MonoBehaviour
    {
        public void SetNightValue(float value)
        {
            var color = nightImage.color;
            color.a = value;
            nightImage.color = color;
        }
        public void SetDarknessValue(float value)
        {
            var color = darknessImage.color;
            color.a = value;
            darknessImage.color = color;
        }
        public void SetRaycastDisabled(bool disabled)
        {
            animator.SetBool("RaycastDisabled", disabled);
        }
        public void SetExitingToNote()
        {
            animator.SetTrigger("Exit");
        }
        public void CallExitLevelToNote()
        {
            OnExitLevelToNoteCalled?.Invoke();
        }
        public void ShowYouDied()
        {
            animator.SetTrigger("YouDied");
        }

        #region 手持物品
        public void SetHeldItemPosition(Vector2 worldPos)
        {
            heldItem.transform.position = worldPos;
        }
        public void SetHeldItemModel(Model prefab)
        {
            heldItem.SetModel(prefab);
        }
        public void SetHeldItemTrigger(bool visible, bool trigger)
        {
            heldItem.SetTrigger(visible, trigger);
        }
        public Model GetHeldItemModel()
        {
            return heldItem.GetModel();
        }
        public void UpdateHeldItemModelFixed()
        {
            heldItem.UpdateModelFixed();
        }
        public void UpdateHeldItemModelFrame(float deltaTime)
        {
            heldItem.UpdateModelFrame(deltaTime);
        }
        public void SetHeldItemModelSimulationSpeed(float speed)
        {
            heldItem.SetModelSimulationSpeed(speed);
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

        #region 选择蓝图
        public void SetViewLawnReturnBlockerActive(bool active)
        {
            viewLawnReturnBlocker.SetActive(active);
        }
        #endregion
        private void Awake()
        {
            pauseDialog.OnResumeClicked += () => OnPauseDialogResumeClicked?.Invoke();

            gameOverDialog.OnRetryButtonClicked += () => OnGameOverRetryButtonClicked?.Invoke();
            gameOverDialog.OnBackButtonClicked += () => OnGameOverBackButtonClicked?.Invoke();

            levelLoadedDialog.OnButtonClicked += (button) => OnLevelLoadedDialogButtonClicked?.Invoke(button);
            levelErrorLoadingDialog.OnButtonClicked += (restart) => OnLevelErrorLoadingDialogButtonClicked?.Invoke(restart);

            viewLawnReturnButton.onClick.AddListener(() => OnBlueprintChooseViewLawnReturnClick?.Invoke());
        }

        public event Action OnExitLevelToNoteCalled;
        public event Action OnPauseDialogResumeClicked;
        public event Action OnGameOverRetryButtonClicked;
        public event Action OnGameOverBackButtonClicked;
        public event Action OnBlueprintChooseViewLawnReturnClick;
        public event Action<LevelLoadedDialog.ButtonType> OnLevelLoadedDialogButtonClicked;
        public event Action<bool> OnLevelErrorLoadingDialogButtonClicked;
        public OptionsDialog OptionsDialog => optionsDialog;

        [SerializeField]
        Animator animator;
        [Header("Blueprint Choosing")]
        [SerializeField]
        Button viewLawnReturnButton;
        [SerializeField]
        GameObject viewLawnReturnBlocker;

        [Header("Shading")]
        [SerializeField]
        private Image nightImage;
        [SerializeField]
        private Image darknessImage;

        [Header("HeldItem")]
        [SerializeField]
        HeldItem heldItem;

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
    }
}
