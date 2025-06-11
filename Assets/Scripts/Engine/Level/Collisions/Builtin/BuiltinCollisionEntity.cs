﻿using System;
using System.Collections.Generic;
using PVZEngine.Entities;

namespace PVZEngine.Level.Collisions
{
    public class BuiltinCollisionEntity
    {
        public BuiltinCollisionEntity()
        {
        }
        public void Init(Entity entity)
        {
            this.entity = entity;
            var mainCollider = new BuiltinCollisionCollider(entity, EntityCollisionHelper.NAME_MAIN, new EntityHitbox(entity));
            AddCollider(mainCollider);
        }
        public void Update()
        {
        }
        public void UpdateEntityDetection()
        {
            foreach (var collider in colliders)
            {
                collider.SetIgnored(entity.GetCollisionDetection() == EntityCollisionHelper.DETECTION_IGNORE);
            }
        }
        public void UpdateEntityPosition()
        {
        }
        public void UpdateEntitySize()
        {
            foreach (var collider in colliders)
            {
                collider.ReevaluateBounds();
            }
        }
        public void GetCurrentCollisions(List<EntityCollision> collisions)
        {
            foreach (var collider in colliders)
            {
                collider.GetCollisions(collisions);
            }
        }
        public void ClearColliders()
        {
            foreach (var collider in colliders)
            {
                OnEntityColliderRemove?.Invoke(this, collider);
                collider.OnEnabled -= OnColliderEnabledCallback;
                collider.OnDisabled -= OnColliderDisabledCallback;
            }
            colliders.Clear();
        }

        #region 碰撞体
        public BuiltinCollisionCollider CreateCustomCollider(ColliderConstructor cons)
        {
            var hitbox = new CustomHitbox(entity);
            hitbox.SetSize(cons.size);
            hitbox.SetPivot(cons.pivot);
            hitbox.SetOffset(cons.offset);
            var collider = new BuiltinCollisionCollider(entity, cons.name, hitbox);
            collider.ArmorSlot = cons.armorSlot;
            collider.ReevaluateBounds();
            AddCollider(collider);
            return collider;
        }
        public void AddCollider(BuiltinCollisionCollider collider)
        {
            colliders.Add(collider);
            collider.OnEnabled += OnColliderEnabledCallback;
            collider.OnDisabled += OnColliderDisabledCallback;
            OnEntityColliderAdd?.Invoke(this, collider);
        }
        public bool RemoveCollider(string name)
        {
            foreach (var collider in colliders)
            {
                if (collider.Name == name)
                {
                    colliders.Remove(collider);
                    collider.OnEnabled -= OnColliderEnabledCallback;
                    collider.OnDisabled -= OnColliderDisabledCallback;
                    OnEntityColliderRemove?.Invoke(this, collider);
                    return true;
                }
            }
            return false;
        }
        public BuiltinCollisionCollider GetCollider(string name)
        {
            foreach (var collider in colliders)
            {
                if (collider.Name == name)
                    return collider;
            }
            return null;
        }
        private void OnColliderEnabledCallback(BuiltinCollisionCollider collider)
        {
            OnEntityColliderEnabled?.Invoke(this, collider);
        }
        private void OnColliderDisabledCallback(BuiltinCollisionCollider collider)
        {
            OnEntityColliderDisabled?.Invoke(this, collider);
        }
        #endregion


        public SerializableBuiltinCollisionSystemEntity ToSerializable()
        {
            return new SerializableBuiltinCollisionSystemEntity()
            {
                id = entity.ID,
                colliders = colliders.ConvertAll(c => c.ToSerializable()).ToArray()
            };
        }
        public void LoadFromSerializable(LevelEngine level, ISerializableCollisionEntity seri)
        {
            if (seri == null)
                return;
            var ent = level.FindEntityByID(seri.ID);
            entity = ent;
            foreach (var seriCollider in seri.Colliders)
            {
                var collider = BuiltinCollisionCollider.FromSerializable(seriCollider, ent);
                AddCollider(collider);
            }
        }
        public void LoadCollisions(LevelEngine level, ISerializableCollisionEntity seri)
        {
            if (seri == null)
                return;
            for (int i = 0; i < colliders.Count; i++)
            {
                var collider = colliders[i];
                var seriCollider = seri.Colliders[i];
                collider.LoadCollisions(level, seriCollider);
            }
        }
        public event Action<BuiltinCollisionEntity, BuiltinCollisionCollider> OnEntityColliderEnabled;
        public event Action<BuiltinCollisionEntity, BuiltinCollisionCollider> OnEntityColliderDisabled;
        public event Action<BuiltinCollisionEntity, BuiltinCollisionCollider> OnEntityColliderAdd;
        public event Action<BuiltinCollisionEntity, BuiltinCollisionCollider> OnEntityColliderRemove;
        public long ID => Entity.ID;
        public Entity Entity => entity;
        private Entity entity;
        private List<BuiltinCollisionCollider> colliders = new List<BuiltinCollisionCollider>();
    }
    public class SerializableBuiltinCollisionSystemEntity : ISerializableCollisionEntity
    {
        public long id;
        public SerializableEntityCollider[] colliders;
        long ISerializableCollisionEntity.ID => id;
        ISerializableCollisionCollider[] ISerializableCollisionEntity.Colliders => colliders;
    }
}
