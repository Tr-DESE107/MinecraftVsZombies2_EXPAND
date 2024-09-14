using System;
using log4net.Core;

namespace PVZEngine.LevelManagement
{
    public partial class Level
    {
        public void SetHeldItem(int type, int id, int priority, bool noCancel = false)
        {
            if (heldItemType > 0 && heldItemPriority > priority)
                return;
            heldItemType = type;
            heldItemID = id;
            heldItemPriority = priority;
            heldItemNoCancel = noCancel;
            OnHeldItemChanged?.Invoke(type, id, priority, noCancel);
        }
        public void ResetHeldItem()
        {
            SetHeldItem(0, 0, 0, false);
        }
        public event Action<int, int, int, bool> OnHeldItemChanged;

        public int HeldItemType => heldItemType;
        public int HeldItemID => heldItemID;
        public int HeldItemPriority => heldItemPriority;
        public bool HeldItemNoCancel => heldItemNoCancel;
        private int heldItemType;
        private int heldItemID;
        private int heldItemPriority;
        private bool heldItemNoCancel;
    }
}