#nullable enable

using System;
using System.Collections.Generic;

namespace MVZ2Logic.Cursor
{
    public static class GameCursor
    {
        public static void AddSource(CursorSource source)
        {
            OnAddSource?.Invoke(source);
        }
        public static void RemoveSource(CursorSource source)
        {
            OnRemoveSource?.Invoke(source);
        }
        public static event Action<CursorSource>? OnAddSource;
        public static event Action<CursorSource>? OnRemoveSource;
        private static List<CursorSource> cursorSources = new List<CursorSource>();
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
        public event Action<bool>? OnEnableChanged;
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