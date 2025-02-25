using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Localization;
using MVZ2.Managers;
using MVZ2.Scenes;
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
                        addons.SetLoadingVisible(true);
                        await Main.LanguageManager.ReloadLanguagePacks(enabledReferences.ToArray());
                        addons.SetLoadingVisible(false);
                    }
                    Hide();
                    addons.DisplayIndex();
                    break;
                case LanguagePacksUI.Buttons.Import:
                    // TODO
                    break;
                case LanguagePacksUI.Buttons.Export:
                    // TODO
                    break;
                case LanguagePacksUI.Buttons.Delete:
                    // TODO
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
            ui.SetButtonInteractable(LanguagePacksUI.Buttons.Export, selected && !isBuiltin);
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
            return !enabledReferences.SequenceEqual(Main.LanguageManager.GetEnabledLanguagePackList());
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
        public MainManager Main => MainManager.Instance;
        [SerializeField]
        private AddonsController addons;
        [SerializeField]
        private LanguagePacksUI ui;
        [SerializeField]
        private float maxRefreshInterval = 1;
        private LanguagePackReference selectedLanguagePack;
        private List<LanguagePackReference> enabledReferences = new List<LanguagePackReference>();
        private List<LanguagePackReference> references = new List<LanguagePackReference>();
        private float refreshInterval;
        
    }
}
