﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.IO;
using MVZ2.Localization;
using MVZ2.Managers;
using MVZ2.Vanilla;
using UnityEngine;

namespace MVZ2.Addons
{
    public class LanguagePacksController : MonoBehaviour
    {
        public async Task Display()
        {
            // 加载语言包引用。
            await RefreshLanguagePacks();
            references.Clear();
            references.AddRange(Main.LanguageManager.GetAllLanguagePackReferences());
            var originPreference = Main.LanguageManager.GetEnabledLanguagePackList();
            enabledReferences.Clear();
            enabledReferences.AddRange(originPreference);

            gameObject.SetActive(true);

            // 更新UI。
            CancelSelection();
            UpdateLanguagePacks();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        #region 生命周期
        private void Awake()
        {
            ui.OnButtonClick += OnButtonClickCallback;
            ui.OnPackItemToggled += OnPackItemToggledCallback;
        }
        private async void Update()
        {
            refreshInterval -= Time.deltaTime;
            if (refreshInterval <= 0)
            {
                await RefreshLanguagePacks();
            }
        }
        #endregion

        #region 事件回调
        private async void OnButtonClickCallback(LanguagePacksUI.Buttons button)
        {
            switch (button)
            {
                case LanguagePacksUI.Buttons.Return:
                    if (IsDirty())
                    {
                        overridedFile = false;
                        addons.SetLoadingVisible(true);
                        await Main.LanguageManager.ReloadLanguagePacks(enabledReferences.ToArray());
                        addons.SetLoadingVisible(false);
                    }
                    Hide();
                    addons.DisplayIndex();
                    break;
                case LanguagePacksUI.Buttons.Import:
                    {
                        await FileHelper.OpenExternalFile(new string[] { "zip" }, importAction);
                        async void importAction(string path)
                        {
                            addons.SetLoadingVisible(true);
                            try
                            {
                                var key = Main.LanguageManager.GetImportKey(path);
                                if (!string.IsNullOrEmpty(key))
                                {
                                    var reference = references.FirstOrDefault(r => r.GetKey() == key);
                                    if (reference != null)
                                    {
                                        var metadata = Main.LanguageManager.GetLanguagePackMetadata(reference);
                                        var languagePackName = metadata?.name ?? key;
                                        var title = Main.LanguageManager._(VanillaStrings.WARNING);
                                        var desc = Main.LanguageManager._(WARNING_OVERRIDE_LANGUAGE_PACK, languagePackName);
                                        var result = await Main.Scene.ShowDialogSelectAsync(title, desc);
                                        if (!result)
                                        {
                                            return;
                                        }
                                        Main.LanguageManager.DeleteLanguagePack(reference);
                                        overridedFile = true;
                                    }
                                    if (!await Main.LanguageManager.ValidateLanguagePack(path))
                                    {
                                        var title = Main.LanguageManager._(VanillaStrings.ERROR);
                                        var desc = Main.LanguageManager._(ERROR_FAILED_TO_IMPORT);
                                        Main.Scene.ShowDialogMessage(title, desc);
                                        return;
                                    }
                                    Main.LanguageManager.ImportLanguagePack(path);
                                    await RefreshLanguagePacks();
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"导入语言包时出现错误：{e}");
                            }
                            finally
                            {
                                addons.SetLoadingVisible(false);
                            }
                        }
                    }
                    break;
                case LanguagePacksUI.Buttons.Export:
                    {
                        var selected = selectedLanguagePack;
                        if (selected != null)
                        {
                            var key = selected.GetKey();
                            bool success = false;
                            var fileName = Path.GetFileNameWithoutExtension(selected.GetFileName());
                            var path = await FileHelper.SaveExternalFile(fileName, new string[] { "zip" }, async dest =>
                            {
                                if (!references.Contains(selected))
                                    return;
                                success = await Main.LanguageManager.ExportLanguagePack(selected, dest);
                            });
                            if (string.IsNullOrEmpty(path))
                                break;
                            if (!success)
                            {
                                var title = Main.LanguageManager._(VanillaStrings.ERROR);
                                var desc = Main.LanguageManager._(ERROR_NOT_SAVED);
                                await Main.Scene.ShowDialogMessageAsync(title, desc);
                            }
                            else
                            {
                                var title = Main.LanguageManager._(VanillaStrings.HINT);
                                var desc = Main.LanguageManager._(HINT_SAVED, path);
                                await Main.Scene.ShowDialogMessageAsync(title, desc);
                            }
                        }
                    }
                    break;
                case LanguagePacksUI.Buttons.Delete:
                    {
                        var selected = selectedLanguagePack;
                        if (selected != null)
                        {
                            var key = selected.GetFileName();
                            var metadata = Main.LanguageManager.GetLanguagePackMetadata(selected);
                            var languagePackName = metadata?.name ?? key;
                            var title = Main.LanguageManager._(VanillaStrings.WARNING);
                            var desc = Main.LanguageManager._(WARNING_DELETE_LANGUAGE_PACK, languagePackName);
                            var result = await Main.Scene.ShowDialogSelectAsync(title, desc);
                            if (result)
                            {
                                Main.LanguageManager.DeleteLanguagePack(selected);
                                await RefreshLanguagePacks();
                            }
                        }
                    }
                    break;
                case LanguagePacksUI.Buttons.Disable:
                    if (selectedLanguagePack != null)
                    {
                        enabledReferences.Remove(selectedLanguagePack);
                        UpdateLanguagePacks();
                        UpdateButtonInteractions();
                        ui.SelectItemUI(false, GetItemUIIndex(false, selectedLanguagePack));
                    }
                    break;
                case LanguagePacksUI.Buttons.Enable:
                    if (selectedLanguagePack != null && !enabledReferences.Contains(selectedLanguagePack))
                    {
                        enabledReferences.Insert(0, selectedLanguagePack);
                        UpdateLanguagePacks();
                        UpdateButtonInteractions();
                        ui.SelectItemUI(true, GetItemUIIndex(true, selectedLanguagePack));
                    }
                    break;
                case LanguagePacksUI.Buttons.MoveUp:
                    if (selectedLanguagePack != null && enabledReferences.Contains(selectedLanguagePack))
                    {
                        var index = enabledReferences.IndexOf(selectedLanguagePack);
                        if (index > 0)
                        {
                            enabledReferences.RemoveAt(index);
                            enabledReferences.Insert(index - 1, selectedLanguagePack);
                            UpdateLanguagePacks();
                            UpdateButtonInteractions();
                            ui.SelectItemUI(true, GetItemUIIndex(true, selectedLanguagePack));
                        }
                    }
                    break;
                case LanguagePacksUI.Buttons.MoveDown:
                    if (selectedLanguagePack != null && enabledReferences.Contains(selectedLanguagePack))
                    {
                        var index = enabledReferences.IndexOf(selectedLanguagePack);
                        if (index < enabledReferences.Count - 1)
                        {
                            enabledReferences.RemoveAt(index);
                            enabledReferences.Insert(index + 1, selectedLanguagePack);
                            UpdateLanguagePacks();
                            UpdateButtonInteractions();
                            ui.SelectItemUI(true, GetItemUIIndex(true, selectedLanguagePack));
                        }
                    }
                    break;
            }
        }
        private void OnPackItemToggledCallback(bool enabled, int index, bool value)
        {
            var reference = GetLanguagePackReferenceByUI(enabled, index);
            if (value)
            {
                selectedLanguagePack = reference;
                UpdateButtonInteractions();
            }
            else if (selectedLanguagePack == reference)
            {
                selectedLanguagePack = null;
                UpdateButtonInteractions();
            }
        }
        #endregion

        private async Task RefreshLanguagePacks()
        {
            refreshInterval = maxRefreshInterval;
            bool changed = await Main.LanguageManager.RefreshLanguagePackReferences();
            if (changed)
            {
                references.Clear();
                references.AddRange(Main.LanguageManager.GetAllLanguagePackReferences());
                enabledReferences.RemoveAll(r => !references.Contains(r));
                UpdateLanguagePacks();
                CancelSelection();
            }
        }
        private void CancelSelection()
        {
            selectedLanguagePack = null;
            ui.DeselectAll();
            UpdateButtonInteractions();
        }
        private void UpdateLanguagePacks()
        {
            var disabled = GetDisabledReferences();
            var disabledViewDatas = disabled.Select(p => GetLanguagePackViewData(p)).ToArray();
            var enabledViewDatas = enabledReferences.Select(p => GetLanguagePackViewData(p)).ToArray();
            ui.SetDisabledLanguagePacks(disabledViewDatas);
            ui.SetEnabledLanguagePacks(enabledViewDatas);
        }
        private void UpdateButtonInteractions()
        {
            bool selected = selectedLanguagePack != null;
            bool isBuiltin = selected && selectedLanguagePack.IsBuiltin;
            bool enabled = selected && enabledReferences.Contains(selectedLanguagePack);
            int index = selected ? enabledReferences.IndexOf(selectedLanguagePack) : -1;
            ui.SetButtonInteractable(LanguagePacksUI.Buttons.Delete, selected && !isBuiltin);
            ui.SetButtonInteractable(LanguagePacksUI.Buttons.Export, selected);
            ui.SetButtonInteractable(LanguagePacksUI.Buttons.Disable, enabled && selected && !isBuiltin);
            ui.SetButtonInteractable(LanguagePacksUI.Buttons.Enable, !enabled && selected && !isBuiltin);
            ui.SetButtonInteractable(LanguagePacksUI.Buttons.MoveUp, enabled && selected && index > 0);
            ui.SetButtonInteractable(LanguagePacksUI.Buttons.MoveDown, enabled && selected && index < enabledReferences.Count - 1);
        }
        private IEnumerable<LanguagePackReference> GetDisabledReferences()
        {
            return references.Except(enabledReferences);
        }
        private bool IsDirty()
        {
            return overridedFile || !enabledReferences.SequenceEqual(Main.LanguageManager.GetEnabledLanguagePackList());
        }
        private LanguagePackReference GetLanguagePackReferenceByUI(bool enabled, int index)
        {
            if (enabled)
            {
                return enabledReferences[index];
            }
            else
            {
                var disabled = GetDisabledReferences();
                return disabled.ElementAtOrDefault(index);
            }
        }
        private int GetItemUIIndex(bool enabled, LanguagePackReference reference)
        {
            if (enabled)
            {
                return enabledReferences.IndexOf(reference);
            }
            else
            {
                var disabled = GetDisabledReferences().ToArray();
                return Array.IndexOf(disabled, reference);
            }
        }
        private LanguagePackViewData GetLanguagePackViewData(LanguagePackReference reference)
        {
            var metadata = Main.LanguageManager.GetLanguagePackMetadata(reference);
            return new LanguagePackViewData()
            {
                name = metadata.name,
                description = metadata.description,
                icon = metadata.icon,
            };
        }
        [TranslateMsg("覆盖语言包的警告，{0}为语言包名称")]
        public const string WARNING_OVERRIDE_LANGUAGE_PACK = "已经存在同名语言包“{0}”，是否覆盖？";
        [TranslateMsg("删除语言包的警告，{0}为语言包名称")]
        public const string WARNING_DELETE_LANGUAGE_PACK = "是否删除语言包“{0}”？";
        [TranslateMsg("语言包导出失败的警告")]
        public const string ERROR_NOT_SAVED = "导出语言包失败。";
        [TranslateMsg("语言包导入失败的警告")]
        public const string ERROR_FAILED_TO_IMPORT = "导入语言包失败。";
        [TranslateMsg("语言包导出成功的提示，{0}为路径")]
        public const string HINT_SAVED = "语言包已导出至{0}。";

        public MainManager Main => MainManager.Instance;
        [SerializeField]
        private AddonsController addons;
        [SerializeField]
        private LanguagePacksUI ui;
        [SerializeField]
        private float maxRefreshInterval = 1;
        private bool overridedFile;
        private LanguagePackReference selectedLanguagePack;
        private List<LanguagePackReference> enabledReferences = new List<LanguagePackReference>();
        private List<LanguagePackReference> references = new List<LanguagePackReference>();
        private float refreshInterval;

    }
}
