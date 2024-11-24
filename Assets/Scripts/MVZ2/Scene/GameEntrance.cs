using System;
using System.IO;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2Logic;
using MVZ2Logic.Scenes;
using UnityEngine;

namespace MVZ2.Scenes
{
    public class GameEntrance : MonoBehaviour
    {
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
            main.MusicManager.Play(VanillaMusicID.mainmenu);
            if (main.IsFastMode())
            {
                main.Scene.DisplayPage(MainScenePageType.Mainmenu);
            }
            else
            {
                main.Scene.DisplayPage(MainScenePageType.Landing);
            }
        }
        private string GetErrorMessage(Exception e)
        {
            switch (e)
            {
                case DirectoryNotFoundException dirNotFound:
                    return main.LanguageManager._p(VanillaStrings.CONTEXT_ERROR, ERROR_DIRECTORY_NOT_FOUND);
                case FileNotFoundException fileNotFound:
                    return main.LanguageManager._p(VanillaStrings.CONTEXT_ERROR, ERROR_FILE_NOT_FOUND);
                case IOException io:
                    if (io.Message.Contains("Sharing Violation"))
                    {
                        return main.LanguageManager._p(VanillaStrings.CONTEXT_ERROR, ERROR_SHARING_VIOLATION);
                    }
                    else
                    {
                        return main.LanguageManager._p(VanillaStrings.CONTEXT_ERROR, ERROR_FAILED_TO_LOAD_FILE);
                    }
                case FormatException format:
                    return main.LanguageManager._p(VanillaStrings.CONTEXT_ERROR, ERROR_INCORRECT_FILE_FORMAT);
                default:
                    return e.Message;
            }
        }
        private void ShowErrorDialog(Exception e)
        {
            Debug.LogException(e);
            var innerMessage = GetErrorMessage(e);
            var title = main.LanguageManager._(VanillaStrings.ERROR);
            var message = main.LanguageManager._p(VanillaStrings.CONTEXT_ERROR, ERROR_FAILED_TO_INITIALIZE, innerMessage);
            var options = new string[]
            {
                main.LanguageManager._(VanillaStrings.QUIT)
            };
            main.Scene.ShowDialog(title, message, options, i =>
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
        [SerializeField]
        private MainManager main;
    }
}
