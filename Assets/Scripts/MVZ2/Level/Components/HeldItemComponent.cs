using MVZ2.GameContent;
using MVZ2.Level;
using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    public partial class HeldItemComponent : MVZ2Component
    {
        public HeldItemComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public bool IsHoldingItem()
        {
            return NamespaceID.IsValid(heldItemType) && heldItemType != HeldTypes.none;
        }
        public void SetHeldItem(NamespaceID type, int id, int priority, bool noCancel = false)
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
        public bool IsValidOnEntity(Entity entity, NamespaceID type, int heldItemID)
        {
            var heldItemDef = Controller.Game.GetDefinition<HeldItemDefinition>(type);
            return heldItemDef.IsValidOnEntity(entity, heldItemID);
        }
        public bool IsValidOnGrid(LawnGrid grid, NamespaceID type, int heldItemID)
        {
            var heldItemDef = Controller.Game.GetDefinition<HeldItemDefinition>(type);
            return heldItemDef.IsValidOnGrid(grid, heldItemID);
        }
        public bool UseOnEntity(Entity entity)
        {
            var heldItemDef = Controller.Game.GetDefinition<HeldItemDefinition>(HeldItemType);
            return heldItemDef.UseOnEntity(entity, heldItemID);
        }
        public bool UseOnGrid(LawnGrid grid)
        {
            var heldItemDef = Controller.Game.GetDefinition<HeldItemDefinition>(HeldItemType);
            return heldItemDef.UseOnGrid(grid, heldItemID);
        }
        public void UseOnLawn(LevelEngine level, LawnArea area)
        {
            var heldItemDef = Controller.Game.GetDefinition<HeldItemDefinition>(HeldItemType);
            heldItemDef.UseOnLawn(level, area, heldItemID);
        }
        public void HoverOnEntity(Entity entity)
        {
            var heldItemDef = Controller.Game.GetDefinition<HeldItemDefinition>(HeldItemType);
            heldItemDef.HoverOnEntity(entity, heldItemID);
        }
        public NamespaceID HeldItemType => heldItemType;
        public int HeldItemID => heldItemID;
        public int HeldItemPriority => heldItemPriority;
        public bool HeldItemNoCancel => heldItemNoCancel;
        private NamespaceID heldItemType;
        private int heldItemID;
        private int heldItemPriority;
        private bool heldItemNoCancel;
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "heldItem");
    }
    public class EmptySerializableLevelComponent : ISerializableLevelComponent
    {
    }
}