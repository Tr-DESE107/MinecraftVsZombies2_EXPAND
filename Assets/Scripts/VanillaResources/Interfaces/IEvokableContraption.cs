using PVZEngine.Entities;

namespace MVZ2.GameContent
{
    public interface IEvokableContraption
    {
        bool CanEvoke(Entity contraption);
        void Evoke(Entity contraption);
    }
}
