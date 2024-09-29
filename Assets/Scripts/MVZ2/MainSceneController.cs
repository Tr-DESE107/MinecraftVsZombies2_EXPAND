using System;
using System.Collections.Generic;
using System.IO;
using MukioI18n;
using MVZ2.GameContent;
using MVZ2.Landing;
using MVZ2.Localization;
using MVZ2.Mainmenu;
using MVZ2.Managers;
using MVZ2.Map;
using MVZ2.Note;
using MVZ2.Titlescreen;
using MVZ2.UI;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class MainSceneController : MonoBehaviour
    {
        public void ShowDialogConfirm(string title, string desc, Action<bool> onSelect = null)
        {
            ShowDialog(title, desc, new string[]
            {
                main.LanguageManager._(StringTable.YES),
                main.LanguageManager._(StringTable.NO),
            }, (index) => onSelect?.Invoke(index == 0));
        }
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            ui.ShowDialog(title, desc, options, onSelect);
        }
        public void ShowPortal()
        {
            portal.Fadeout();
        }
        public void SetPortalFadeIn(Action OnFadeIn)
        {
            portal.SetDisplay(true);
            portal.OnFadeIn += OnFadeIn;
        }
        public void SetPortalFadeOut()
        {
            portal.SetDisplay(false);
        }
        public void DisplayPage(MainScenePageType type)
        {
            foreach (var pair in pages)
            {
                if (pair.Key == type)
                    pair.Value.Display();
                else
                    pair.Value.Hide();
            }
        }
        public void DisplayMap(NamespaceID mapId)
        {
            DisplayPage(MainScenePageType.Map);
            map.SetMap(mapId);
        }
        public void DisplayNote(NamespaceID id, string buttonText, Action onClose)
        {
            DisplayPage(MainScenePageType.Note);
            note.SetNote(id);
            note.SetButtonText(buttonText);
            note.OnClose += onClose;
        }
        private void Awake()
        {
            pages.Add(MainScenePageType.Landing, landing);
            pages.Add(MainScenePageType.Titlescreen, titlescreen);
            pages.Add(MainScenePageType.Mainmenu, mainmenu);
            pages.Add(MainScenePageType.Note, note);
            pages.Add(MainScenePageType.Map, map);
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
            if (main.IsFastMode())
            {
                DisplayPage(MainScenePageType.Mainmenu);
            }
            else
            {
                DisplayPage(MainScenePageType.Landing);
            }
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
            var title = main.LanguageManager._(StringTable.ERROR);
            var message = main.LanguageManager._p(StringTable.CONTEXT_ERROR, ERROR_FAILED_TO_INITIALIZE, innerMessage);
            var options = new string[]
            {
                main.LanguageManager._(StringTable.QUIT)
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
        private MainManager main => MainManager.Instance;
        private Dictionary<MainScenePageType, MainScenePage> pages = new Dictionary<MainScenePageType, MainScenePage>();
        [SerializeField]
        private MainSceneUI ui;
        [SerializeField]
        private LandingController landing;
        [SerializeField]
        private TitlescreenController titlescreen;
        [SerializeField]
        private MainmenuController mainmenu;
        [SerializeField]
        private NoteController note;
        [SerializeField]
        private MapController map;
        [SerializeField]
        private PortalController portal;
    }
    public enum MainScenePageType
    {
        Landing,
        Titlescreen,
        Mainmenu,
        Note,
        Map
    }
}
