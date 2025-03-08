using System;
using System.Linq;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Scenes;
using MVZ2.Supporters;
using MVZ2Logic.Scenes;
using UnityEngine;

namespace MVZ2.Titlescreen
{
    public class TitlescreenController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            ui.SetVersionText(GetVersionText());
        }
        public async void Init()
        {
            if (!main.IsFastMode())
            {
                loadPipeline = new TaskPipeline();
                await main.InitLoad(loadPipeline);
                loadPipeline = null;
            }
        }
        #region 生命周期
        private void Awake()
        {
            ui.OnButtonClick += OnButtonClickCallback;
            ui.OnLanguageDialogConfirmed += OnLanguageDialogConfirmedCallback;
        }
        private void Update()
        {
            if (loadPipeline != null)
            {
                var progress = loadPipeline.GetProgress();
                var name = loadPipeline.GetCurrentTaskName();
                var text = main.LanguageManager._(name);
                targetProgress = progress;
                ui.SetLoadingText(text);
                ui.SetButtonInteractable(loadPipeline.IsFinished());
            }
            else
            {
                var text = main.LanguageManager._(CLICK_TO_START);
                targetProgress = 1;
                ui.SetLoadingText(text);
                ui.SetButtonInteractable(true);
            }
            progress = progress * 0.5f + targetProgress * 0.5f;
            ui.SetLoadingProgress(progress);
        }
        #endregion

        #region 事件回调
        private void OnButtonClickCallback()
        {
            if (!main.OptionsManager.IsLanguageInitialized())
            {
                var languages = main.LanguageManager.GetAllLanguages();
                ui.ShowLanguageDialog(languages.Select(l => main.LanguageManager.GetLanguageName(l)).ToArray());
            }
            else
            {
                main.Scene.DisplayPage(MainScenePageType.Mainmenu);
            }
        }
        private void OnLanguageDialogConfirmedCallback(int index)
        {
            ui.HideLanguageDialog();
            var languages = main.LanguageManager.GetAllLanguages();
            main.OptionsManager.SetLanguage(languages[index]);
            main.Scene.DisplayPage(MainScenePageType.Mainmenu);
        }
        #endregion

        private string GetVersionText()
        {
            return main.LanguageManager._(VERSION_TEXT, Application.version);
        }

        #region 属性字段
        [TranslateMsg("标题页面的版本号文本，{0}为版本号")]
        public const string VERSION_TEXT = "版本{0}";
        [TranslateMsg("标题界面按钮文本")]
        public const string CLICK_TO_START = "点击以开始！";
        private MainManager main => MainManager.Instance;
        private TaskPipeline loadPipeline;
        private float progress;
        private float targetProgress;
        [SerializeField]
        private TitlescreenUI ui;
        #endregion
    }
}
