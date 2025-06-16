﻿using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.UI;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Scenes
{
    public class KeybindingController : MonoBehaviour
    {
        #region 按键绑定
        public void Display()
        {
            bindingKeys = Main.OptionsManager.GetAllKeyBindings();
            bindingKeyIndex = -1;
            UpdateKeybindingItems();
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        #endregion

        #region 生命周期
        private void Awake()
        {
            ui.OnBackButtonClick += OnKeybindingReturnButtonClickCallback;
            ui.OnResetButtonClick += OnKeybindingResetButtonClickCallback;
            ui.OnItemButtonClick += OnKeybindingItemButtonClickCallback;
        }
        private void Update()
        {
            UpdateKeybindingCheck();
        }
        #endregion

        #region 事件回调
        private void OnKeybindingReturnButtonClickCallback()
        {
            bindingKeys = null;
            bindingKeyIndex = -1;
            Hide();
        }
        private void OnKeybindingResetButtonClickCallback()
        {
            var title = Main.LanguageManager._(VanillaStrings.WARNING);
            var desc = Main.LanguageManager._(RESET_KEY_BINDINGS_WARNING);
            Main.Scene.ShowDialogSelect(title, desc, (confirm) =>
            {
                if (confirm)
                {
                    Main.OptionsManager.ResetKeyBindings();
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

        #region 私有方法
        private void UpdateKeybindingItems()
        {
            var viewDatas = new List<KeybindingItemViewData>();
            var conflictKeys = bindingKeys
                .Select(k => Main.OptionsManager.GetKeyBinding(k))
                .GroupBy(k => k)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
            for (int i = 0; i < bindingKeys.Length; i++)
            {
                var id = bindingKeys[i];
                var keyCode = Main.OptionsManager.GetKeyBinding(id);
                var conflict = conflictKeys.Contains(keyCode);
                var viewData = GetKeybindingItemViewData(i, conflict);
                viewDatas.Add(viewData);
            }
            ui.UpdateItems(viewDatas.ToArray());
        }
        private void UpdateKeybindingItem(int index)
        {
            var conflictKeys = bindingKeys
                .Select(k => Main.OptionsManager.GetKeyBinding(k))
                .GroupBy(k => k)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
            var id = bindingKeys[index];
            var keyCode = Main.OptionsManager.GetKeyBinding(id);
            var conflict = conflictKeys.Contains(keyCode);
            var viewData = GetKeybindingItemViewData(index, conflict);
            ui.UpdateItem(index, viewData);
        }
        private KeybindingItemViewData GetKeybindingItemViewData(int index, bool conflict)
        {
            var id = bindingKeys[index];
            var nameKey = Main.OptionsManager.GetHotkeyNameKey(id);
            var name = Main.LanguageManager._p(VanillaStrings.CONTEXT_HOTKEY_NAME, nameKey);

            var keyCode = Main.OptionsManager.GetKeyBinding(id);
            var keyColor = Color.white;
            string keyName;
            if (bindingKeyIndex != index)
            {
                keyName = Main.InputManager.GetKeyCodeName(keyCode);
                keyColor = conflict ? Color.red : Color.white;
            }
            else
            {
                keyName = Main.LanguageManager._(PRESS_KEY_HINT);
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
            KeyCode code = Main.InputManager.GetCurrentPressedKey();
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
            Main.OptionsManager.SetKeyBinding(id, code);

            var level = Main.LevelManager.GetLevel();
            if (level)
            {
                level.UpdateHotkeyTexts();
            }
            UpdateKeybindingItems();
        }
        #endregion
        [TranslateMsg("设置按键提醒")]
        public const string PRESS_KEY_HINT = "请按键";
        [TranslateMsg("重置所有按键绑定的警告")]
        public const string RESET_KEY_BINDINGS_WARNING = "确认要重置所有按键绑定吗？";
        private MainManager Main => MainManager.Instance;
        [SerializeField]
        private KeybindingPage ui;
        private NamespaceID[] bindingKeys;
        private int bindingKeyIndex;
    }
}
