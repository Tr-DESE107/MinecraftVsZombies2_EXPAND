using PVZEngine.Entities;

namespace MVZ2Logic.Entities
{
    public interface IEvokableContraption
    {
        bool CanEvoke(Entity contraption);
        void Evoke(Entity contraption);
    }
}
