﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Notes;
using MVZ2.GameContent.Stages;
using MVZ2.IO;
using MVZ2.Mainmenu.UI;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Options;
using MVZ2.Saves;
using MVZ2.Scenes;
using MVZ2.Supporters;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.Saves;
using PVZEngine;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.Mainmenu
{
    public class MainmenuController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            foreach (var button in GetAllButtons())
            {
                button.Interactable = true;
            }
            ui.SetButtonActive(MainmenuButtonType.Almanac, main.SaveManager.IsAlmanacUnlocked());
            ui.SetButtonActive(MainmenuButtonType.Store, main.SaveManager.IsStoreUnlocked());
            ui.SetButtonActive(MainmenuButtonType.MusicRoom, main.SaveManager.IsMusicRoomUnlocked());
            ui.SetButtonActive(MainmenuButtonType.Arcade, main.SaveManager.IsArcadeUnlocked());

            isDark = false;
            ui.SetBackgroundDark(false);
            ui.SetOptionsDialogVisible(false);
            ui.SetUserManageDialogVisible(false);
            ui.SetRayblockerActive(true);

            UpdateWindowView();

            if (!main.MusicManager.IsPlaying(VanillaMusicID.mainmenu))
            {
                main.MusicManager.Play(VanillaMusicID.mainmenu);
            }
            ui.SetVersion(Application.version);
            var name = main.SaveManager.GetCurrentUserName();
            bool isSpecialName = main.Game.IsSpecialUserName(name);
            ui.SetUserName(name);
            ui.SetUserNameColor(isSpecialName ? Color.red : Color.black);
            ui.SetUserNameGold(!isSpecialName && main.SponsorManager.HasSponsorPlan(name, SponsorPlans.Furnace.TYPE, SponsorPlans.Furnace.BLAST_FURNACE));
            animatorBlendStart = mainmenuBlend;
            animatorBlendEnd = mainmenuBlend;
        }
        public void SetViewToBasement()
        {
            animator.SetTrigger("Instant");
            animatorBlendStart = basementBlend;
            animatorBlendEnd = basementBlend;
            var blend = GetCurrentAnimatorBlend();
            animator.SetFloat("BlendX", blend.x);
            animator.SetFloat("BlendY", blend.y);
            ui.SetRayblockerActive(false);
        }
        public void Reload()
        {
            Hide();
            Display();
        }
        public async void Init()
        {
            var userName = main.SaveManager.GetCurrentUserName();
            if (string.IsNullOrEmpty(userName))
            {
                var result = await main.Scene.ShowInputNameDialogAsync(InputNameType.Initialize);
                if (!string.IsNullOrEmpty(result))
                {
                    RenameUser(main.SaveManager.GetCurrentUserIndex(), result);
                }
            }
            ui.SetRayblockerActive(false);
        }
        public void ShowCredits()
        {
            var viewDatas = new List<CreditsCategoryViewData>();
            var categories = main.ResourceManager.GetAllCreditsCategories();
            foreach (var category in categories)
            {
                var viewData = new CreditsCategoryViewData()
                {
                    name = main.LanguageManager._p(VanillaStrings.CONTEXT_CREDITS_CATEGORY, category.Name),
                    entries = category.Entries.Select(e => main.LanguageManager._p(VanillaStrings.CONTEXT_STAFF_NAME, e)).ToArray(),
                };
                viewDatas.Add(viewData);
            }
            ui.ShowCredits(viewDatas.ToArray());
        }
        public void ShowKeybinding()
        {
            bindingKeys = main.OptionsManager.GetAllKeyBindings();
            bindingKeyIndex = -1;
            UpdateKeybindingItems();
            ui.SetKeybindingActive(true);
        }
        #region 生命周期
        private void Awake()
        {
            mainmenuActionDict.Add(MainmenuButtonType.Adventure, OnAdventureButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Options, OnOptionsButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Help, OnHelpButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.UserManage, OnUserManageButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Quit, OnQuitButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Almanac, OnAlmanacButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Store, OnStoreButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.MoreMenu, OnMoreMenuButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.BackToMenu, OnBackToMenuButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Archive, OnArchiveButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Addons, OnAddonsButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Stats, OnStatsButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Achievement, OnAchievementButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.MusicRoom, OnMusicRoomButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Arcade, OnArcadeButtonClickCallback);
            ui.OnMainmenuButtonClick += OnMainmenuButtonClickCallback;

            ui.OnUserManageDialogButtonClick += OnUserManageButtonClickCallback;
            ui.OnUserManageDialogUserSelect += OnUserManageUserSelectCallback;
            ui.OnUserManageDialogCreateNewUserButtonClick += OnUserManageCreateNewUserButtonClickCallback;

            ui.OnStatsReturnButtonClick += OnStatsReturnClickCallback;
            ui.OnAchievementsReturnButtonClick += OnAchievementsReturnClickCallback;
            ui.OnCreditsReturnButtonClick += OnCreditsReturnClickCallback;

            ui.OnKeybindingReturnButtonClick += OnKeybindingReturnButtonClickCallback;
            ui.OnKeybindingResetButtonClick += OnKeybindingResetButtonClickCallback;
            ui.OnKeybindingItemButtonClick += OnKeybindingItemButtonClickCallback;
        }
        private void Update()
        {
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    StartCoroutine(GotoDebugStage());
                }
            }
            if (animatorBlendTimeout > 0)
            {
                animatorBlendTimeout -= Time.deltaTime;
                if (animatorBlendTimeout <= 0)
                {
                    animatorBlendTimeout = 0;
                    ui.SetRayblockerActive(false);
                }
                var blend = GetCurrentAnimatorBlend();
                animator.SetFloat("BlendX", blend.x);
                animator.SetFloat("BlendY", blend.y);
            }
            UpdateKeybindingCheck();
        }
        #endregion

        #region 事件回调
        private void OnMainmenuButtonClickCallback(MainmenuButtonType type)
        {
            if (mainmenuActionDict.TryGetValue(type, out var action))
            {
                action?.Invoke();
            }
        }
        private void OnAdventureButtonClickCallback()
        {
            StartCoroutine(StartAdventure());
        }
        private void OnOptionsButtonClickCallback()
        {
            ui.SetOptionsDialogVisible(true);
            optionsLogic = new OptionsLogicMainmenu(ui.OptionsDialog, this);
            optionsLogic.InitDialog();
            optionsLogic.OnClose += OnOptionsCloseClickCallback;
        }
        private void OnHelpButtonClickCallback()
        {
            main.SoundManager.Play2D(VanillaSoundID.paper);
            main.MusicManager.Stop();
            var buttonText = main.LanguageManager._(Vanilla.VanillaStrings.BACK);
            main.Scene.DisplayNote(VanillaNoteID.help, buttonText);
        }
        private void OnUserManageButtonClickCallback()
        {
            ui.SetUserManageDialogVisible(true);
            RefreshUserManageDialog();
        }
        private void OnQuitButtonClickCallback()
        {
            var title = main.LanguageManager._(Vanilla.VanillaStrings.QUIT);
            var desc = main.LanguageManager._(QUIT_DESC);
            main.Scene.ShowDialogSelect(title, desc, (value) =>
            {
                if (value)
                    Application.Quit();
            });
        }

        private void OnAlmanacButtonClickCallback()
        {
            main.Scene.DisplayAlmanac(() => main.Scene.DisplayMainmenu());
        }
        private void OnStoreButtonClickCallback()
        {
            main.Scene.DisplayStore(() => main.Scene.DisplayMainmenu(), false);
        }
        private void OnMoreMenuButtonClickCallback()
        {
            StartAnimatorTransition(basementBlend);
        }

        private void OnBackToMenuButtonClickCallback()
        {
            StartAnimatorTransition(mainmenuBlend);
        }
        private void OnArchiveButtonClickCallback()
        {
            main.Scene.DisplayArchive(() => main.Scene.DisplayMainmenuToBasement());
        }
        private void OnAddonsButtonClickCallback()
        {
            main.Scene.DisplayAddons(() => main.Scene.DisplayMainmenuToBasement());
        }
        private void OnStatsButtonClickCallback()
        {
            ReloadStats();
            StartAnimatorTransition(statsBlend);
        }
        private void OnAchievementButtonClickCallback()
        {
            ReloadAchievements();
            StartAnimatorTransition(achievementsBlend);
        }
        private void OnMusicRoomButtonClickCallback()
        {
            main.Scene.DisplayMusicRoom(() => main.Scene.DisplayMainmenu());
        }
        private void OnArcadeButtonClickCallback()
        {
            main.Scene.DisplayArcade(() => main.Scene.DisplayMainmenu());
        }

        private void OnOptionsCloseClickCallback()
        {
            ui.SetOptionsDialogVisible(false);
            if (optionsLogic == null)
                return;
            if (optionsLogic.NeedsReload)
            {
                main.OptionsManager.SetLanguage(optionsLogic.Language);
                main.OptionsManager.SetBloodAndGore(optionsLogic.BloodAndGore);
                Reload();
            }
            optionsLogic.OnClose -= OnOptionsCloseClickCallback;
            optionsLogic.Dispose();
        }

        #region 用户管理
        private void OnUserManageUserSelectCallback(int index)
        {
            selectedUserArrayIndex = index;
            UpdateUserManageButtons();
        }
        private async void OnUserManageCreateNewUserButtonClickCallback()
        {
            string result = await main.Scene.ShowInputNameDialogAsync(InputNameType.CreateNewUser);
            if (!string.IsNullOrEmpty(result))
            {
                main.SaveManager.CreateNewUser(result);
                main.SaveManager.SaveUserList();
                RefreshUserManageDialog();
            }
        }
        private async void OnUserManageButtonClickCallback(UserManageDialog.ButtonType type)
        {
            switch (type)
            {
                case UserManageDialog.ButtonType.Rename:
                    {
                        var userIndex = GetSelectedUserIndex();
                        var currentName = main.SaveManager.GetUserName(userIndex);
                        if (!main.SaveManager.CanRenameUser(currentName))
                        {
                            var title = main.LanguageManager._(VanillaStrings.HINT);
                            var desc = main.LanguageManager._(VanillaStrings.ERROR_MESSAGE_CANNOT_RENAME_THIS_USER);
                            main.Scene.ShowDialogMessage(title, desc);
                            break;
                        }
                        string result = await main.Scene.ShowInputNameDialogRenameAsync(userIndex);
                        if (!string.IsNullOrEmpty(result))
                        {
                            RenameUser(userIndex, result);
                        }
                    }
                    break;
                case UserManageDialog.ButtonType.Delete:
                    {
                        var userIndex = GetSelectedUserIndex();
                        var title = main.LanguageManager._(VanillaStrings.WARNING);
                        var desc = main.LanguageManager._(VanillaStrings.WARNING_DELETE_USER, main.SaveManager.GetUserName(userIndex));
                        main.Scene.ShowDialogSelect(title, desc, (value) =>
                        {
                            if (value)
                            {
                                DeleteUser(GetSelectedUserIndex());
                            }
                        });
                    }
                    break;
                case UserManageDialog.ButtonType.Switch:
                    {
                        var userIndex = GetSelectedUserIndex();
                        SwitchUser(userIndex);
                    }
                    break;
                case UserManageDialog.ButtonType.Back:
                    {
                        HideUserManageDialog();
                    }
                    break;
                case UserManageDialog.ButtonType.Import:
                    {
                        if (IsUserFull())
                        {
                            break;
                        }
                        await FileHelper.OpenExternalFile(new string[] { "zip" }, OnImportPathSelected);
                    }
                    break;
                case UserManageDialog.ButtonType.Export:
                    {
                        var userIndex = GetSelectedUserIndex();
                        if (userIndex < 0)
                            break;
                        var userName = main.SaveManager.GetUserName(userIndex);
                        bool success = false;
                        var fileName = userName;
                        foreach (var chr in Path.GetInvalidFileNameChars())
                        {
                            fileName = fileName.Replace(chr, '_');
                        }

                        var path = await FileHelper.SaveExternalFile(fileName, new string[] { "zip" }, dest =>
                        {
                            if (string.IsNullOrEmpty(dest))
                                return;
                            try
                            {
                                success = main.SaveManager.ExportUserDataPack(userIndex, dest);
                            }
                            catch (Exception)
                            {
                                success = false;
                            }
                        });
                        if (string.IsNullOrEmpty(path))
                            break;
                        string title, desc;
                        if (!success)
                        {
                            title = main.LanguageManager._(VanillaStrings.ERROR);
                            desc = main.LanguageManager._(ERROR_NOT_EXPORTED);
                        }
                        else
                        {
                            title = main.LanguageManager._(VanillaStrings.HINT);
                            desc = main.LanguageManager._(HINT_EXPORTED, path);
                        }
                        await main.Scene.ShowDialogMessageAsync(title, desc);
                    }
                    break;
            }
        }
        private async void OnImportPathSelected(string path)
        {
            try
            {
                var userIndex = main.SaveManager.FindFreeUserIndex();
                if (userIndex < 0)
                    return;
                if (string.IsNullOrEmpty(path))
                    return;
                if (!File.Exists(path))
                    return;

                UserDataPackMetadata metadata;
                try
                {
                    metadata = main.SaveManager.ImportUserDataPackMetadata(path);
                    if (metadata == null)
                    {
                        throw new IOException($"Cannot import user data pack metadata from path {path}");
                    }
                }
                catch (Exception)
                {
                    // 加载失败，用户文件可能损坏。
                    var title = main.LanguageManager._(VanillaStrings.ERROR);
                    var desc = main.LanguageManager._(ERROR_CORRUPT_USER_DATA_PACK);
                    main.Scene.ShowDialogMessage(title, desc);
                    return;
                }

                // 如果有重复的名称，则提示用户重新命名。
                var userName = metadata.username;
                if (main.SaveManager.HasDuplicateUserName(userName, -1))
                {
                    // 如果不能重命名，直接提示错误。
                    if (!main.SaveManager.CanRenameUser(userName))
                    {
                        var title = main.LanguageManager._(VanillaStrings.ERROR);
                        var desc = main.LanguageManager._(ERROR_DUPLICATE_IMPORTING_USER_NAME_AND_CANNOT_RENAME);
                        main.Scene.ShowDialogMessage(title, desc);
                        return;
                    }
                    else
                    {
                        var newName = await main.Scene.ShowInputNameDialogAsync(InputNameType.Rename);
                        if (string.IsNullOrEmpty(newName))
                            return;
                        userName = newName;
                    }
                }

                main.SaveManager.ImportUserDataPack(userName, userIndex, path);
                RefreshUserManageDialog();
            }
            catch (Exception e)
            {
                Debug.LogError($"导入用户存档时出现错误：{e}");
            }
        }
        #endregion

        #region 统计
        private void OnStatsReturnClickCallback()
        {
            StartAnimatorTransition(basementBlend);
        }
        private void OnAchievementsReturnClickCallback()
        {
            StartAnimatorTransition(basementBlend);
        }
        #endregion

        #region 制作人员名单
        private void OnCreditsReturnClickCallback()
        {
            ui.HideCredits();
        }
        #endregion

        #region 按键绑定
        private void OnKeybindingReturnButtonClickCallback()
        {
            bindingKeys = null;
            bindingKeyIndex = -1;
            ui.SetKeybindingActive(false);
        }
        private void OnKeybindingResetButtonClickCallback()
        {
            var title = main.LanguageManager._(VanillaStrings.WARNING);
            var desc = main.LanguageManager._(RESET_KEY_BINDINGS_WARNING);
            main.Scene.ShowDialogSelect(title, desc, (confirm) =>
            {
                if (confirm)
                {
                    main.OptionsManager.ResetKeyBindings();
                    UpdateKeybindingItems();
                }
            });
        }
        private void OnKeybindingItemButtonClickCallback(int index)
        {
            bindingKeyIndex = index;
            UpdateKeybindingItem(index);
        }
        #endregion

        #endregion
        private IEnumerator StartAdventure()
        {
            if (!main.IsFastMode())
            {
                isDark = true;
                ui.SetBackgroundDark(true);
                UpdateWindowView();
                main.MusicManager.Stop();
                main.SoundManager.Play2D(VanillaSoundID.loseMusic);

                foreach (var button in GetAllButtons())
                {
                    button.Interactable = false;
                }

                yield return new WaitForSeconds(6);
            }
            if (main.SaveManager.IsLevelCleared(VanillaStageID.prologue))
            {
                var lastMapID = main.SaveManager.GetLastMapID() ?? main.ResourceManager.GetFirstMapID();
                main.Scene.DisplayMap(lastMapID);
            }
            else
            {
                var task = GotoPrologue();
                while (!task.IsCompleted)
                {
                    yield return null;
                }
            }
        }
        private IEnumerable<MainmenuButton> GetAllButtons()
        {
            return ui.GetAllButtons();
        }
        private async Task GotoPrologue()
        {
            await main.LevelManager.GotoLevelSceneAsync();
            main.LevelManager.InitLevel(VanillaAreaID.day, VanillaStageID.prologue);
            Hide();
        }
        private IEnumerator GotoDebugStage()
        {
            var task = main.LevelManager.GotoLevelSceneAsync();
            while (!task.IsCompleted)
                yield return null;
            main.LevelManager.InitLevel(VanillaAreaID.mausoleum, VanillaStageID.debug);
            Hide();
        }

        #region 用户管理
        private void DeleteUser(int userIndex)
        {
            try
            {
                var currentUserIndex = main.SaveManager.GetCurrentUserIndex();
                if (userIndex == currentUserIndex)
                {
                    // 要删除当前存档，需要切换至其他存档。
                    SwitchToOtherUserBeforeDelete(userIndex);
                    // 切换成功。
                    HideUserManageDialog();
                    Reload();
                    main.SaveManager.DeleteUser(userIndex);
                }
                else
                {
                    main.SaveManager.DeleteUser(userIndex);
                    RefreshUserManageDialog();
                }
                main.SaveManager.SaveUserList();
            }
            catch (Exception e)
            {
                var title = main.LanguageManager._(VanillaStrings.ERROR);
                var desc = main.LanguageManager._(ERROR_MESSAGE_UNABLE_TO_DELETE_USER, e.Message);
                main.Scene.ShowDialogMessage(title, desc);
                Debug.LogError($"Unable to delete user{userIndex}'s save data : {e}");
            }
        }
        private void RenameUser(int userIndex, string name)
        {
            main.SaveManager.SetUserName(userIndex, name);
            main.SaveManager.SaveUserList();
            RefreshUserManageDialog();
            var currentUserIndex = main.SaveManager.GetCurrentUserIndex();
            if (userIndex == currentUserIndex)
            {
                ui.SetUserName(name);
                ui.SetUserNameGold(main.SponsorManager.HasSponsorPlan(name, SponsorPlans.Furnace.TYPE, SponsorPlans.Furnace.BLAST_FURNACE));
            }
        }
        private void SwitchUser(int userIndex)
        {
            try
            {
                main.SaveManager.SaveModDatas();
                main.SaveManager.SetCurrentUserIndex(userIndex);
                main.SaveManager.SaveUserList();
                HideUserManageDialog();
                Reload();
            }
            catch (Exception e)
            {
                var title = main.LanguageManager._(VanillaStrings.ERROR);
                var desc = main.LanguageManager._(ERROR_MESSAGE_UNABLE_TO_SWITCH_TO_USER, e.Message);
                main.Scene.ShowDialogMessage(title, desc);
                Debug.LogError($"Unable to switch to user{userIndex}'s save data : {e}");
            }
        }
        private void SwitchToOtherUserBeforeDelete(int currentIndex)
        {
            // 先保存当前存档，以防出现错误。
            main.SaveManager.SaveModDatas();
            // 获取所有后备的可选其他存档。
            var backupUserIndexes = managingUserIndexes.Where(u => u != currentIndex);
            bool success = false;
            foreach (var nextUserIndex in backupUserIndexes)
            {
                try
                {
                    // 切换至其他存档。
                    main.SaveManager.SetCurrentUserIndex(nextUserIndex);
                    // 切换成功。
                    success = true;
                    break;
                }
                catch (Exception e)
                {
                    // 切换失败，换下一个。
                    Debug.LogError($"Unable to switch to user{nextUserIndex}'s save data while deleting user{currentIndex} : {e}");
                }
            }
            // 切换成功，或者全部切换失败。
            if (!success)
            {
                // 全部切换失败，重新读取当前存档，防止出现错误。
                main.SaveManager.LoadUserData(currentIndex);
                // 报错。
                var message = main.LanguageManager._(ERROR_MESSAGE_NO_SPARE_USERS_TO_SWITCH);
                throw new InvalidOperationException(message);
            }
        }
        private int GetSelectedUserIndex()
        {
            return managingUserIndexes[selectedUserArrayIndex];
        }
        private void RefreshUserManageDialog()
        {
            var users = main.SaveManager.GetAllUsers();
            var currentIndex = main.SaveManager.GetCurrentUserIndex();
            var userIndexes = users.Select((user, index) => (user, index)).Where(a => a.user != null).Select(p => p.index);
            var reorderedUserPairs = userIndexes.Where(i => i != currentIndex).Prepend(currentIndex);
            managingUserIndexes = reorderedUserPairs.ToArray();
            selectedUserArrayIndex = Array.IndexOf(managingUserIndexes, currentIndex);
            var names = new UserNameItemViewData[managingUserIndexes.Length];
            for (int i = 0; i < names.Length; i++)
            {
                var name = main.SaveManager.GetUserName(managingUserIndexes[i]);
                var isSpecialName = main.Game.IsSpecialUserName(name);
                names[i] = new UserNameItemViewData()
                {
                    name = name,
                    color = isSpecialName ? Color.red : Color.black
                };
            }
            ui.UpdateUserManageDialog(names, selectedUserArrayIndex);
            UpdateUserManageButtons();
        }
        private void HideUserManageDialog()
        {
            selectedUserArrayIndex = -1;
            managingUserIndexes = null;
            ui.SetUserManageDialogVisible(false);
        }
        private void UpdateUserManageButtons()
        {
            bool selected = selectedUserArrayIndex >= 0;
            bool hasEmptySlot = !IsUserFull();
            ui.SetUserManageCreateNewUserActive(hasEmptySlot);
            ui.SetUserManageButtonInteractable(UserManageDialog.ButtonType.Rename, selected);
            ui.SetUserManageButtonInteractable(UserManageDialog.ButtonType.Delete, selected && managingUserIndexes.Length >= 2);
            ui.SetUserManageButtonInteractable(UserManageDialog.ButtonType.Switch, selected && GetSelectedUserIndex() != main.SaveManager.GetCurrentUserIndex());
            ui.SetUserManageButtonInteractable(UserManageDialog.ButtonType.Import, hasEmptySlot);
            ui.SetUserManageButtonInteractable(UserManageDialog.ButtonType.Export, selected);
        }
        private bool IsUserFull()
        {
            return managingUserIndexes.Length >= SaveManager.MAX_USER_COUNT;
        }
        #endregion

        private void StartAnimatorTransition(Vector2 target)
        {
            animatorBlendStart = GetCurrentAnimatorBlend();
            animatorBlendEnd = target;
            animatorBlendTimeout = transitionTime;
            ui.SetRayblockerActive(true);
        }
        private Vector2 GetCurrentAnimatorBlend()
        {
            return Vector2.Lerp(animatorBlendStart, animatorBlendEnd, MathTool.EaseInAndOut((transitionTime - animatorBlendTimeout) / transitionTime));
        }
        private void UpdateWindowView()
        {
            var saves = main.SaveManager;
            var resources = main.ResourceManager;
            var viewsID = resources.GetAllMainmenuViews();
            var metas = viewsID.Select(id => resources.GetMainmenuViewMeta(id)).Where(m => m != null);
            var ordered = metas.OrderByDescending(m => m.Priority);
            Sprite sprite = null;
            int sheetIndex = isDark ? 1 : 0;
            foreach (var meta in ordered)
            {
                if (meta.Conditions == null || saves.MeetsXMLConditions(meta.Conditions))
                {
                    var spriteRef = new SpriteReference(meta.SpritesheetID, sheetIndex);
                    if (SpriteReference.IsValid(spriteRef))
                    {
                        sprite = main.GetFinalSprite(spriteRef);
                        break;
                    }
                }
            }
            ui.SetWindowViewSprite(sprite);
        }

        #region 统计
        private void ReloadStats()
        {
            var nsp = main.BuiltinNamespace;
            UserStats stats = main.SaveManager.GetUserStats(nsp);
            var categories = stats.GetAllCategories();
            var viewDatas = new StatCategoryViewData[categories.Length];
            for (int i = 0; i < viewDatas.Length; i++)
            {
                var category = categories[i];
                var meta = main.ResourceManager.GetStatCategoryMeta(new NamespaceID(nsp, category.Name));
                var metaName = meta?.Name ?? category.Name;
                var metaType = meta?.Type ?? StatCategoryType.Entity;
                var metaOperation = meta?.Operation ?? StatOperation.Sum;

                var title = main.LanguageManager._p(VanillaStrings.CONTEXT_STAT_CATEGORY, metaName);
                long categoryNumber = 0;
                switch (metaOperation)
                {
                    case StatOperation.Sum:
                        categoryNumber = category.GetSum();
                        break;
                    case StatOperation.Max:
                        categoryNumber = category.GetMax();
                        break;
                }
                var entries = category.GetAllEntries();
                var entriesViewData = new List<StatEntryViewData>();
                for (int j = 0; j < entries.Length; j++)
                {
                    var entry = entries[j];
                    var name = main.ResourceManager.GetStatEntryName(entry.ID, metaType);
                    var count = entry.Value;
                    entriesViewData.Add(new StatEntryViewData()
                    {
                        name = name,
                        count = count
                    });
                }
                viewDatas[i] = new StatCategoryViewData()
                {
                    entries = entriesViewData.OrderByDescending(e => e.count).ToArray(),
                    sum = categoryNumber.ToString(),
                    title = title
                };
            }
            ui.UpdateStats(viewDatas);
        }
        #endregion

        #region 成就
        private void ReloadAchievements()
        {
            var nsp = main.BuiltinNamespace;
            var metas = main.ResourceManager.GetModAchievementMetas(nsp);
            var viewDatas = new AchievementEntryViewData[metas.Length];
            for (int i = 0; i < viewDatas.Length; i++)
            {
                var meta = metas[i];
                if (meta == null)
                    continue;
                var iconRef = meta.Icon;
                var metaName = meta.Name;
                var metaDescription = meta.Description;

                var icon = main.GetFinalSprite(iconRef);
                var name = main.LanguageManager._p(VanillaStrings.CONTEXT_ACHIEVEMENT, metaName);
                var earned = main.SaveManager.IsAchievementEarned(new NamespaceID(nsp, meta.ID));
                var description = main.LanguageManager._p(VanillaStrings.CONTEXT_ACHIEVEMENT, metaDescription);
                viewDatas[i] = new AchievementEntryViewData()
                {
                    icon = icon,
                    name = name,
                    earned = earned,
                    description = description
                };
            }
            ui.UpdateAchievements(viewDatas);
        }
        #endregion

        #region 按键绑定
        private void UpdateKeybindingItems()
        {
            var viewDatas = new List<KeybindingItemViewData>();
            var conflictKeys = bindingKeys
                .Select(k => main.OptionsManager.GetKeyBinding(k))
                .GroupBy(k => k)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
            for (int i = 0; i < bindingKeys.Length; i++)
            {
                var id = bindingKeys[i];
                var keyCode = main.OptionsManager.GetKeyBinding(id);
                var conflict = conflictKeys.Contains(keyCode);
                var viewData = GetKeybindingItemViewData(i, conflict);
                viewDatas.Add(viewData);
            }
            ui.UpdateKeyBindingItems(viewDatas.ToArray());
        }
        private void UpdateKeybindingItem(int index)
        {
            var conflictKeys = bindingKeys
                .Select(k => main.OptionsManager.GetKeyBinding(k))
                .GroupBy(k => k)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
            var id = bindingKeys[index];
            var keyCode = main.OptionsManager.GetKeyBinding(id);
            var conflict = conflictKeys.Contains(keyCode);
            var viewData = GetKeybindingItemViewData(index, conflict);
            ui.UpdateKeyBindingItem(index, viewData);
        }
        private KeybindingItemViewData GetKeybindingItemViewData(int index, bool conflict)
        {
            var id = bindingKeys[index];
            var nameKey = main.OptionsManager.GetHotkeyNameKey(id);
            var name = main.LanguageManager._p(VanillaStrings.CONTEXT_HOTKEY_NAME, nameKey);

            var keyCode = main.OptionsManager.GetKeyBinding(id);
            var keyColor = Color.white;
            string keyName;
            if (bindingKeyIndex != index)
            {
                var key = main.InputManager.GetKeyCodeNameKey(keyCode);
                keyName = main.LanguageManager._p(InputManager.CONTEXT_KEY_NAME, key);
                keyColor = conflict ? Color.red : Color.white;
            }
            else
            {
                keyName = main.LanguageManager._(PRESS_KEY_HINT);
            }
            return new KeybindingItemViewData()
            {
                name = name,
                key = keyName,
                keyColor = keyColor
            };
        }
        private void UpdateKeybindingCheck()
        {
            if (bindingKeys == null)
                return;
            if (bindingKeyIndex < 0 || bindingKeyIndex >= bindingKeys.Length)
                return;
            if (!Input.anyKeyDown)
                return;
            KeyCode code = main.InputManager.GetCurrentPressedKey();
            if (code == KeyCode.None)
            {
                bindingKeyIndex = -1;
                UpdateKeybindingItems();
                return;
            }
            if (code == KeyCode.Escape)
            {
                code = KeyCode.None;
            }
            var id = bindingKeys[bindingKeyIndex];
            bindingKeyIndex = -1;
            main.OptionsManager.SetKeyBinding(id, code);
            UpdateKeybindingItems();
        }
        #endregion

        #region 属性字段
        [TranslateMsg("删除用户时的错误信息，{0}为错误信息")]
        public const string ERROR_MESSAGE_UNABLE_TO_DELETE_USER = "无法删除用户：{0}";
        [TranslateMsg("切换用户时的错误信息，{0}为错误信息")]
        public const string ERROR_MESSAGE_UNABLE_TO_SWITCH_TO_USER = "无法切换至用户：{0}";
        [TranslateMsg("删除用户时的错误信息")]
        public const string ERROR_MESSAGE_NO_SPARE_USERS_TO_SWITCH = "没有其他有效的用户可供切换。";
        [TranslateMsg("退出对话框的描述")]
        public const string QUIT_DESC = "确认要退出吗？";
        [TranslateMsg("重置所有按键绑定的警告")]
        public const string RESET_KEY_BINDINGS_WARNING = "确认要重置所有按键绑定吗？";
        [TranslateMsg("设置按键提醒")]
        public const string PRESS_KEY_HINT = "请按键";
        [TranslateMsg("存档导出失败的警告")]
        public const string ERROR_NOT_EXPORTED = "导出存档失败。";
        [TranslateMsg("存档导入失败的警告")]
        public const string ERROR_FAILED_TO_IMPORT = "导入存档失败。";
        [TranslateMsg("存档导入失败的警告")]
        public const string ERROR_CORRUPT_USER_DATA_PACK = "导入存档失败，文件可能已损坏。";
        [TranslateMsg("存档导入失败的警告")]
        public const string ERROR_DUPLICATE_IMPORTING_USER_NAME_AND_CANNOT_RENAME = "导入存档失败，游戏中存在同名用户，但正在导入的存档不可重命名。";
        [TranslateMsg("存档导出成功的提示，{0}为路径")]
        public const string HINT_EXPORTED = "存档已导出至{0}。";

        private Dictionary<MainmenuButtonType, Action> mainmenuActionDict = new Dictionary<MainmenuButtonType, Action>();
        private MainManager main => MainManager.Instance;
        [SerializeField]
        private MainmenuUI ui;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private float transitionTime = 1;
        [SerializeField]
        private Vector2 mainmenuBlend = new Vector2(0, 0);
        [SerializeField]
        private Vector2 basementBlend = new Vector2(0, -1);
        [SerializeField]
        private Vector2 statsBlend = new Vector2(-1, -1);
        [SerializeField]
        private Vector2 achievementsBlend = new Vector2(1, -1);

        private OptionsLogicMainmenu optionsLogic;
        private int[] managingUserIndexes;
        private int selectedUserArrayIndex = -1;
        private bool isDark;

        private Vector2 animatorBlendStart;
        private Vector2 animatorBlendEnd;
        private float animatorBlendTimeout;

        private NamespaceID[] bindingKeys;
        public int bindingKeyIndex;
        #endregion
    }
}
