using MVZ2.HeldItems;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2Logic.HeldItems
{
    public interface IEntityHeldItemBehaviour
    {
        Entity GetEntity(LevelEngine level, IHeldItemData id);
    }
}
