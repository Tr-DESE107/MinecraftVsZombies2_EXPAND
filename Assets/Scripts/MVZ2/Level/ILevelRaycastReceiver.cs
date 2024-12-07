using MVZ2Logic.HeldItems;
using PVZEngine.Level;

namespace MVZ2.Level
{
    public interface ILevelRaycastReceiver
    {
        bool IsValidReceiver(LevelEngine level, HeldItemDefinition definition, long id);
    }
}
