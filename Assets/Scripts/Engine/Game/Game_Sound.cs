using System;
using UnityEngine;

namespace PVZEngine
{
    public partial class Game
    {
        public void PlaySound(NamespaceID id, Vector3 position, float pitch = 1)
        {
            OnPlaySoundPosition?.Invoke(id, position, pitch);
        }
        public void PlaySound(NamespaceID id, float pitch = 1)
        {
            OnPlaySound?.Invoke(id, pitch);
        }
        public event Action<NamespaceID, Vector3, float> OnPlaySoundPosition;
        public event Action<NamespaceID, float> OnPlaySound;
    }
}