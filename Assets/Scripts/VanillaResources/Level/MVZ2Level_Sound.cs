using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2Logic.Level
{
    public static partial class MVZ2Level
    {
        public static ISoundComponent GetSoundComponent(this LevelEngine level)
        {
            return level.GetComponent<ISoundComponent>();
        }
        public static void PlaySound(this LevelEngine level, NamespaceID id, Vector3 position, float pitch = 1)
        {
            var component = level.GetSoundComponent();
            component.PlaySound(id, position, pitch);
        }
        public static void PlaySound(this LevelEngine level, NamespaceID id, float pitch = 1)
        {
            var component = level.GetSoundComponent();
            component.PlaySound(id, pitch);
        }
    }
}
