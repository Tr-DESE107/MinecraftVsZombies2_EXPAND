using System;
using System.Collections.Generic;
using MVZ2.UI;
using TMPro;
using UnityEngine;

namespace MVZ2.Mainmenu
{
    public class MainmenuUI : MonoBehaviour
    {
        public void SetUserName(string name)
        {
            userNameText.text = name;
        }
        public void SetOptionsDialogVisible(bool visible)
        {
            optionsDialog.gameObject.SetActive(visible);
        }
        public void SetInputNameDialogVisible(bool visible)
        {
            SetInputNameDialogError(string.Empty);
            inputNameDialog.gameObject.SetActive(visible);
        }
        public void SetInputNameDialogError(string message)
        {
            inputNameDialog.SetErrorMessage(message);
        }
        public void SetBackgroundDark(bool dark)
        {
            backgroundLight.gameObject.SetActive(!dark);
            backgroundDark.gameObject.SetActive(dark);
        }
        public void SetButtonActive(MainmenuButtonType type, bool active)
        {
            almanacButton.gameObject.SetActive(false);
            storeButton.gameObject.SetActive(false);
        }
        public void SetRayblockerActive(bool active)
        {
            rayblocker.SetActive(active);
        }
        public IEnumerable<MainmenuButton> GetAllButtons()
        {
            return mainmenuButtonDict.Values;
        }
        private void Awake()
        {
            mainmenuButtonDict.Add(MainmenuButtonType.Adventure, adventureButton);
            mainmenuButtonDict.Add(MainmenuButtonType.Options, optionsButton);
            mainmenuButtonDict.Add(MainmenuButtonType.Help, helpButton);
            mainmenuButtonDict.Add(MainmenuButtonType.UserManage, userManageButton);
            mainmenuButtonDict.Add(MainmenuButtonType.Quit, quitButton);
            mainmenuButtonDict.Add(MainmenuButtonType.Almanac, almanacButton);
            mainmenuButtonDict.Add(MainmenuButtonType.Store, storeButton);
            mainmenuButtonDict.Add(MainmenuButtonType.MoreMenu, moreMenuButton);
            mainmenuButtonDict.Add(MainmenuButtonType.BackToMenu, backToMenuButton);
            mainmenuButtonDict.Add(MainmenuButtonType.Archive, archiveButton);
            mainmenuButtonDict.Add(MainmenuButtonType.Achievement, achievementButton);

            foreach (var pair in mainmenuButtonDict)
            {
                var type = pair.Key;
                pair.Value.OnClick += () => OnMainmenuButtonClick?.Invoke(type);
            }

            inputNameDialog.OnConfirm += OnInputNameConfirmCallback;
            inputNameDialog.OnCancel += OnInputNameCancelCallback;
        }
        private void OnInputNameConfirmCallback(string name)
        {
            OnInputNameConfirm?.Invoke(name);
        }
        private void OnInputNameCancelCallback()
        {
            OnInputNameCancel?.Invoke();
        }
        public event Action<string> OnInputNameConfirm;
        public event Action OnInputNameCancel;
        public event Action<MainmenuButtonType> OnMainmenuButtonClick;


        public OptionsDialog OptionsDialog => optionsDialog;
        private Dictionary<MainmenuButtonType, MainmenuButton> mainmenuButtonDict = new Dictionary<MainmenuButtonType, MainmenuButton>();

        [SerializeField]
        private GameObject backgroundLight;
        [SerializeField]
        private GameObject backgroundDark;
        [SerializeField]
        private GameObject rayblocker;
        [SerializeField]
        private TextMeshPro userNameText;
        [SerializeField]
        private InputNameDialog inputNameDialog;
        [SerializeField]
        private OptionsDialog optionsDialog;
        [Header("Buttons")]
        [SerializeField]
        private MainmenuButton adventureButton;
        [SerializeField]
        private MainmenuButton optionsButton;
        [SerializeField]
        private MainmenuButton helpButton;
        [SerializeField]
        private MainmenuButton userManageButton;
        [SerializeField]
        private MainmenuButton quitButton;
        [SerializeField]
        private MainmenuButton almanacButton;
        [SerializeField]
        private MainmenuButton storeButton;
        [SerializeField]
        private MainmenuButton moreMenuButton;
        [SerializeField]
        private MainmenuButton backToMenuButton;
        [SerializeField]
        private MainmenuButton archiveButton;
        [SerializeField]
        private MainmenuButton achievementButton;
    }
    public enum MainmenuButtonType
    {
        Adventure,
        Options,
        Help,
        UserManage,
        Quit,
        Almanac,
        Store,
        MoreMenu,
        BackToMenu,
        Archive,
        Achievement
    }
}
