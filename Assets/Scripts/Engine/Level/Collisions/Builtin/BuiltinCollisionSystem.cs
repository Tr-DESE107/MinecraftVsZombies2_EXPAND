﻿using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Entities;
using Tools.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace PVZEngine.Level.Collisions
{
    public class BuiltinCollisionSystem : ICollisionSystem
    {
        public BuiltinCollisionSystem(QuadTreeParams treeParams)
        {
            quadTreeParams = treeParams;
            entityPool = new ObjectPool<BuiltinCollisionEntity>(CreateEntityFunc);
        }
        public void Update()
        {
            UpdateTrash();
            Simulate();
        }
        private void Simulate()
        {
            colliderBuffer.Clear();
            foreach (var quadTree in quadTrees.Values)
            {
                quadTree.Update();
                quadTree.GetAllTargets(colliderBuffer);
            }

            foreach (var collider1 in colliderBuffer)
            {
                var ent1 = collider1.Entity;
                int maskHostile = ent1.CollisionMaskHostile;
                int maskFriendly = ent1.CollisionMaskFriendly;
                var maskTotal = maskHostile | maskFriendly;
                int ent1Faction = ent1.Cache.Faction;


                var rect1 = collider1.GetCollisionRect();

                collisionBuffer.Clear();
                foreach (var pair in quadTrees)
                {
                    var flag = pair.Key;
                    if ((flag & maskTotal) == 0)
                        continue;
                    var tree = pair.Value;
                    tree.FindTargetsInRect(rect1, collisionBuffer, 0);
                }
                var prevPosition = collider1.GetPosition() - (ent1.Position - ent1.PreviousPosition);
                var axis = ent1.Velocity.normalized;
                bool isAxisValid = axis.sqrMagnitude > 0.0001f;

                int SortCollider(BuiltinCollisionCollider c1, BuiltinCollisionCollider c2)
                {
                    var pos1 = Vector3Int.FloorToInt(c1.GetPosition());
                    var pos2 = Vector3Int.FloorToInt(c2.GetPosition());
                    if (isAxisValid)
                    {
                        var dot1 = Vector3.Dot(pos1, axis);
                        var dot2 = Vector3.Dot(pos2, axis);
                        var dotResult = dot1.CompareTo(dot2);
                        if (dotResult != 0)
                            return dotResult;
                    }
                    else
                    {
                        var distance1 = (pos1 - prevPosition).sqrMagnitude;
                        var distance2 = (pos2 - prevPosition).sqrMagnitude;
                        var distanceResult = distance1.CompareTo(distance2);
                        if (distanceResult != 0)
                            return distanceResult;
                    }
                    var id1 = c1.Entity.ID;
                    var id2 = c2.Entity.ID;
                    return id1.CompareTo(id2);
                }
                collisionBuffer.Sort(SortCollider);

                foreach (var collider2 in collisionBuffer)
                {
                    if (collider1 == collider2)
                        continue;
                    var ent2 = collider2.Entity;
                    if (ent1 == ent2)
                        continue;
                    if (!EntityCollisionHelper.CanCollideFaction(maskHostile, maskFriendly, ent1Faction, ent2))
                        continue;

                    collider1.DoCollision(collider2, Vector3.zero);
                }

                collider1.ExitCollision();
            }
        }
        private void UpdateTrash()
        {
            foreach (var trash in entityTrash)
            {
                entityPool.Release(trash.Value);
            }
            entityTrash.Clear();
        }


        #region 实体
        public void InitEntity(Entity entity)
        {
            var collisionEntity = CreateCollisionEntity();
            collisionEntity.Init(entity);
            entities.Add(entity.ID, collisionEntity);
        }
        public void UpdateEntityDetection(Entity entity)
        {
            var collisionEntity = GetCollisionEntity(entity);
            collisionEntity?.UpdateEntityDetection();
        }
        public void UpdateEntityPosition(Entity entity)
        {
            var collisionEntity = GetCollisionEntity(entity);
            collisionEntity?.UpdateEntityPosition();
        }
        public void UpdateEntitySize(Entity entity)
        {
            var collisionEntity = GetCollisionEntity(entity);
            collisionEntity?.UpdateEntitySize();
        }
        public void DestroyEntity(Entity entity)
        {
            var collisionEntity = GetCollisionEntity(entity);
            if (collisionEntity == null)
                return;
            RemoveCollisionEntity(collisionEntity);
        }
        public void GetCurrentCollisions(Entity entity, List<EntityCollision> collisions)
        {
            var collisionEntity = GetCollisionEntity(entity);
            if (collisionEntity == null)
                return;
            collisionEntity.GetCurrentCollisions(collisions);
        }

        private BuiltinCollisionEntity CreateEntityFunc()
        {
            return new BuiltinCollisionEntity();
        }
        public BuiltinCollisionEntity CreateCollisionEntity()
        {
            var collisionEnt = entityPool.Get();
            collisionEnt.OnEntityColliderEnabled += OnEntityColliderEnabledCallback;
            collisionEnt.OnEntityColliderDisabled += OnEntityColliderDisabledCallback;
            collisionEnt.OnEntityColliderAdd += OnEntityColliderAddCallback;
            collisionEnt.OnEntityColliderRemove += OnEntityColliderRemoveCallback;
            return collisionEnt;
        }
        public BuiltinCollisionEntity GetCollisionEntity(Entity entity)
        {
            return entities.TryGetValue(entity.ID, out var collisionEntity) ? collisionEntity : null;
        }
        public bool RemoveCollisionEntity(BuiltinCollisionEntity entity)
        {
            var id = entity.ID;
            if (entities.Remove(id))
            {
                entity.ClearColliders();
                entity.OnEntityColliderEnabled -= OnEntityColliderEnabledCallback;
                entity.OnEntityColliderDisabled -= OnEntityColliderDisabledCallback;
                entity.OnEntityColliderAdd -= OnEntityColliderAddCallback;
                entity.OnEntityColliderRemove -= OnEntityColliderRemoveCallback;
                entityTrash.Add(id, entity);
                return true;
            }
            return false;
        }
        #endregion


        #region 碰撞体
        public IEntityCollider CreateCustomCollider(Entity entity, ColliderConstructor cons)
        {
            var collisionEntity = GetCollisionEntity(entity);
            if (collisionEntity == null)
                return null;
            return collisionEntity.CreateCustomCollider(cons);
        }
        public void AddCollider(Entity entity, IEntityCollider collider)
        {
            if (collider is not BuiltinCollisionCollider entCol)
                return;
            var collisionEntity = GetCollisionEntity(entity);
            if (collisionEntity == null)
                return;
            collisionEntity.AddCollider(entCol);
        }
        public bool RemoveCollider(Entity entity, string name)
        {
            var collisionEntity = GetCollisionEntity(entity);
            if (collisionEntity == null)
                return false;
            return collisionEntity.RemoveCollider(name);
        }
        public BuiltinCollisionCollider GetCollider(Entity entity, string name)
        {
            var collisionEntity = GetCollisionEntity(entity);
            if (collisionEntity == null)
                return null;
            return collisionEntity.GetCollider(name);
        }
        IEntityCollider ICollisionSystem.GetCollider(Entity entity, string name) => GetCollider(entity, name);

        #endregion

        #region 检测
        public IEntityCollider[] OverlapBox(Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask)
        {
            var min = center - size * 0.5f;
            var filterRect = new Rect(min.x, min.z, size.x, size.y);
            var bounds = new Bounds(center, size);
            return Overlap(filterRect, faction, hostileMask, friendlyMask, h => bounds.IntersectsOptimized(h.GetBounds()));
        }
        public void OverlapBoxNonAlloc(Vector3 center, Vector3 size, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results)
        {
            var min = center - size * 0.5f;
            var filterRect = new Rect(min.x, min.z, size.x, size.z);
            var bounds = new Bounds(center, size);
            OverlapNonAlloc(filterRect, faction, hostileMask, friendlyMask, h => bounds.IntersectsOptimized(h.GetBounds()), results);
        }
        public IEntityCollider[] OverlapSphere(Vector3 center, float radius, int faction, int hostileMask, int friendlyMask)
        {
            var min = center - Vector3.one * radius;
            var filterRect = new Rect(min.x, min.z, radius * 2, radius * 2);
            return Overlap(filterRect, faction, hostileMask, friendlyMask, h => MathTool.CollideBetweenCubeAndSphere(h.GetBounds(), center, radius));
        }
        public void OverlapSphereNonAlloc(Vector3 center, float radius, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results)
        {
            var min = center - Vector3.one * radius;
            var filterRect = new Rect(min.x, min.z, radius * 2, radius * 2);
            OverlapNonAlloc(filterRect, faction, hostileMask, friendlyMask, h => MathTool.CollideBetweenCubeAndSphere(h.GetBounds(), center, radius), results);
        }
        public IEntityCollider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int faction, int hostileMask, int friendlyMask)
        {
            var center = (point1 + point0) * 0.5f;
            var min = center - Vector3.one * radius;
            var filterRect = new Rect(min.x, min.z, radius * 2, radius * 2);
            var capsule = new Capsule(point0, point1, radius);
            return Overlap(filterRect, faction, hostileMask, friendlyMask, h => MathTool.CollideBetweenCubeAndCapsule(capsule, h.GetBoundsCenter(), h.GetBoundsSize()));
        }
        public void OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, int faction, int hostileMask, int friendlyMask, List<IEntityCollider> results)
        {
            var center = (point1 + point0) * 0.5f;
            var min = center - Vector3.one * radius;
            var filterRect = new Rect(min.x, min.z, radius * 2, radius * 2);
            var capsule = new Capsule(point0, point1, radius);
            OverlapNonAlloc(filterRect, faction, hostileMask, friendlyMask, h => MathTool.CollideBetweenCubeAndCapsule(capsule, h.GetBoundsCenter(), h.GetBoundsSize()), results);
        }
        public IEntityCollider[] Overlap(Rect filterRect, int faction, int hostileMask, int friendlyMask, Predicate<Hitbox> predicate)
        {
            if (predicate == null)
                return Array.Empty<IEntityCollider>();
            var entityColliders = new List<BuiltinCollisionCollider>();
            var totalMask = hostileMask | friendlyMask;
            FindCollidersRange(totalMask, filterRect, entityColliders);
            for (int c = entityColliders.Count - 1; c >= 0; c--)
            {
                var collider = entityColliders[c];
                var entity = collider.Entity;
                if (EntityCollisionHelper.CanCollideFaction(hostileMask, friendlyMask, faction, entity))
                {
                    var hitbox = collider.GetHitbox();
                    if (predicate(hitbox))
                    {
                        continue;
                    }
                }
                entityColliders.RemoveAt(c);
            }
            return entityColliders.ToArray();
        }
        private void OverlapNonAlloc(Rect filterRect, int faction, int hostileMask, int friendlyMask, Predicate<Hitbox> predicate, List<IEntityCollider> results)
        {
            if (predicate == null)
                return;
            var entityColliders = new List<BuiltinCollisionCollider>();
            var totalMask = hostileMask | friendlyMask;
            FindCollidersRange(totalMask, filterRect, entityColliders);
            foreach (var collider in entityColliders)
            {
                if (results.Contains(collider))
                    continue;
                var entity = collider.Entity;
                if (EntityCollisionHelper.CanCollideFaction(hostileMask, friendlyMask, faction, entity))
                {
                    var hitbox = collider.GetHitbox();
                    if (predicate(hitbox))
                    {
                        results.Add(collider);
                    }
                }
            }
        }
        #endregion

        #region 四叉树
        public QuadTreeCollider GetCollisionQuadTree(int flag)
        {
            if (quadTrees.TryGetValue(flag, out var tree))
                return tree;
            return null;
        }
        private void FindCollidersRange(int mask, Rect rect, List<BuiltinCollisionCollider> collider)
        {
            foreach (var pair in quadTrees)
            {
                var flag = pair.Key;
                if ((flag & mask) == 0)
                    continue;
                var quadTree = pair.Value;
                quadTree.FindTargetsInRect(rect, collider);
            }
        }
        private void InsertColliderToTree(int flag, BuiltinCollisionCollider collider)
        {
            if (!quadTrees.TryGetValue(flag, out var tree))
            {
                tree = CreateQuadTree();
                quadTrees.Add(flag, tree);
            }
            tree.Insert(collider);
        }
        private void RemoveColliderFromTree(int flag, BuiltinCollisionCollider collider)
        {
            if (!quadTrees.TryGetValue(flag, out var tree))
            {
                return;
            }
            tree.Remove(collider);
        }
        private QuadTreeCollider CreateQuadTree()
        {
            return new QuadTreeCollider(quadTreeParams.size, quadTreeParams.maxObjects, quadTreeParams.maxDepth);
        }
        #endregion

        #region 回调
        private void OnEntityColliderEnabledCallback(BuiltinCollisionEntity entity, BuiltinCollisionCollider collider)
        {
            InsertColliderToTree(entity.Entity.TypeCollisionFlag, collider);
        }
        private void OnEntityColliderDisabledCallback(BuiltinCollisionEntity entity, BuiltinCollisionCollider collider)
        {
            RemoveColliderFromTree(collider.Entity.TypeCollisionFlag, collider);
        }
        private void OnEntityColliderAddCallback(BuiltinCollisionEntity entity, BuiltinCollisionCollider collider)
        {
            if (collider.Enabled)
            {
                InsertColliderToTree(entity.Entity.TypeCollisionFlag, collider);
            }
        }
        private void OnEntityColliderRemoveCallback(BuiltinCollisionEntity entity, BuiltinCollisionCollider collider)
        {
            if (collider.Enabled)
            {
                RemoveColliderFromTree(entity.Entity.TypeCollisionFlag, collider);
            }
        }
        #endregion

        public SerializableBuiltinCollisionSystem ToSerializable()
        {
            var seri = new SerializableBuiltinCollisionSystem();
            seri.entities = entities.Select(p => p.Value.ToSerializable()).ToArray();
            seri.entityTrash = entityTrash.Select(p => p.Value.ToSerializable()).ToArray();
            return seri;
        }
        public void LoadFromSerializable(LevelEngine level, ISerializableCollisionSystem seri)
        {
            if (seri.Entities != null)
            {
                foreach (var seriEnt in seri.Entities)
                {
                    var entity = CreateCollisionEntity();
                    entity.LoadFromSerializable(level, seriEnt);
                    entities.Add(seriEnt.ID, entity);
                }
            }
            if (seri.EntityTrash != null)
            {
                foreach (var seriEnt in seri.EntityTrash)
                {
                    var entity = CreateCollisionEntity();
                    entity.LoadFromSerializable(level, seriEnt);
                    entityTrash.Add(seriEnt.ID, entity);
                }
            }
            if (seri.Entities != null)
            {
                foreach (var seriEnt in seri.Entities)
                {
                    var entity = entities[seriEnt.ID];
                    entity.LoadCollisions(level, seriEnt);
                }
            }
            if (seri.EntityTrash != null)
            {
                foreach (var seriEnt in seri.EntityTrash)
                {
                    var entity = entityTrash[seriEnt.ID];
                    entity.LoadCollisions(level, seriEnt);
                }
            }
        }
        ISerializableCollisionSystem ICollisionSystem.ToSerializable()
        {
            return ToSerializable();
        }


        private List<BuiltinCollisionCollider> colliderBuffer = new List<BuiltinCollisionCollider>();
        private List<BuiltinCollisionCollider> collisionBuffer = new List<BuiltinCollisionCollider>();
        private ObjectPool<BuiltinCollisionEntity> entityPool;
        private Dictionary<int, QuadTreeCollider> quadTrees = new Dictionary<int, QuadTreeCollider>();
        private QuadTreeParams quadTreeParams;

        private SortedDictionary<long, BuiltinCollisionEntity> entities = new SortedDictionary<long, BuiltinCollisionEntity>();
        private SortedDictionary<long, BuiltinCollisionEntity> entityTrash = new SortedDictionary<long, BuiltinCollisionEntity>();
    }
    public class SerializableBuiltinCollisionSystem : ISerializableCollisionSystem
    {
        public SerializableBuiltinCollisionSystemEntity[] entities;
        public SerializableBuiltinCollisionSystemEntity[] entityTrash;
        ISerializableCollisionEntity[] ISerializableCollisionSystem.Entities => entities;
        ISerializableCollisionEntity[] ISerializableCollisionSystem.EntityTrash => entityTrash;
    }
}
