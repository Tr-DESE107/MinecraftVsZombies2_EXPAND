﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.Mainmenu.UI;
using MVZ2.Managers;
using MVZ2.Saves;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Scenes;
using UnityEngine;

namespace MVZ2.Scenes
{
    public class GameEntrance : MonoBehaviour
    {
        private async void Start()
        {
            if (!await Initialize())
                return;

            await CheckSaveDataStatus();
            await StartGame();
        }
        private async Task<bool> Initialize()
        {
            try
            {
                loadingText.SetActive(true);
                await main.Initialize();
                return true;
            }
            catch (Exception e)
            {
                ShowErrorDialog(e);
                return false;
            }
            finally
            {
                loadingText.SetActive(false);
            }
        }
        private async Task StartGame()
        {
            try
            {
                loadingText.SetActive(true);
                main.InitLoad();

                if (!main.IsFastMode())
                {
                    main.Scene.DisplayPage(MainScenePageType.Splash);
                }
                else
                {
                    var initTask = main.GetInitTask();
                    if (initTask != null)
                    {
                        await initTask;
                    }
                    main.Scene.DisplayMainmenu();
                }
                main.MusicManager.Play(VanillaMusicID.mainmenu);
            }
            catch (Exception e)
            {
                ShowErrorDialog(e);
            }
            finally
            {
                loadingText.SetActive(false);
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
        private async Task CheckSaveDataStatus()
        {
            var status = main.SaveManager.GetSaveDataStatus();
            switch (status.State)
            {
                case SaveDataState.SomeCorrupted:
                    {
                        var corruptedIndexes = status.GetCorruptedUserIndexes();
                        var corruptedUserNames = corruptedIndexes.Select(i => main.SaveManager.GetUserName(i));
                        var names = string.Join(", ", corruptedUserNames);
                        var currentName = main.SaveManager.GetCurrentUserName();
                        var title = main.LanguageManager._(VanillaStrings.WARNING);
                        var desc = main.LanguageManager._(ERROR_SOME_USERS_CORRUPTED, currentName, names);
                        await main.Scene.ShowDialogMessageAsync(title, desc);
                    }
                    break;
                case SaveDataState.AllCorrupted:
                    {
                        var title = main.LanguageManager._(VanillaStrings.WARNING);
                        var desc = main.LanguageManager._(ERROR_ALL_USERS_CORRUPTED);
                        await main.Scene.ShowDialogMessageAsync(title, desc);

                        var result = await main.Scene.ShowInputNameDialogAsync(InputNameType.Initialize);
                        var newIndex = main.SaveManager.CreateNewUser(result);
                        main.SaveManager.SetCurrentUserIndex(newIndex);
                        main.SaveManager.SaveUserList();
                    }
                    break;
                case SaveDataState.FullCorrupted:
                    {
                        var title = main.LanguageManager._(VanillaStrings.WARNING);
                        var desc = main.LanguageManager._(ERROR_FULL_USERS_CORRUPTED);
                        await main.Scene.ShowDialogMessageAsync(title, desc);

                        var users = main.SaveManager.GetAllUsers();
                        var deleteIndex = await main.Scene.ShowDeleteUserDialogAsync(users);
                        main.SaveManager.DeleteUser(deleteIndex);
                        main.SaveManager.SaveUserList();

                        var result = await main.Scene.ShowInputNameDialogAsync(InputNameType.Initialize);
                        var newIndex = main.SaveManager.CreateNewUser(result);
                        main.SaveManager.SetCurrentUserIndex(newIndex);
                        main.SaveManager.SaveUserList();
                    }
                    break;
            }
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
        [TranslateMsg("开始游戏时读取出错，对话框的错误信息，{0}为新使用的存档，{1}为存档列表")]
        public const string ERROR_SOME_USERS_CORRUPTED = "部分存档无法读取，将使用存档{0}开始游戏。\n无法读取的存档：\n{1}";
        [TranslateMsg("开始游戏时读取出错，对话框的错误信息")]
        public const string ERROR_ALL_USERS_CORRUPTED = "所有存档均无法读取，文件可能已损坏。\n必须新建存档以继续游戏。";
        [TranslateMsg("开始游戏时读取出错，对话框的错误信息")]
        public const string ERROR_FULL_USERS_CORRUPTED = "所有存档均无法读取，文件可能已损坏。\n必须删除一个存档，并新建存档以继续游戏。";
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private GameObject loadingText;
    }
}
