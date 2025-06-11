﻿using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Level.Collisions;
using Tools;
using UnityEngine;

namespace MVZ2.Collisions
{
    public class UnityEntityCollider : MonoBehaviour, IEntityCollider
    {
        public void ResetCollider()
        {
            SetEnabled(true);
            SetMain();
            touchingColliders.Clear();
            collisionList.Clear();
            collisionBuffer.Clear();
        }
        public void Init(Entity entity, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"The name of an EntityCollider cannot be null or empty.");
            Entity = entity;
            Name = name;
            gameObject.name = Name;

        }
        public void SetMain()
        {
            updateMode = ColliderUpdateMode.Main;
            ArmorSlot = null;
            customSize = Vector3.zero;
            customOffset = Vector3.zero;
            customPivot = Vector3.one * 0.5f;
        }
        public void SetCustom(ColliderConstructor constructor)
        {
            updateMode = ColliderUpdateMode.Custom;
            ArmorSlot = constructor.armorSlot;
            customSize = constructor.size;
            customOffset = constructor.offset;
            customPivot = constructor.pivot;
        }
        public void SetEnabled(bool enabled)
        {
            if (Enabled == enabled)
                return;
            Enabled = enabled;
            boxCollider.enabled = enabled;
        }
        public void UpdateEntitySize()
        {
            Vector3 boundsSize;
            Vector3 boundsPivot;
            Vector3 boundsOffset;
            switch (updateMode)
            {
                case ColliderUpdateMode.Main:
                    {
                        boundsSize = Entity.GetSize();
                        boundsPivot = Entity.GetBoundsPivot();
                        boundsOffset = Vector3.zero;
                    }
                    break;
                case ColliderUpdateMode.Custom:
                    {
                        boundsSize = customSize;
                        boundsPivot = customPivot;
                        boundsOffset = customOffset;
                    }
                    break;
                default:
                    return;
            }

            var scale = Entity.GetFinalScale();
            var center = Vector3.Scale(Vector3.one * 0.5f - boundsPivot, boundsSize) + boundsOffset;
            center = Vector3.Scale(center, scale);
            boundsSize = Vector3.Scale(boundsSize, scale);
            boundsSize = boundsSize.Abs();
            boxCollider.center = center;
            boxCollider.size = boundsSize;
        }
        public void Simulate()
        {
            foreach (var colliderCache in touchingColliders)
            {
                var collider = colliderCache.collider;
                if (!collider)
                    continue;
                var otherCollider = collider.GetComponent<UnityEntityCollider>();
                if (!otherCollider)
                    continue;

                var other = otherCollider.Entity;
                if (other == null)
                    continue;

                var collision = collisionList.Find(c => (c.OtherCollider as UnityEntityCollider) == otherCollider);
                if (collision == null)
                {
                    collision = new EntityCollision(this, otherCollider);
                    collision.Enter = true;
                    collisionList.Add(collision);
                }
                else
                {
                    collision.Enter = false;
                }
                collision.Checked = true;
            }
            collisionBuffer.Clear();
            collisionBuffer.CopyFrom(collisionList);
            for (int i = 0; i < collisionBuffer.Count; i++)
            {
                var collision = collisionBuffer[i];
                if (collision.Checked)
                {
                    if (CallPreCollision(collision))
                    {
                        if (collision.Enter)
                        {
                            CallPostCollision(collision, EntityCollisionHelper.STATE_ENTER);
                        }
                        else
                        {
                            CallPostCollision(collision, EntityCollisionHelper.STATE_STAY);
                        }
                    }
                }
                else
                {
                    collisionList.Remove(collision);
                    CallPostCollision(collision, EntityCollisionHelper.STATE_EXIT);
                }
                collision.Checked = false;
            }
            touchingColliders.Clear();
        }
        public Bounds GetBoundingBox()
        {
            return boxCollider.bounds;
        }
        private void OnTriggerStay(Collider other)
        {
            foreach (var c in touchingColliders)
            {
                if (c.collider == other)
                    return;
            }
            var otherCollider = other.GetComponent<UnityEntityCollider>();
            if (!otherCollider)
                return;
            var targetEnt = otherCollider.Entity;
            var cache = new ColliderCache(other, Entity, targetEnt.ID);
            var mask = Entity.IsHostile(targetEnt) ? Entity.CollisionMaskHostile : Entity.CollisionMaskFriendly;
            if (!EntityCollisionHelper.CanCollide(mask, targetEnt))
                return;
            var index = touchingColliders.BinarySearch(cache);
            if (index < 0)
            {
                touchingColliders.Insert(~index, cache);
            }
        }

