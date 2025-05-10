using PVZEngine.Entities;
using PVZEngine.Callbacks;

namespace MVZ2.Vanilla.Entities
{
    public interface IStackEntity
    {
        void CanStackOnEntity(Entity target, CallbackResult result);
        void StackOnEntity(Entity target);
    }
}
