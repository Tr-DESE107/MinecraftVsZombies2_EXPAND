using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Extensions
{
    public static class MVZ2Entity
    {
        public static void PlaySound(this Entity entity, NamespaceID soundID, float pitch = 1)
        {
            entity.Level.PlaySound(soundID, entity.Pos, pitch);
        }
    }
}
