﻿using PVZEngine.Level.Collisions;
using UnityEngine;

namespace PVZEngine.Entities
{
    public class SerializableEntityCollider : ISerializableCollisionCollider
    {
        public string name;
        public bool enabled;
        public NamespaceID armorSlot;
        public SerializableEntityCollision[] collisionList;
        public int updateMode;
        public Vector3 customSize;
        public Vector3 customOffset;
        public Vector3 customPivot;
        string ISerializableCollisionCollider.Name => name;
        bool ISerializableCollisionCollider.Enabled => enabled;
        NamespaceID ISerializableCollisionCollider.ArmorSlot => armorSlot;
        SerializableEntityCollision[] ISerializableCollisionCollider.Collisions => collisionList;
        int ISerializableCollisionCollider.UpdateMode => updateMode;
        Vector3 ISerializableCollisionCollider.CustomSize => customSize;
        Vector3 ISerializableCollisionCollider.CustomOffset => customOffset;
        Vector3 ISerializableCollisionCollider.CustomPivot => customPivot;
    }
}
