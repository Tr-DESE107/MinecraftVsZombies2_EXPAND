using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public interface ICollectiblePickup
    {
        bool? PreCollect(Entity pickup);
        void PostCollect(Entity pickup);
    }
}
