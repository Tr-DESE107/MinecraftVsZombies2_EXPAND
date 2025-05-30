using MVZ2.HeldItems;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2.GameContent.HeldItems
{
    public interface IHeldEntityBehaviour
    {
        bool IsHeldItemValidFor(Entity entity, IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer);
        HeldHighlight GetHeldItemHighlight(Entity entity, IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer);
    }
    public interface IHeldEntityGetModelIDHandler : IHeldEntityBehaviour
    {
        void GetHeldItemModelID(Entity entity, LevelEngine level, IHeldItemData data, CallbackResult result);
    }
    public interface IHeldEntityGetRadiusHandler : IHeldEntityBehaviour
    {
        void GetHeldItemRadius(Entity entity, LevelEngine level, IHeldItemData data, CallbackResult result);
    }
    public interface IHeldEntityBeginHandler : IHeldEntityBehaviour
    {
        void OnHeldItemBegin(Entity source, LevelEngine level, IHeldItemData data);
    }
    public interface IHeldEntityEndHandler : IHeldEntityBehaviour
    {
        void OnHeldItemEnd(Entity source, LevelEngine level, IHeldItemData data);
    }
    public interface IHeldEntityUpdateHandler : IHeldEntityBehaviour
    {
        void OnHeldItemUpdate(Entity source, LevelEngine level, IHeldItemData data);
    }
    public interface IHeldEntitySetModelHandler : IHeldEntityBehaviour
    {
        void OnHeldItemSetModel(Entity entity, LevelEngine level, IHeldItemData data, IModelInterface model);
    }
    public interface IHeldEntityPointerEventHandler : IHeldEntityBehaviour
    {
        void OnHeldItemPointerEvent(Entity source, IHeldItemTarget target, IHeldItemData data, PointerInteractionData interactionData);
    }
}
