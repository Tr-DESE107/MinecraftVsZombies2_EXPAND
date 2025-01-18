using System;
using System.Collections.Generic;
using MVZ2.UI;
using TMPro;
using UnityEngine;

namespace MVZ2.Mainmenu.UI
{
    public class MainmenuUI : MonoBehaviour
    {
        public void SetUserName(string name)
        {
            userNameText.text = name;
            userNameGoldText.text = name;
        }
        public void SetUserNameGold(bool gold)
        {
            userNameText.gameObject.SetActive(!gold);
            userNameGoldText.gameObject.SetActive(gold);
        }
        public void SetOptionsDialogVisible(bool visible)
        {
            optionsDialog.gameObject.SetActive(visible);
            optionsDialog.ResetPosition();
        }
        public void SetUserManageDialogVisible(bool visible)
        {
            userManageDialog.gameObject.SetActive(visible);
        }
        public void UpdateUserManageDialog(string[] names, int selectedIndex)
        {
            userManageDialog.UpdateUsers(names);
            userManageDialog.ResetPosition();
            userManageDialog.SelectUser(selectedIndex);
        }
        public void SetUserManageButtonInteractable(UserManageDialog.ButtonType type, bool interactable)
        {
            userManageDialog.SetButtonInteractable(type, interactable);
        }
        public void SetUserManageCreateNewUserActive(bool active)
        {
            userManageDialog.SetCreateNewUserActive(active);
        }
        public void SetWindowViewSprite(Sprite sprite)
        {
            windowViewSpriteRenderer.sprite = sprite;
        }
        public void SetBackgroundDark(bool dark)
        {
            backgroundLight.gameObject.SetActive(!dark);
            backgroundDark.gameObject.SetActive(dark);
        }
        public void SetButtonActive(MainmenuButtonType type, bool active)
        {
            switch (type)
            {
                case MainmenuButtonType.Almanac:
                    almanacButton.gameObject.SetActive(active);
                    break;
                case MainmenuButtonType.Store:
                    storeButton.gameObject.SetActive(active);
                    break;
            }
        }
        public void SetRayblockerActive(bool active)
        {
            rayblocker.SetActive(active);
        }
        public void UpdateStats(StatCategoryViewData[] viewDatas)
        {
            stats.UpdateStats(viewDatas);
        }
        public void UpdateAchievements(AchievementEntryViewData[] viewDatas)
        {
            achievements.UpdateAchievements(viewDatas);
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
            mainmenuButtonDict.Add(MainmenuButtonType.Stats, statsButton);
            mainmenuButtonDict.Add(MainmenuButtonType.Achievement, achievementButton);

            foreach (var pair in mainmenuButtonDict)
            {
                var type = pair.Key;
                pair.Value.OnClick += () => OnMainmenuButtonClick?.Invoke(type);
            }

            userManageDialog.OnCreateNewUserButtonClick += () => OnUserManageDialogCreateNewUserButtonClick?.Invoke();
            userManageDialog.OnButtonClick += type => OnUserManageDialogButtonClick?.Invoke(type);
            userManageDialog.OnUserSelect += index => OnUserManageDialogUserSelect?.Invoke(index);

            stats.OnReturnClick += () => OnStatsReturnButtonClick?.Invoke();
            achievements.OnReturnClick += () => OnAchievementsReturnButtonClick?.Invoke();
        }
        public event Action<MainmenuButtonType> OnMainmenuButtonClick;
        public event Action OnUserManageDialogCreateNewUserButtonClick;
        public event Action<UserManageDialog.ButtonType> OnUserManageDialogButtonClick;
        public event Action<int> OnUserManageDialogUserSelect;
        public event Action OnStatsReturnButtonClick;
        public event Action OnAchievementsReturnButtonClick;


        public OptionsDialog OptionsDialog => optionsDialog;
        private Dictionary<MainmenuButtonType, MainmenuButton> mainmenuButtonDict = new Dictionary<MainmenuButtonType, MainmenuButton>();

        [SerializeField]
        private GameObject rayblocker;
        [SerializeField]
        private StatsUI stats;
        [SerializeField]
        private AchievementsUI achievements;

        [Header("Backgrounds")]
        [SerializeField]
        private GameObject backgroundLight;
        [SerializeField]
        private GameObject backgroundDark;
        [SerializeField]
        private TextMeshPro userNameText;
        [SerializeField]
        private TextMeshPro userNameGoldText;
        [SerializeField]
        private SpriteRenderer windowViewSpriteRenderer;

        [Header("Dialogs")]
        [SerializeField]
        private OptionsDialog optionsDialog;
        [SerializeField]
        private UserManageDialog userManageDialog;

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
        private MainmenuButton statsButton;
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
        Stats,
        Achievement
    }
}
