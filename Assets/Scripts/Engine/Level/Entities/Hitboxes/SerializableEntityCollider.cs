using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine.Entities
{
    public class SerializableEntityCollider
    {
        public string name;
        public bool enabled;
        public SerializableHitbox[] hitboxes;
        public EntityColliderReference[] collisionList;
    }
}