        #region 检测
        public bool CheckSphere(Vector3 center, float radius)
        {
            var closest = boxCollider.ClosestPoint(center);
            return (closest - (transform.position + boxCollider.center)).sqrMagnitude < radius * radius;
        }
        #endregion

        #region 碰撞
        public void GetCollisions(List<EntityCollision> collisions)
        {
            collisions.AddRange(collisionList);
        }
        private bool CallPreCollision(EntityCollision collision)
        {
            return Entity.PreCollision(collision);
        }
        private void CallPostCollision(EntityCollision collision, int state)
        {
            Entity.PostCollision(collision, state);
        }
        #endregion

        #region 序列化
        public EntityColliderReference ToReference()
        {
            return new EntityColliderReference(Entity.ID, Name);
        }
        public SerializableUnityEntityCollider ToSerializable()
        {
            return new SerializableUnityEntityCollider()
            {
                name = Name,
                enabled = Enabled,
                armorSlot = ArmorSlot,
                collisionList = collisionList.Select(c => c.ToSerializable()).ToArray(),
                updateMode = (int)updateMode,
                customSize = customSize,
                customOffset = customOffset,
                customPivot = customPivot,
            };
        }
        public void LoadFromSerializable(ISerializableCollisionCollider seri, Entity entity)
        {
            Entity = entity;
            Name = seri.Name;
            ArmorSlot = seri.ArmorSlot;
            updateMode = (ColliderUpdateMode)seri.UpdateMode;
            customSize = seri.CustomSize;
            customOffset = seri.CustomOffset;
            customPivot = seri.CustomPivot;
            gameObject.name = Name;
            SetEnabled(seri.Enabled);
        }
        public void LoadCollisions(LevelEngine level, ISerializableCollisionCollider seri)
        {
            collisionList = seri.Collisions.Select(s => EntityCollision.FromSerializable(s, level)).ToList();
        }
        #endregion

        public bool Enabled { get; private set; } = true;
        public string Name { get; private set; }
        public Entity Entity { get; private set; }
        public NamespaceID ArmorSlot { get; private set; }
        private ColliderUpdateMode updateMode;
        private Vector3 customSize = Vector3.zero;
        private Vector3 customOffset = Vector3.zero;
        private Vector3 customPivot = Vector3.one * 0.5f;

        [SerializeField]
        private BoxCollider boxCollider;
        private List<ColliderCache> touchingColliders = new List<ColliderCache>();
        private List<EntityCollision> collisionList = new List<EntityCollision>();
        private ArrayBuffer<EntityCollision> collisionBuffer = new ArrayBuffer<EntityCollision>(1024);

        private struct ColliderCache : IComparable<ColliderCache>, IEquatable<ColliderCache>
        {
            public ColliderCache(Collider collider, Entity self, long colliderEntityID)
            {
                var prevPos = self.PreviousPosition;
                this.collider = collider;
                sqrDistance = (collider.attachedRigidbody.position - prevPos).sqrMagnitude;
                colliderID = colliderEntityID;
            }
            public int CompareTo(ColliderCache other)
            {
                var value = sqrDistance.CompareTo(other.sqrDistance);
                if (value != 0)
                    return value;
                return colliderID.CompareTo(other.colliderID);
            }
            public bool Equals(ColliderCache other)
            {
                return collider == other.collider;
            }
            public override int GetHashCode()
            {
                return collider.GetHashCode();
            }
            public Collider collider;
            public float sqrDistance;
            public long colliderID;
        }
    }
    public class SerializableUnityEntityCollider : ISerializableCollisionCollider
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
