using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public interface ITriggerableContraption
    {
        bool CanTrigger(Entity contraption);
        void Trigger(Entity contraption);
    }
}
