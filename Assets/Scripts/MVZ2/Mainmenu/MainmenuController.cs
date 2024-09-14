using System.Collections;
using System.Collections.Generic;
using MVZ2.GameContent;
using UnityEngine;

namespace MVZ2.Mainmenu
{
    public class MainmenuController : MonoBehaviour
    {
        public void Display()
        {
            gameObject.SetActive(true);
            foreach (var button in GetAllButtons())
            {
                button.Interactable = true;
            }
            almanacButton.gameObject.SetActive(false);
            storeButton.gameObject.SetActive(false);
            backgroundLight.gameObject.SetActive(true);
            backgroundDark.gameObject.SetActive(false);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        #region 生命周期
        private void Awake()
        {
            adventureButton.OnClick += OnAdventureButtonClickCallback;
            optionsButton.OnClick += OnOptionsButtonClickCallback;
            helpButton.OnClick += OnHelpButtonClickCallback;
            userManageButton.OnClick += OnUserManageButtonClickCallback;
            quitButton.OnClick += OnQuitButtonClickCallback;

            almanacButton.OnClick += OnAlmanacButtonClickCallback;
            storeButton.OnClick += OnStoreButtonClickCallback;
            moreMenuButton.OnClick += OnMoreMenuButtonClickCallback;

            backToMenuButton.OnClick += OnBackToMenuButtonClickCallback;
            archiveButton.OnClick += OnArchiveButtonClickCallback;
            achievementButton.OnClick += OnAchievementButtonClickCallback;
        }
        #endregion

        #region 事件回调
        public void OnAdventureButtonClickCallback()
        {
            StartCoroutine(StartAdventure());
        }
        public void OnOptionsButtonClickCallback() { }
        public void OnHelpButtonClickCallback() { }
        public void OnUserManageButtonClickCallback() { }
        public void OnQuitButtonClickCallback()
        {
            Application.Quit();
        }

        public void OnAlmanacButtonClickCallback() { }
        public void OnStoreButtonClickCallback() { }
        public void OnMoreMenuButtonClickCallback() { }

        public void OnBackToMenuButtonClickCallback() { }
        public void OnArchiveButtonClickCallback() { }
        public void OnAchievementButtonClickCallback() { }
        #endregion
        private IEnumerator StartAdventure()
        {
            backgroundLight.gameObject.SetActive(false);
            backgroundDark.gameObject.SetActive(true);
            main.SoundManager.Play2D(SoundID.loseMusic);

            foreach (var button in GetAllButtons())
            {
                button.Interactable = false;
            }

            yield return new WaitForSeconds(6);
            var task = main.LevelManager.GotoLevelScene();
            while (!task.IsCompleted)
            {
                yield return null;
            }
            main.LevelManager.StartLevel();
            Hide();
        }
        private IEnumerable<MainmenuButton> GetAllButtons()
        {
            yield return adventureButton;
            yield return optionsButton;
            yield return helpButton;
            yield return userManageButton;
            yield return quitButton;

            yield return almanacButton;
            yield return storeButton;
            yield return moreMenuButton;

            yield return backToMenuButton;
            yield return archiveButton;
            yield return achievementButton;
        }
        #region 属性字段
        private MainManager main => MainManager.Instance;
        [SerializeField]
        private GameObject backgroundLight;
        [SerializeField]
        private GameObject backgroundDark;
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
        #endregion
    }
}
