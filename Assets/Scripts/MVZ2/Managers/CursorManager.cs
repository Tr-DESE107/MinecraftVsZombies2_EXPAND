using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVZ2
{
    public class CursorManager : MonoBehaviour
    {
        public static void AddSource(ICursorSource source)
        {
            var instance = MainManager.Instance.CursorManager;
            if (!instance)
                return;

            instance.cursorSources.Add(source);
            instance.UpdateCursor();
        }
        public static bool RemoveSource(ICursorSource source)
        {
            var instance = MainManager.Instance.CursorManager;
            if (!instance)
                return false;

            if (instance.cursorSources.Remove(source))
            {
                instance.UpdateCursor();
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
            var invalidSources = cursorSources.RemoveAll(s => !s.IsValid());
            if (invalidSources > 0)
            {
                UpdateCursor();
            }
        }
        private void UpdateCursor()
        {
            targetCursorType = cursorSources.Count > 0 ? cursorSources.OrderByDescending(c => c.Priority).FirstOrDefault().CursorType : CursorType.Arrow;

            if (targetCursorType == CursorType.Empty)
            {
                Cursor.visible = false;
            }
            else
            {
                Cursor.visible = true;
                var cursorData = cursorDatas.FirstOrDefault(d => d.type == targetCursorType);
                Cursor.SetCursor(cursorData.texture, cursorData.hotspot, CursorMode.Auto);
            }
        }
        private List<ICursorSource> cursorSources = new List<ICursorSource>();
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
    public interface ICursorSource
    {
        CursorType CursorType { get; }
        int Priority { get; }
        bool IsValid();
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
