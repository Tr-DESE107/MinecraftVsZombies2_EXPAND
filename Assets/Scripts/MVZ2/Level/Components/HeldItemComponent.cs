using System;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public partial class HeldItemComponent : MVZ2Component, IHeldItemComponent
    {
        public HeldItemComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public bool IsHoldingItem()
        {
            return NamespaceID.IsValid(heldItemType) && heldItemType != HeldTypes.none;
        }
        public void SetHeldItem(NamespaceID type, long id, int priority, bool noCancel = false)
        {
            if (IsHoldingItem() && heldItemPriority > priority)
                return;
            heldItemType = type;
            heldItemID = id;
            heldItemPriority = priority;
            heldItemNoCancel = noCancel;
            Controller.SetHeldItemUI(type, id, priority, noCancel);
        }
        public void ResetHeldItem()
        {
            SetHeldItem(HeldTypes.none, 0, 0, false);
        }
        public bool CancelHeldItem()
        {
            if (!IsHoldingItem() || HeldItemNoCancel)
                return false;
            ResetHeldItem();
            return true;
        }
        public NamespaceID HeldItemType => heldItemType;
        public long HeldItemID => heldItemID;
        public int HeldItemPriority => heldItemPriority;
        public bool HeldItemNoCancel => heldItemNoCancel;
        private NamespaceID heldItemType;
        private long heldItemID;
        private int heldItemPriority;
        private bool heldItemNoCancel;
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "heldItem");
    }
    [Serializable]
    public class EmptySerializableLevelComponent : ISerializableLevelComponent
    {
    }
}