using PVZEngine.Entities;
using PVZEngine.Level;

namespace PVZEngine.Auras
{
    public interface IAuraSource
    {
        Entity GetEntity();
        LevelEngine GetLevel();
    }
}
