using PVZEngine.Entities;

namespace MVZ2Logic.Entities
{
    public interface ICollectiblePickup
    {
        bool? PreCollect(Entity pickup);
        void PostCollect(Entity pickup);
    }
}
