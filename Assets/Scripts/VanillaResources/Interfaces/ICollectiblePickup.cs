using PVZEngine.Entities;

namespace MVZ2.GameContent
{
    public interface ICollectiblePickup
    {
        bool? PreCollect(Entity pickup);
        void PostCollect(Entity pickup);
    }
}
