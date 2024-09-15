using System;
using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2
{
    public static partial class MVZ2Level
    {
        public static SoundComponent GetSoundComponent(this LevelEngine level)
        {
            return level.GetComponent<SoundComponent>();
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
