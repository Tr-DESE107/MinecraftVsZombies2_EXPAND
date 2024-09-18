using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.GameContent;
using MVZ2.Landing;
using MVZ2.Mainmenu;
using MVZ2.Titlescreen;
using MVZ2.UI;
using UnityEngine;

namespace MVZ2
{
    public class MainSceneController : MonoBehaviour
    {
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            ui.ShowDialog(title, desc, options, onSelect);
        }
        public void ShowMainmenu()
        {
            mainmenu.Display();
        }
        public void ShowTitlescreen()
        {
            titlescreen.Display();
        }
        private async void Start()
        {
            try
            {
                await main.Initialize();
            }
            catch (Exception e)
            {
                ShowErrorDialog(e);
                return;
            }
            main.MusicManager.Play(MusicID.mainmenu);
            landing.Display();
        }
        private string GetErrorMessage(Exception e)
        {
            switch (e)
            {
                case DirectoryNotFoundException dirNotFound:
                    return main.LanguageManager._p(StringTable.CONTEXT_ERROR, ERROR_DIRECTORY_NOT_FOUND);
                case FileNotFoundException fileNotFound:
                    return main.LanguageManager._p(StringTable.CONTEXT_ERROR, ERROR_FILE_NOT_FOUND);
                case IOException io:
                    if (io.Message.Contains("Sharing Violation"))
                    {
                        return main.LanguageManager._p(StringTable.CONTEXT_ERROR, ERROR_SHARING_VIOLATION);
                    }
                    else
                    {
                        return main.LanguageManager._p(StringTable.CONTEXT_ERROR, ERROR_FAILED_TO_LOAD_FILE);
                    }
                case FormatException format:
                    return main.LanguageManager._p(StringTable.CONTEXT_ERROR, ERROR_INCORRECT_FILE_FORMAT);
                default:
                    return e.Message;
            }
        }
        private void ShowErrorDialog(Exception e)
        {
            Debug.LogException(e);
            var innerMessage = GetErrorMessage(e);
            var title = main.LanguageManager._p(StringTable.CONTEXT_ERROR, ERROR_TITLE);
            var message = main.LanguageManager._p(StringTable.CONTEXT_ERROR, ERROR_FAILED_TO_INITIALIZE, innerMessage);
            var options = new string[]
            {
                main.LanguageManager._(ERROR_QUIT)
            };
            ui.ShowDialog(title, message, options, i =>
            {
                Quit();
            });
        }
        private void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        [TranslateMsg("开始游戏时读取出错，对话框的标题")]
        public const string ERROR_TITLE = "错误";
        [TranslateMsg("开始游戏时读取出错，对话框的描述，{0}为错误信息")]
        public const string ERROR_FAILED_TO_INITIALIZE = "游戏加载失败：\n{0}";
        [TranslateMsg("开始游戏时读取出错，对话框的错误信息")]
        public const string ERROR_DIRECTORY_NOT_FOUND = "文件目录丢失";
        [TranslateMsg("开始游戏时读取出错，对话框的错误信息")]
        public const string ERROR_FILE_NOT_FOUND = "文件丢失";
        [TranslateMsg("开始游戏时读取出错，对话框的错误信息")]
        public const string ERROR_SHARING_VIOLATION = "文件被占用";
        [TranslateMsg("开始游戏时读取出错，对话框的错误信息")]
        public const string ERROR_FAILED_TO_LOAD_FILE = "文件读取失败";
        [TranslateMsg("开始游戏时读取出错，对话框的错误信息")]
        public const string ERROR_INCORRECT_FILE_FORMAT = "文件格式错误";
        [TranslateMsg("开始游戏时读取出错，对话框的按钮文本")]
        public const string ERROR_QUIT = "退出";
        private MainManager main => MainManager.Instance;
        [SerializeField]
        private MainSceneUI ui;
        [SerializeField]
        private LandingController landing;
        [SerializeField]
        private MainmenuController mainmenu;
        [SerializeField]
        private TitlescreenController titlescreen; 
    }
}
