﻿using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Options
{
    public partial class OptionsManager
    {
        #region 按键绑定
        public KeyCode GetKeyBinding(NamespaceID hotkey)
        {
            if (options.keyBindings.TryGetKeyBinding(hotkey, out var code))
            {
                return code;
            }
            return GetDefaultKeyBinding(hotkey);
        }
        public KeyCode GetBlueprintKeyBinding(int index)
        {
            var keyID = HotKeys.GetBlueprintHotKey(index);
            return GetKeyBinding(keyID);
        }
        public void SetKeyBinding(NamespaceID hotkey, KeyCode code)
        {
            options.keyBindings.SetKeyBinding(hotkey, code);
            SaveOptionsToFile();
        }
        public void ResetKeyBindings()
        {
            options.keyBindings.Reset();
            SaveOptionsToFile();
        }
        public string GetHotkeyNameKey(NamespaceID hotkey)
        {
            var meta = GetKeybindingMeta(hotkey);
            if (meta == null)
                return HOTKEY_NAME_UNKNOWN;
            return meta.Name;
        }
        public KeyCode GetDefaultKeyBinding(NamespaceID hotkey)
        {
            var meta = GetKeybindingMeta(hotkey);
            if (meta == null)
                return KeyCode.None;
            return meta.DefaultCode;
        }
        public NamespaceID[] GetAllKeyBindings()
        {
            return keyBindingMetas.Keys.ToArray();
        }
        public KeyBindingMeta GetKeybindingMeta(NamespaceID hotkey)
        {
            if (keyBindingMetas.TryGetValue(hotkey, out var meta))
            {
                return meta;
            }
            return null;
        }
        private void InitKeyBindings()
        {
            AddKeyBindingMeta(HotKeys.pickaxe, KeyCode.Q, HOTKEY_NAME_PICKAXE);
            AddKeyBindingMeta(HotKeys.starshard, KeyCode.W, HOTKEY_NAME_STARSHARD);
            AddKeyBindingMeta(HotKeys.trigger, KeyCode.BackQuote, HOTKEY_NAME_TRIGGER);
            AddKeyBindingMeta(HotKeys.fastForward, KeyCode.F, HOTKEY_NAME_FASTFORWARD);
            AddKeyBindingMeta(HotKeys.blueprint1, KeyCode.Alpha1, HOTKEY_NAME_BLUEPRINT1);
            AddKeyBindingMeta(HotKeys.blueprint2, KeyCode.Alpha2, HOTKEY_NAME_BLUEPRINT2);
            AddKeyBindingMeta(HotKeys.blueprint3, KeyCode.Alpha3, HOTKEY_NAME_BLUEPRINT3);
            AddKeyBindingMeta(HotKeys.blueprint4, KeyCode.Alpha4, HOTKEY_NAME_BLUEPRINT4);
            AddKeyBindingMeta(HotKeys.blueprint5, KeyCode.Alpha5, HOTKEY_NAME_BLUEPRINT5);
            AddKeyBindingMeta(HotKeys.blueprint6, KeyCode.Alpha6, HOTKEY_NAME_BLUEPRINT6);
            AddKeyBindingMeta(HotKeys.blueprint7, KeyCode.Alpha7, HOTKEY_NAME_BLUEPRINT7);
            AddKeyBindingMeta(HotKeys.blueprint8, KeyCode.Alpha8, HOTKEY_NAME_BLUEPRINT8);
            AddKeyBindingMeta(HotKeys.blueprint9, KeyCode.Alpha9, HOTKEY_NAME_BLUEPRINT9);
            AddKeyBindingMeta(HotKeys.blueprint10, KeyCode.Alpha0, HOTKEY_NAME_BLUEPRINT10);
        }
        private void AddKeyBindingMeta(NamespaceID hotkey, KeyCode defaultCode, string name)
        {
            keyBindingMetas.Add(hotkey, new KeyBindingMeta(hotkey, defaultCode, name));
        }
        private Dictionary<NamespaceID, KeyBindingMeta> keyBindingMetas = new Dictionary<NamespaceID, KeyBindingMeta>();

        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_UNKNOWN = "？？？";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_PICKAXE = "铁镐";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_STARSHARD = "星之碎片";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_TRIGGER = "触发器";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_FASTFORWARD = "快进";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT1 = "蓝图1";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT2 = "蓝图2";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT3 = "蓝图3";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT4 = "蓝图4";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT5 = "蓝图5";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT6 = "蓝图6";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT7 = "蓝图7";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT8 = "蓝图8";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT9 = "蓝图9";
        [TranslateMsg("按键名", VanillaStrings.CONTEXT_HOTKEY_NAME)]
        public const string HOTKEY_NAME_BLUEPRINT10 = "蓝图10";
        #endregion
    }
}
