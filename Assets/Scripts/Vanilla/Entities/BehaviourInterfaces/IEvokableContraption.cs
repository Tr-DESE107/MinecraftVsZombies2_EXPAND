using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public interface IEvokableContraption
    {
        bool CanEvoke(Entity contraption);
        void Evoke(Entity contraption);
    }
}
