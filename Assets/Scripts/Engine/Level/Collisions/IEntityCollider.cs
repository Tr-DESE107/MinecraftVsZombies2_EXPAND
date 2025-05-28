﻿using UnityEngine;

namespace PVZEngine.Entities
{
    public interface IEntityCollider
    {
        string Name { get; }
        Entity Entity { get; }
        bool Enabled { get; }
        NamespaceID ArmorSlot { get; }
        EntityColliderReference ToReference();
        bool CheckSphere(Vector3 center, float radius);
        Bounds GetBoundingBox();
        void SetEnabled(bool enabled);
    }
}
