using MukioI18n;
using MVZ2.Managers;
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
        #region 生命周期
        private void Awake()
        {
            ui.OnButtonClick += OnButtonClickCallback;
        }
        #endregion

        #region 事件回调
        private void OnButtonClickCallback()
        {
            var scene = main.Scene;
            scene.DisplayPage(MainScenePageType.Mainmenu);
        }
        #endregion

        private string GetVersionText()
        {
            return main.LanguageManager._(VERSION_TEXT, Application.version);
        }

        #region 属性字段
        [TranslateMsg("标题页面的版本号文本，{0}为版本号")]
        public const string VERSION_TEXT = "版本{0}";
        private MainManager main => MainManager.Instance;
        [SerializeField]
        private TitlescreenUI ui;
        #endregion
    }
}
