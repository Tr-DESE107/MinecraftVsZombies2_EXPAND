using System;
using UnityEngine;

namespace PVZEngine
{
    public partial class Game
    {
        public void PlaySound(NamespaceID id, Vector3 position)
        {
            OnPlaySound?.Invoke(id, position);
        }
        public event Action<NamespaceID, Vector3> OnPlaySound;
    }
}