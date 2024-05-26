using System.Collections.Generic;
using UnityEngine;

namespace PVZEngine.Serialization
{
    public class SerializableProjectile : SerializableEntity
    {
        public List<int> collided = new List<int>();
        public bool canHitSpawner;
    }
}
