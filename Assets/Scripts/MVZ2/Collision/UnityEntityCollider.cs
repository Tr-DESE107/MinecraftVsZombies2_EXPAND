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
            updateMode = EntityColliderUpdateMode.Main;
            ArmorSlot = null;
            customSize = Vector3.zero;
            customOffset = Vector3.zero;
            customPivot = Vector3.one * 0.5f;
        }
        public void SetCustom(ColliderConstructor constructor)
        {
            updateMode = EntityColliderUpdateMode.Custom;
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
                case EntityColliderUpdateMode.Main:
                    {
                        boundsSize = Entity.GetSize();
                        boundsPivot = Entity.GetBoundsPivot();
                        boundsOffset = Vector3.zero;
                    }
                    break;
                case EntityColliderUpdateMode.Custom:
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
            foreach (var collider in touchingColliders)
            {
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
            if (touchingColliders.Contains(other))
                return;
            var otherCollider = other.GetComponent<UnityEntityCollider>();
            if (!otherCollider)
                return;
            var targetEnt = otherCollider.Entity;
            var mask = Entity.IsHostile(targetEnt) ? Entity.CollisionMaskHostile : Entity.CollisionMaskFriendly;
            if (!EntityCollisionHelper.CanCollide(mask, targetEnt))
                return;
            touchingColliders.Add(other);
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
        public void LoadFromSerializable(SerializableUnityEntityCollider seri, Entity entity)
        {
            Entity = entity;
            Name = seri.name;
            ArmorSlot = seri.armorSlot;
            updateMode = (EntityColliderUpdateMode)seri.updateMode;
            customSize = seri.customSize;
            customOffset = seri.customOffset;
            customPivot = seri.customPivot;
            gameObject.name = Name;
            SetEnabled(seri.enabled);
        }
        public void LoadCollisions(LevelEngine level, SerializableUnityEntityCollider seri)
        {
            collisionList = seri.collisionList.Select(s => EntityCollision.FromSerializable(s, level)).ToList();
        }
        #endregion

        public bool Enabled { get; private set; } = true;
        public string Name { get; private set; }
        public Entity Entity { get; private set; }
        public NamespaceID ArmorSlot { get; private set; }
        private EntityColliderUpdateMode updateMode;
        private Vector3 customSize = Vector3.zero;
        private Vector3 customOffset = Vector3.zero;
        private Vector3 customPivot = Vector3.one * 0.5f;

        [SerializeField]
        private BoxCollider boxCollider;
        private HashSet<Collider> touchingColliders = new HashSet<Collider>();
        private List<EntityCollision> collisionList = new List<EntityCollision>();
        private ArrayBuffer<EntityCollision> collisionBuffer = new ArrayBuffer<EntityCollision>(1024);
    }
    public class SerializableUnityEntityCollider
    {
        public string name;
        public bool enabled;
        public NamespaceID armorSlot;
        public SerializableEntityCollision[] collisionList;
        public int updateMode;
        public Vector3 customSize;
        public Vector3 customOffset;
        public Vector3 customPivot;
    }
    public enum EntityColliderUpdateMode
    {
        Main = 0,
        Custom = 1
    }
}
