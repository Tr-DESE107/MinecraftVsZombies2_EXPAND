using System;
using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.HeldItems;
using MVZ2Logic.Games;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2.Level.Components
{
    public partial class HeldItemComponent : MVZ2Component, IHeldItemComponent
    {
        public HeldItemComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public override void Update()
        {
            base.Update();
            var definition = Level.Content.GetHeldItemDefinition(Data.Type);
            if (definition != null)
            {
                definition.Update(Level, Data);
            }
        }
        public bool IsHoldingItem()
        {
            return NamespaceID.IsValid(Data.Type) && Data.Type != BuiltinHeldTypes.none;
        }
        public void SetHeldItem(NamespaceID type, long id, int priority, bool noCancel = false)
        {
            SetHeldItem(new HeldItemData()
            {
                Type = type,
                ID = id,
                Priority = priority,
                NoCancel = noCancel
            });
        }
        public void SetHeldItem(IHeldItemData value)
        {
            if (IsHoldingItem() && info.Priority > value.Priority)
                return;

            var before = Level.Content.GetHeldItemDefinition(Data.Type);
            before?.End(Level, Data);

            info.Type = value.Type;
            info.ID = value.ID;
            info.Priority = value.Priority;
            info.NoCancel = value.NoCancel;
            info.InstantTrigger = value.InstantTrigger;
            info.InstantEvoke = value.InstantEvoke;
            Controller.SetHeldItemUI(info);

            var definition = Level.Content.GetHeldItemDefinition(Data.Type);
            definition?.Begin(Level, Data);
        }
        public IModelInterface GetHeldItemModelInterface()
        {
            return Controller.GetHeldItemModelInterface();
        }
        public void ResetHeldItem()
        {
            var before = Level.Content.GetHeldItemDefinition(Data.Type);
            before?.End(Level, Data);

            info.Type = BuiltinHeldTypes.none;
            info.ID = 0;
            info.Priority = 0;
            info.NoCancel = false;
            info.InstantTrigger = false;
            info.InstantEvoke = false;
            Controller.SetHeldItemUI(info);
        }
        public bool CancelHeldItem()
        {
            if (!IsHoldingItem() || info.NoCancel)
                return false;
            ResetHeldItem();
            return true;
        }
        public IHeldItemData Data => info;
        private HeldItemData info = new HeldItemData()
        {
            Type = BuiltinHeldTypes.none
        };
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "heldItem");
    }
    [Serializable]
    public class EmptySerializableLevelComponent : ISerializableLevelComponent
    {
    }
}