﻿using System.Collections.Generic;
using PVZEngine.Entities;
using UnityEngine;

namespace PVZEngine.Level.Collisions
{
    public interface ICollisionSystem
    {
        void Update();
        void InitEntity(Entity entity);
        void UpdateEntityDetection(Entity entity);
        void UpdateEntityPosition(Entity entity);
        void UpdateEntitySize(Entity entity);
        void DestroyEntity(Entity entity);
        IEntityCollider CreateCustomCollider(Entity entity, ColliderConstructor info);
        bool RemoveCollider(Entity entity, string name);
        IEntityCollider GetCollider(Entity entity, string name);
        void GetCurrentCollisions(Entity entity, List<EntityCollision> collisions);


        ISerializableCollisionSystem ToSerializable();
        void LoadFromSerializable(LevelEngine level, ISerializableCollisionSystem seri);


        IEntityCollider[] OverlapBox(Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask);
        void OverlapBoxNonAlloc(Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results);
        IEntityCollider[] OverlapSphere(Vector3 center, float radius, int faction, int hostileMask, int friendlyMask);
        void OverlapSphereNonAlloc(Vector3 center, float radius, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results);
        IEntityCollider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int faction, int hostileMask, int friendlyMask);
        void OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results);
    }
    public interface ISerializableCollisionSystem
    {
        ISerializableCollisionEntity[] Entities { get; }
        ISerializableCollisionEntity[] EntityTrash { get; }
    }
    public interface ISerializableCollisionEntity
    {
        long ID { get; }
        ISerializableCollisionCollider[] Colliders { get; }
    }
    public interface ISerializableCollisionCollider
    {
        string Name { get; }
        bool Enabled { get; }
        NamespaceID ArmorSlot { get; }
        SerializableEntityCollision[] Collisions { get; }
        int UpdateMode { get; }
        Vector3 CustomSize { get; }
        Vector3 CustomOffset { get; }
        Vector3 CustomPivot { get; }
    }
}
