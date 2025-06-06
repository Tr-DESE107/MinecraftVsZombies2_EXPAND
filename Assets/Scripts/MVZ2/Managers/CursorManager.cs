﻿using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Cursors
{
    public class CursorManager : MonoBehaviour
    {
        public static void AddSource(CursorSource source)
        {
            var instance = MainManager.Instance.CursorManager;
            if (!instance)
                return;
            instance.AddCursorSource(source);
        }
        public static bool RemoveSource(CursorSource source)
        {
            var instance = MainManager.Instance.CursorManager;
            if (!instance)
                return false;
            return instance.RemoveCursorSource(source);
        }
        public void AddCursorSource(CursorSource source)
        {
            cursorSources.Add(source);
            cursorSources.Sort();
            source.OnEnableChanged += OnSourceEnableChangedCallback;
            UpdateCursor();
        }
        public bool RemoveCursorSource(CursorSource source)
        {
            if (cursorSources.Remove(source))
            {
                source.OnEnableChanged -= OnSourceEnableChangedCallback;
                UpdateCursor();
                return true;
            }
            return false;
        }
        private void OnEnable()
        {
            UpdateCursor();
        }
        private void Update()
        {
            var mousePosition = Input.mousePosition;
            var outScreen = mousePosition.x <= 0 || mousePosition.y <= 0 || mousePosition.x >= Screen.width || mousePosition.y >= Screen.height;
            var invalidSources = RemoveInvalidCursorSources();
            if (invalidSources > 0 || outScreen != outOfScreen)
            {
                outOfScreen = outScreen;
                UpdateCursor();
            }
        }
        private void OnSourceEnableChangedCallback(bool value)
        {
            UpdateCursor();
        }
        private int RemoveInvalidCursorSources()
        {
            int removed = 0;
            for (int i = cursorSources.Count - 1; i >= 0; i--)
            {
                CursorSource source = cursorSources[i];
                if (!source.IsValid())
                {
                    cursorSources.RemoveAt(i);
                    source.OnEnableChanged -= OnSourceEnableChangedCallback;
                    removed++;
                }
            }
            return removed;
        }
        private void UpdateCursor()
        {
            var main = MainManager.Instance;
            // 移动端不要调用Cursor相关内容，可能会崩溃
            if (main == null || main.IsMobile())
                return;

            var cursorType = CursorType.Arrow;
            var enabledSources = cursorSources.Where(s => s.Enabled);
            if (enabledSources.Count() > 0)
            {
                cursorType = enabledSources.LastOrDefault().CursorType;
            }

            if (targetCursorType == cursorType)
                return;
            targetCursorType = cursorType;

            if (targetCursorType == CursorType.Empty)
            {
                Cursor.visible = outOfScreen;
            }
            else
            {
                Cursor.visible = true;
                var cursorData = cursorDatas.FirstOrDefault(d => d.type == targetCursorType);
                Cursor.SetCursor(cursorData.texture, cursorData.hotspot, CursorMode.Auto);
            }
        }
        private bool outOfScreen;
        private List<CursorSource> cursorSources = new List<CursorSource>();
        [SerializeField]
        private CursorType targetCursorType;
        [SerializeField]
        private List<CursorData> cursorDatas = new List<CursorData>();
    }
    [Serializable]
    public struct CursorData
    {
        public CursorType type;
        public Texture2D texture;
        public Vector2 hotspot;
    }
    public abstract class CursorSource : IComparable<CursorSource>
    {
        public abstract bool IsValid();
        public void SetEnabled(bool enabled)
        {
            if (enabled == Enabled)
                return;
            Enabled = enabled;
            OnEnableChanged?.Invoke(enabled);
        }
        public event Action<bool> OnEnableChanged;
        public bool Enabled { get; private set; } = true;
        public abstract CursorType CursorType { get; }
        public abstract int Priority { get; }

        public int CompareTo(CursorSource other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
    public enum CursorType
    {
        Arrow,
        Point,
        Drag,
        Move,
        Empty
    }
}
