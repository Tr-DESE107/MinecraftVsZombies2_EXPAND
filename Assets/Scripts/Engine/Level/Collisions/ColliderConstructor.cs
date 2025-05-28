﻿using UnityEngine;

namespace PVZEngine.Level.Collisions
{
    public struct ColliderConstructor
    {
        public ColliderConstructor(string name, NamespaceID armorSlot, Vector3 size, Vector3 offset, Vector3 pivot)
        {
            this.name = name;
            this.armorSlot = armorSlot;
            this.size = size;
            this.offset = offset;
            this.pivot = pivot;
        }
        public string name;
        public NamespaceID armorSlot;
        public Vector3 size;
        public Vector3 offset;
        public Vector3 pivot;
    }
}
