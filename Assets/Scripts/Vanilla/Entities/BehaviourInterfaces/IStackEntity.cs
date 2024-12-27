using PVZEngine.Entities;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.Entities
{
    public interface IStackEntity
    {
        void CanStackOnEntity(Entity target, TriggerResultBoolean result);
        void StackOnEntity(Entity target);
    }
}
