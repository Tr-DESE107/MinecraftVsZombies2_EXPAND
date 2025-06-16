﻿using System.Collections.Generic;
using MukioI18n;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class InputManager : MonoBehaviour
    {
        public void InitKeys()
        {
            keyInfos.Clear();
            AddKeyInfo(KeyCode.None, KEYNAME_NONE);
            AddKeyInfo(KeyCode.Backspace, KEYNAME_BACKSPACE);
            AddKeyInfo(KeyCode.Delete, KEYNAME_DELETE);
            AddKeyInfo(KeyCode.Tab, KEYNAME_TAB);
            AddKeyInfo(KeyCode.Clear, KEYNAME_CLEAR);
            AddKeyInfo(KeyCode.Return, KEYNAME_RETURN);
            AddKeyInfo(KeyCode.Pause, KEYNAME_PAUSE);
            AddKeyInfo(KeyCode.Escape, KEYNAME_ESCAPE);
            AddKeyInfo(KeyCode.Space, KEYNAME_SPACE);

            AddKeyInfo(KeyCode.Keypad0, KEYNAME_KEYPAD0);
            AddKeyInfo(KeyCode.Keypad1, KEYNAME_KEYPAD1);
            AddKeyInfo(KeyCode.Keypad2, KEYNAME_KEYPAD2);
            AddKeyInfo(KeyCode.Keypad3, KEYNAME_KEYPAD3);
            AddKeyInfo(KeyCode.Keypad4, KEYNAME_KEYPAD4);
            AddKeyInfo(KeyCode.Keypad5, KEYNAME_KEYPAD5);
            AddKeyInfo(KeyCode.Keypad6, KEYNAME_KEYPAD6);
            AddKeyInfo(KeyCode.Keypad7, KEYNAME_KEYPAD7);
            AddKeyInfo(KeyCode.Keypad8, KEYNAME_KEYPAD8);
            AddKeyInfo(KeyCode.Keypad9, KEYNAME_KEYPAD9);
            AddKeyInfo(KeyCode.KeypadPeriod, KEYNAME_KEYPADPERIOD);
            AddKeyInfo(KeyCode.KeypadDivide, KEYNAME_KEYPADDIVIDE);
            AddKeyInfo(KeyCode.KeypadMultiply, KEYNAME_KEYPADMULTIPLY);
            AddKeyInfo(KeyCode.KeypadMinus, KEYNAME_KEYPADMINUS);
            AddKeyInfo(KeyCode.KeypadPlus, KEYNAME_KEYPADPLUS);
            AddKeyInfo(KeyCode.KeypadEnter, KEYNAME_KEYPADENTER);
            AddKeyInfo(KeyCode.KeypadEquals, KEYNAME_KEYPADEQUALS);

            AddKeyInfo(KeyCode.UpArrow, KEYNAME_UPARROW);
            AddKeyInfo(KeyCode.DownArrow, KEYNAME_DOWNARROW);
            AddKeyInfo(KeyCode.RightArrow, KEYNAME_RIGHTARROW);
            AddKeyInfo(KeyCode.LeftArrow, KEYNAME_LEFTARROW);

            AddKeyInfo(KeyCode.Insert, KEYNAME_INSERT);
            AddKeyInfo(KeyCode.Home, KEYNAME_HOME);
            AddKeyInfo(KeyCode.End, KEYNAME_END);
            AddKeyInfo(KeyCode.PageUp, KEYNAME_PAGEUP);
            AddKeyInfo(KeyCode.PageDown, KEYNAME_PAGEDOWN);

            AddKeyInfo(KeyCode.F1, KEYNAME_F1);
            AddKeyInfo(KeyCode.F2, KEYNAME_F2);
            AddKeyInfo(KeyCode.F3, KEYNAME_F3);
            AddKeyInfo(KeyCode.F4, KEYNAME_F4);
            AddKeyInfo(KeyCode.F5, KEYNAME_F5);
            AddKeyInfo(KeyCode.F6, KEYNAME_F6);
            AddKeyInfo(KeyCode.F7, KEYNAME_F7);
            AddKeyInfo(KeyCode.F8, KEYNAME_F8);
            AddKeyInfo(KeyCode.F9, KEYNAME_F9);
            AddKeyInfo(KeyCode.F10, KEYNAME_F10);
            AddKeyInfo(KeyCode.F11, KEYNAME_F11);
            AddKeyInfo(KeyCode.F12, KEYNAME_F12);
            AddKeyInfo(KeyCode.F13, KEYNAME_F13);
            AddKeyInfo(KeyCode.F14, KEYNAME_F14);
            AddKeyInfo(KeyCode.F15, KEYNAME_F15);

            AddKeyInfo(KeyCode.Alpha0, KEYNAME_ALPHA0);
            AddKeyInfo(KeyCode.Alpha1, KEYNAME_ALPHA1);
            AddKeyInfo(KeyCode.Alpha2, KEYNAME_ALPHA2);
            AddKeyInfo(KeyCode.Alpha3, KEYNAME_ALPHA3);
            AddKeyInfo(KeyCode.Alpha4, KEYNAME_ALPHA4);
            AddKeyInfo(KeyCode.Alpha5, KEYNAME_ALPHA5);
            AddKeyInfo(KeyCode.Alpha6, KEYNAME_ALPHA6);
            AddKeyInfo(KeyCode.Alpha7, KEYNAME_ALPHA7);
            AddKeyInfo(KeyCode.Alpha8, KEYNAME_ALPHA8);
            AddKeyInfo(KeyCode.Alpha9, KEYNAME_ALPHA9);

            AddKeyInfo(KeyCode.Exclaim, KEYNAME_EXCLAIM);
            AddKeyInfo(KeyCode.DoubleQuote, KEYNAME_DOUBLEQUOTE);
            AddKeyInfo(KeyCode.Hash, KEYNAME_HASH);
            AddKeyInfo(KeyCode.Dollar, KEYNAME_DOLLAR);
            AddKeyInfo(KeyCode.Percent, KEYNAME_PERCENT);
            AddKeyInfo(KeyCode.Ampersand, KEYNAME_AMPERSAND);
            AddKeyInfo(KeyCode.Quote, KEYNAME_QUOTE);
            AddKeyInfo(KeyCode.LeftParen, KEYNAME_LEFTPAREN);
            AddKeyInfo(KeyCode.RightParen, KEYNAME_RIGHTPAREN);
            AddKeyInfo(KeyCode.Asterisk, KEYNAME_ASTERISK);
            AddKeyInfo(KeyCode.Plus, KEYNAME_PLUS);
            AddKeyInfo(KeyCode.Comma, KEYNAME_COMMA);
            AddKeyInfo(KeyCode.Minus, KEYNAME_MINUS);
            AddKeyInfo(KeyCode.Period, KEYNAME_PERIOD);
            AddKeyInfo(KeyCode.Slash, KEYNAME_SLASH);
            AddKeyInfo(KeyCode.Colon, KEYNAME_COLON);
            AddKeyInfo(KeyCode.Semicolon, KEYNAME_SEMICOLON);
            AddKeyInfo(KeyCode.Less, KEYNAME_LESS);
            AddKeyInfo(KeyCode.Equals, KEYNAME_EQUALS);
            AddKeyInfo(KeyCode.Greater, KEYNAME_GREATER);
            AddKeyInfo(KeyCode.Question, KEYNAME_QUESTION);
            AddKeyInfo(KeyCode.At, KEYNAME_AT);

            AddKeyInfo(KeyCode.LeftBracket, KEYNAME_LEFTBRACKET);
            AddKeyInfo(KeyCode.Backslash, KEYNAME_BACKSLASH);
            AddKeyInfo(KeyCode.RightBracket, KEYNAME_RIGHTBRACKET);
            AddKeyInfo(KeyCode.Caret, KEYNAME_CARET);
            AddKeyInfo(KeyCode.Underscore, KEYNAME_UNDERSCORE);
            AddKeyInfo(KeyCode.BackQuote, KEYNAME_BACKQUOTE);
            AddKeyInfo(KeyCode.A, KEYNAME_A);
            AddKeyInfo(KeyCode.B, KEYNAME_B);
            AddKeyInfo(KeyCode.C, KEYNAME_C);
            AddKeyInfo(KeyCode.D, KEYNAME_D);
            AddKeyInfo(KeyCode.E, KEYNAME_E);
            AddKeyInfo(KeyCode.F, KEYNAME_F);
            AddKeyInfo(KeyCode.G, KEYNAME_G);
            AddKeyInfo(KeyCode.H, KEYNAME_H);
            AddKeyInfo(KeyCode.I, KEYNAME_I);
            AddKeyInfo(KeyCode.J, KEYNAME_J);
            AddKeyInfo(KeyCode.K, KEYNAME_K);
            AddKeyInfo(KeyCode.L, KEYNAME_L);
            AddKeyInfo(KeyCode.M, KEYNAME_M);
            AddKeyInfo(KeyCode.N, KEYNAME_N);
            AddKeyInfo(KeyCode.O, KEYNAME_O);
            AddKeyInfo(KeyCode.P, KEYNAME_P);
            AddKeyInfo(KeyCode.Q, KEYNAME_Q);
            AddKeyInfo(KeyCode.R, KEYNAME_R);
            AddKeyInfo(KeyCode.S, KEYNAME_S);
            AddKeyInfo(KeyCode.T, KEYNAME_T);
            AddKeyInfo(KeyCode.U, KEYNAME_U);
            AddKeyInfo(KeyCode.V, KEYNAME_V);
            AddKeyInfo(KeyCode.W, KEYNAME_W);
            AddKeyInfo(KeyCode.X, KEYNAME_X);
            AddKeyInfo(KeyCode.Y, KEYNAME_Y);
            AddKeyInfo(KeyCode.Z, KEYNAME_Z);

            AddKeyInfo(KeyCode.LeftCurlyBracket, KEYNAME_LEFTCURLYBRACKET);
            AddKeyInfo(KeyCode.Pipe, KEYNAME_PIPE);
            AddKeyInfo(KeyCode.RightCurlyBracket, KEYNAME_RIGHTCURLYBRACKET);
            AddKeyInfo(KeyCode.Tilde, KEYNAME_TILDE);

            AddKeyInfo(KeyCode.Numlock, KEYNAME_NUMLOCK);
            AddKeyInfo(KeyCode.CapsLock, KEYNAME_CAPSLOCK);
            AddKeyInfo(KeyCode.ScrollLock, KEYNAME_SCROLLLOCK);
            AddKeyInfo(KeyCode.RightShift, KEYNAME_RIGHTSHIFT);
            AddKeyInfo(KeyCode.LeftShift, KEYNAME_LEFTSHIFT);
            AddKeyInfo(KeyCode.RightControl, KEYNAME_RIGHTCONTROL);
            AddKeyInfo(KeyCode.LeftControl, KEYNAME_LEFTCONTROL);
            AddKeyInfo(KeyCode.RightAlt, KEYNAME_RIGHTALT);
            AddKeyInfo(KeyCode.LeftAlt, KEYNAME_LEFTALT);
            AddKeyInfo(KeyCode.LeftCommand, KEYNAME_LEFTCOMMAND);
            AddKeyInfo(KeyCode.LeftWindows, KEYNAME_LEFTWINDOWS);
            AddKeyInfo(KeyCode.RightCommand, KEYNAME_RIGHTCOMMAND);
            AddKeyInfo(KeyCode.RightWindows, KEYNAME_RIGHTWINDOWS);
            AddKeyInfo(KeyCode.AltGr, KEYNAME_ALTGR);
            AddKeyInfo(KeyCode.Print, KEYNAME_PRINT);
            AddKeyInfo(KeyCode.Menu, KEYNAME_MENU);
        }
        public string GetKeyCodeNameKey(KeyCode keycode)
        {
            var info = GetKeyCodeInfo(keycode);
            if (info == null)
                return KEYNAME_UNKNOWN;
            return info.name;
        }
        public string GetKeyCodeName(KeyCode keycode)
        {
            var nameKey = GetKeyCodeNameKey(keycode);
            return Main.LanguageManager._p(InputManager.CONTEXT_KEY_NAME, nameKey);
        }
        public KeyCode GetCurrentPressedKey()
        {
            foreach (var pair in keyInfos)
            {
                if (Input.GetKeyDown(pair.Key))
                    return pair.Key;
            }
            return KeyCode.None;
        }
        public KeyCodeInfo GetKeyCodeInfo(KeyCode keycode)
        {
            if (keyInfos.TryGetValue(keycode, out var info))
            {
                return info;
            }
            return null;
        }
        private void AddKeyInfo(KeyCode code, string name)
        {
            keyInfos[code] = new KeyCodeInfo(code, name);
        }
        public readonly Dictionary<KeyCode, KeyCodeInfo> keyInfos = new Dictionary<KeyCode, KeyCodeInfo>();

        public const string CONTEXT_KEY_NAME = "key.name";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_UNKNOWN = "未知";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_NONE = "无";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_BACKSPACE = "Backspace";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_DELETE = "Delete";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_TAB = "Tab";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_CLEAR = "Clear";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RETURN = "Enter";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_PAUSE = "Pause";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ESCAPE = "Escape";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_SPACE = "Space";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD0 = "小键盘 0";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD1 = "小键盘 1";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD2 = "小键盘 2";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD3 = "小键盘 3";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD4 = "小键盘 4";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD5 = "小键盘 5";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD6 = "小键盘 6";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD7 = "小键盘 7";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD8 = "小键盘 8";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPAD9 = "小键盘 9";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPADPERIOD = "小键盘 .";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPADDIVIDE = "小键盘 /";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPADMULTIPLY = "小键盘 *";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPADMINUS = "小键盘 -";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPADPLUS = "小键盘 +";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPADENTER = "小键盘 Enter";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_KEYPADEQUALS = "小键盘 =";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_UPARROW = "↑";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_DOWNARROW = "↓";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RIGHTARROW = "→";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LEFTARROW = "←";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_INSERT = "Insert";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_HOME = "Home";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_END = "End";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_PAGEUP = "PageUp";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_PAGEDOWN = "PageDown";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F1 = "F1";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F2 = "F2";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F3 = "F3";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F4 = "F4";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F5 = "F5";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F6 = "F6";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F7 = "F7";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F8 = "F8";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F9 = "F9";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F10 = "F10";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F11 = "F11";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F12 = "F12";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F13 = "F13";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F14 = "F14";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F15 = "F15";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA0 = "0";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA1 = "1";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA2 = "2";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA3 = "3";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA4 = "4";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA5 = "5";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA6 = "6";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA7 = "7";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA8 = "8";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALPHA9 = "9";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_EXCLAIM = "!";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_DOUBLEQUOTE = "\"";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_HASH = "#";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_DOLLAR = "$";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_PERCENT = "%";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_AMPERSAND = "&";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_QUOTE = "'";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LEFTPAREN = "(";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RIGHTPAREN = ")";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ASTERISK = "*";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_PLUS = "+";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_COMMA = ",";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_MINUS = "-";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_PERIOD = ".";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_SLASH = "/";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_COLON = ":";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_SEMICOLON = ";";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LESS = "<";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_EQUALS = "=";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_GREATER = ">";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_QUESTION = "?";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_AT = "@";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LEFTBRACKET = "[";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_BACKSLASH = "\\";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RIGHTBRACKET = "]";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_CARET = "^";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_UNDERSCORE = "_";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_BACKQUOTE = "`";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_A = "A";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_B = "B";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_C = "C";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_D = "D";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_E = "E";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_F = "F";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_G = "G";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_H = "H";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_I = "I";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_J = "J";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_K = "K";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_L = "L";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_M = "M";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_N = "N";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_O = "O";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_P = "P";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_Q = "Q";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_R = "R";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_S = "S";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_T = "T";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_U = "U";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_V = "V";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_W = "W";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_X = "X";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_Y = "Y";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_Z = "Z";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LEFTCURLYBRACKET = "{";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_PIPE = "|";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RIGHTCURLYBRACKET = "}";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_TILDE = "~";

        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_NUMLOCK = "Numlock";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_CAPSLOCK = "CapsLock";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_SCROLLLOCK = "ScrollLock";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RIGHTSHIFT = "右Shift";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LEFTSHIFT = "左Shift";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RIGHTCONTROL = "右Ctrl";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LEFTCONTROL = "左Ctrl";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RIGHTALT = "右Alt";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LEFTALT = "左Alt";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LEFTCOMMAND = "左Command";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_LEFTWINDOWS = "左Windows";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RIGHTCOMMAND = "右Command";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_RIGHTWINDOWS = "右Windows";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_ALTGR = "AltGr";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_PRINT = "Print";
        [TranslateMsg("按键名", CONTEXT_KEY_NAME)] public const string KEYNAME_MENU = "Menu";
    }
    public class KeyCodeInfo
    {
        public KeyCodeInfo(KeyCode code, string name)
        {
            this.code = code;
            this.name = name;
        }
        public KeyCode code;
        public string name;
    }
}
