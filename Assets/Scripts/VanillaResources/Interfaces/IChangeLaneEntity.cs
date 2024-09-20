using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public interface IChangeLaneEntity
    {
        bool IsChangingLane(Entity entity);
        void SetChangingLane(Entity entity, bool value);
        int GetChangeLaneTarget(Entity entity);
        int GetChangeLaneSource(Entity entity);
        void SetChangeLaneTarget(Entity entity, int value);
        void SetChangeLaneSource(Entity entity, int value);
        float GetChangeLaneSpeed(Entity entity);
        void PostStartChangingLane(Entity entity, int target);
        void PostStopChangingLane(Entity entity);
    }
}
